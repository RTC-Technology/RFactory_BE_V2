using Microsoft.EntityFrameworkCore;
using RFactory.Application.Modules.MasterData.DTOs;
using RFactory.Infrastructure.Data;
using RFactory.Infrastructure.Entities;
using RFactory.Shared.Results;

namespace RFactory.Application.Modules.MasterData.Services;

public class LabelTemplateService : ILabelTemplateService
{
    private readonly RFactoryContext _db;

    public LabelTemplateService(RFactoryContext db)
    {
        _db = db;
    }

    public async Task<List<LabelTemplateDto>> GetAllAsync(CancellationToken ct = default)
    {
        var entities = await _db.LabelTemplates
            .OrderBy(t => t.EntityType)
            .ThenByDescending(t => t.IsDefault)
            .ThenBy(t => t.TemplateName)
            .ToListAsync(ct);

        return entities.Select(MapToDto).ToList();
    }

    public async Task<LabelTemplateDto?> GetByIdAsync(ulong id, CancellationToken ct = default)
    {
        var entity = await _db.LabelTemplates.FirstOrDefaultAsync(t => t.Id == id, ct);
        return entity == null ? null : MapToDto(entity);
    }

    public async Task<List<LabelTemplateDto>> GetByEntityTypeAsync(string entityType, CancellationToken ct = default)
    {
        var type = entityType.ToUpperInvariant();
        var entities = await _db.LabelTemplates
            .Where(t => t.EntityType == type)
            .OrderByDescending(t => t.IsDefault)
            .ThenBy(t => t.TemplateName)
            .ToListAsync(ct);

        return entities.Select(MapToDto).ToList();
    }

    public async Task<LabelTemplateDto?> GetDefaultByEntityTypeAsync(string entityType, CancellationToken ct = default)
    {
        var type = entityType.ToUpperInvariant();
        var entity = await _db.LabelTemplates
            .FirstOrDefaultAsync(t => t.EntityType == type && t.IsDefault == 1, ct);

        entity ??= await _db.LabelTemplates
            .FirstOrDefaultAsync(t => t.EntityType == type, ct);

        return entity == null ? null : MapToDto(entity);
    }

    public async Task<Result<LabelTemplateDto>> CreateAsync(LabelTemplateRequest request, CancellationToken ct = default)
    {
        var code = request.TemplateCode?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(code))
        {
            return Result<LabelTemplateDto>.Failure("Template code is required.");
        }

        var exists = await _db.LabelTemplates.AnyAsync(t => t.TemplateCode == code, ct);
        if (exists)
        {
            return Result<LabelTemplateDto>.Failure($"Template with code '{code}' already exists.");
        }

        var entityType = (request.EntityType ?? "PRODUCT").ToUpperInvariant();

        // If marked default, unset other defaults for same entity type
        if (request.IsDefault)
        {
            await UnsetOtherDefaultsAsync(entityType, 0, ct);
        }

        var entity = new LabelTemplate
        {
            TemplateCode = code,
            TemplateName = request.TemplateName.Trim(),
            EntityType = entityType,
            WidthMm = request.WidthMm > 0 ? request.WidthMm : 70.00m,
            HeightMm = request.HeightMm > 0 ? request.HeightMm : 50.00m,
            Orientation = (request.Orientation ?? "LANDSCAPE").ToUpperInvariant(),
            IsDefault = request.IsDefault ? 1UL : 0UL,
            IsActive = request.IsActive ? 1UL : 0UL,
            HeaderText = request.HeaderText?.Trim(),
            FooterText = request.FooterText?.Trim(),
            ShowQr = request.ShowQr ? 1UL : 0UL,
            ShowBarcode = request.ShowBarcode ? 1UL : 0UL,
            ShowLogo = request.ShowLogo ? 1UL : 0UL,
            ShowBorder = request.ShowBorder ? 1UL : 0UL,
            QrSizePx = request.QrSizePx > 0 ? request.QrSizePx : 150,
            FieldsConfigJson = request.FieldsConfigJson,
            CreatedDate = DateTime.UtcNow
        };

        _db.LabelTemplates.Add(entity);
        await _db.SaveChangesAsync(ct);

        return Result<LabelTemplateDto>.Success(MapToDto(entity));
    }

    public async Task<Result<LabelTemplateDto>> UpdateAsync(ulong id, LabelTemplateRequest request, CancellationToken ct = default)
    {
        var entity = await _db.LabelTemplates.FirstOrDefaultAsync(t => t.Id == id, ct);
        if (entity == null)
        {
            return Result<LabelTemplateDto>.Failure($"Label template #{id} not found.");
        }

        var code = request.TemplateCode?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(code))
        {
            return Result<LabelTemplateDto>.Failure("Template code is required.");
        }

        var clash = await _db.LabelTemplates.AnyAsync(t => t.TemplateCode == code && t.Id != id, ct);
        if (clash)
        {
            return Result<LabelTemplateDto>.Failure($"Template with code '{code}' already exists.");
        }

        var entityType = (request.EntityType ?? entity.EntityType).ToUpperInvariant();

        if (request.IsDefault && entity.IsDefault == 0)
        {
            await UnsetOtherDefaultsAsync(entityType, id, ct);
        }

        entity.TemplateCode = code;
        entity.TemplateName = request.TemplateName.Trim();
        entity.EntityType = entityType;
        entity.WidthMm = request.WidthMm > 0 ? request.WidthMm : entity.WidthMm;
        entity.HeightMm = request.HeightMm > 0 ? request.HeightMm : entity.HeightMm;
        entity.Orientation = (request.Orientation ?? "LANDSCAPE").ToUpperInvariant();
        entity.IsDefault = request.IsDefault ? 1UL : 0UL;
        entity.IsActive = request.IsActive ? 1UL : 0UL;
        entity.HeaderText = request.HeaderText?.Trim();
        entity.FooterText = request.FooterText?.Trim();
        entity.ShowQr = request.ShowQr ? 1UL : 0UL;
        entity.ShowBarcode = request.ShowBarcode ? 1UL : 0UL;
        entity.ShowLogo = request.ShowLogo ? 1UL : 0UL;
        entity.ShowBorder = request.ShowBorder ? 1UL : 0UL;
        entity.QrSizePx = request.QrSizePx > 0 ? request.QrSizePx : 150;
        entity.FieldsConfigJson = request.FieldsConfigJson;
        entity.UpdatedDate = DateTime.UtcNow;

        await _db.SaveChangesAsync(ct);
        return Result<LabelTemplateDto>.Success(MapToDto(entity));
    }

    public async Task<Result<bool>> DeleteAsync(ulong id, CancellationToken ct = default)
    {
        var entity = await _db.LabelTemplates.FirstOrDefaultAsync(t => t.Id == id, ct);
        if (entity == null)
        {
            return Result<bool>.Failure($"Label template #{id} not found.");
        }

        entity.IsDeleted = 1;
        entity.UpdatedDate = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);

        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> SetDefaultAsync(ulong id, CancellationToken ct = default)
    {
        var entity = await _db.LabelTemplates.FirstOrDefaultAsync(t => t.Id == id, ct);
        if (entity == null)
        {
            return Result<bool>.Failure($"Label template #{id} not found.");
        }

        await UnsetOtherDefaultsAsync(entity.EntityType, id, ct);
        entity.IsDefault = 1;
        entity.UpdatedDate = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);

        return Result<bool>.Success(true);
    }

    private async Task UnsetOtherDefaultsAsync(string entityType, ulong currentId, CancellationToken ct)
    {
        var others = await _db.LabelTemplates
            .Where(t => t.EntityType == entityType && t.Id != currentId && t.IsDefault == 1)
            .ToListAsync(ct);

        foreach (var other in others)
        {
            other.IsDefault = 0;
            other.UpdatedDate = DateTime.UtcNow;
        }
    }

    private static LabelTemplateDto MapToDto(LabelTemplate t) => new()
    {
        Id = t.Id,
        TemplateCode = t.TemplateCode,
        TemplateName = t.TemplateName,
        EntityType = t.EntityType,
        WidthMm = t.WidthMm,
        HeightMm = t.HeightMm,
        Orientation = t.Orientation,
        IsDefault = t.IsDefault == 1,
        IsActive = t.IsActive == 1,
        HeaderText = t.HeaderText,
        FooterText = t.FooterText,
        ShowQr = t.ShowQr == 1,
        ShowBarcode = t.ShowBarcode == 1,
        ShowLogo = t.ShowLogo == 1,
        ShowBorder = t.ShowBorder == 1,
        QrSizePx = t.QrSizePx,
        FieldsConfigJson = t.FieldsConfigJson,
        CreatedDate = t.CreatedDate
    };
}
