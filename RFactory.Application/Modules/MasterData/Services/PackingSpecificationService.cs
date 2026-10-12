using Microsoft.EntityFrameworkCore;
using RFactory.Application.Modules.MasterData.DTOs;
using RFactory.Infrastructure.Data;
using RFactory.Infrastructure.Entities;
using RFactory.Shared.Results;

namespace RFactory.Application.Modules.MasterData.Services;

public class PackingSpecificationService : IPackingSpecificationService
{
    private readonly RFactoryContext _db;

    public PackingSpecificationService(RFactoryContext db)
    {
        _db = db;
    }

    public async Task<List<PackingSpecificationDto>> GetAllAsync(CancellationToken ct = default)
    {
        var query = from ps in _db.PackingSpecifications
                    join p in _db.Products on ps.ProductId equals p.Id into pGroup
                    from prod in pGroup.DefaultIfEmpty()
                    join u in _db.Units on ps.PackUnitId equals u.Id into uGroup
                    from unit in uGroup.DefaultIfEmpty()
                    join lt in _db.LabelTemplates on ps.LabelTemplateId equals lt.Id into ltGroup
                    from tmpl in ltGroup.DefaultIfEmpty()
                    orderby ps.ProductId, ps.IsDefault descending, ps.SpecName
                    select MapToDto(ps, prod, unit, tmpl);

        return await query.ToListAsync(ct);
    }

    public async Task<PackingSpecificationDto?> GetByIdAsync(ulong id, CancellationToken ct = default)
    {
        var query = from ps in _db.PackingSpecifications
                    where ps.Id == id
                    join p in _db.Products on ps.ProductId equals p.Id into pGroup
                    from prod in pGroup.DefaultIfEmpty()
                    join u in _db.Units on ps.PackUnitId equals u.Id into uGroup
                    from unit in uGroup.DefaultIfEmpty()
                    join lt in _db.LabelTemplates on ps.LabelTemplateId equals lt.Id into ltGroup
                    from tmpl in ltGroup.DefaultIfEmpty()
                    select MapToDto(ps, prod, unit, tmpl);

        return await query.FirstOrDefaultAsync(ct);
    }

    public async Task<List<PackingSpecificationDto>> GetByProductIdAsync(ulong productId, CancellationToken ct = default)
    {
        var query = from ps in _db.PackingSpecifications
                    where ps.ProductId == productId
                    join p in _db.Products on ps.ProductId equals p.Id into pGroup
                    from prod in pGroup.DefaultIfEmpty()
                    join u in _db.Units on ps.PackUnitId equals u.Id into uGroup
                    from unit in uGroup.DefaultIfEmpty()
                    join lt in _db.LabelTemplates on ps.LabelTemplateId equals lt.Id into ltGroup
                    from tmpl in ltGroup.DefaultIfEmpty()
                    orderby ps.IsDefault descending, ps.SpecName
                    select MapToDto(ps, prod, unit, tmpl);

        return await query.ToListAsync(ct);
    }

    public async Task<PackingSpecificationDto?> GetDefaultByProductIdAsync(ulong productId, CancellationToken ct = default)
    {
        var query = from ps in _db.PackingSpecifications
                    where ps.ProductId == productId && ps.IsDefault == 1
                    join p in _db.Products on ps.ProductId equals p.Id into pGroup
                    from prod in pGroup.DefaultIfEmpty()
                    join u in _db.Units on ps.PackUnitId equals u.Id into uGroup
                    from unit in uGroup.DefaultIfEmpty()
                    join lt in _db.LabelTemplates on ps.LabelTemplateId equals lt.Id into ltGroup
                    from tmpl in ltGroup.DefaultIfEmpty()
                    select MapToDto(ps, prod, unit, tmpl);

        var result = await query.FirstOrDefaultAsync(ct);
        if (result != null) return result;

        // Fallback to first available spec for this product
        var fallback = from ps in _db.PackingSpecifications
                       where ps.ProductId == productId
                       join p in _db.Products on ps.ProductId equals p.Id into pGroup
                       from prod in pGroup.DefaultIfEmpty()
                       join u in _db.Units on ps.PackUnitId equals u.Id into uGroup
                       from unit in uGroup.DefaultIfEmpty()
                       join lt in _db.LabelTemplates on ps.LabelTemplateId equals lt.Id into ltGroup
                       from tmpl in ltGroup.DefaultIfEmpty()
                       select MapToDto(ps, prod, unit, tmpl);

        return await fallback.FirstOrDefaultAsync(ct);
    }

    public async Task<Result<PackingSpecificationDto>> CreateAsync(PackingSpecificationRequest request, CancellationToken ct = default)
    {
        var code = request.SpecCode?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(code))
        {
            return Result<PackingSpecificationDto>.Failure("Packing specification code is required.");
        }

        var exists = await _db.PackingSpecifications.AnyAsync(s => s.SpecCode == code, ct);
        if (exists)
        {
            return Result<PackingSpecificationDto>.Failure($"Specification with code '{code}' already exists.");
        }

        if (request.ProductId.HasValue && request.IsDefault)
        {
            await UnsetOtherDefaultsAsync(request.ProductId.Value, 0, ct);
        }

        var volume = CalculateVolume(request.LengthMm, request.WidthMm, request.HeightMm, request.VolumeM3);
        var tare = CalculateTare(request.GrossWeightKg, request.NetWeightKg, request.TareWeightKg);
        var palletTotal = CalculatePalletUnits(request.PalletTi, request.PalletHi, request.QuantityPerPack, request.TotalUnitsPerPallet);

        var entity = new PackingSpecification
        {
            SpecCode = code,
            SpecName = request.SpecName.Trim(),
            ProductId = request.ProductId,
            CustomerCode = request.CustomerCode?.Trim(),
            CustomerName = request.CustomerName?.Trim(),
            PackagingType = (request.PackagingType ?? "CARTON").ToUpperInvariant(),
            QuantityPerPack = request.QuantityPerPack > 0 ? request.QuantityPerPack : 1,
            PackUnitId = request.PackUnitId,
            LengthMm = request.LengthMm,
            WidthMm = request.WidthMm,
            HeightMm = request.HeightMm,
            VolumeM3 = volume,
            NetWeightKg = request.NetWeightKg,
            GrossWeightKg = request.GrossWeightKg,
            TareWeightKg = tare,
            InnerPackQty = request.InnerPackQty,
            MasterCartonQty = request.MasterCartonQty,
            PalletTi = request.PalletTi,
            PalletHi = request.PalletHi,
            TotalUnitsPerPallet = palletTotal,
            LabelTemplateId = request.LabelTemplateId,
            Barcode = request.Barcode?.Trim(),
            PackagingMaterial = request.PackagingMaterial?.Trim(),
            Instructions = request.Instructions?.Trim(),
            IsDefault = request.IsDefault ? 1UL : 0UL,
            Status = (request.Status ?? "ACTIVE").ToUpperInvariant(),
            CreatedDate = DateTime.UtcNow
        };

        _db.PackingSpecifications.Add(entity);
        await _db.SaveChangesAsync(ct);

        return Result<PackingSpecificationDto>.Success(await GetByIdAsync(entity.Id, ct) ?? MapToDto(entity, null, null, null));
    }

    public async Task<Result<PackingSpecificationDto>> UpdateAsync(ulong id, PackingSpecificationRequest request, CancellationToken ct = default)
    {
        var entity = await _db.PackingSpecifications.FirstOrDefaultAsync(s => s.Id == id, ct);
        if (entity == null)
        {
            return Result<PackingSpecificationDto>.Failure($"Packing specification #{id} not found.");
        }

        var code = request.SpecCode?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(code))
        {
            return Result<PackingSpecificationDto>.Failure("Packing specification code is required.");
        }

        var clash = await _db.PackingSpecifications.AnyAsync(s => s.SpecCode == code && s.Id != id, ct);
        if (clash)
        {
            return Result<PackingSpecificationDto>.Failure($"Specification with code '{code}' already exists.");
        }

        if (request.ProductId.HasValue && request.IsDefault && entity.IsDefault == 0)
        {
            await UnsetOtherDefaultsAsync(request.ProductId.Value, id, ct);
        }

        var volume = CalculateVolume(request.LengthMm, request.WidthMm, request.HeightMm, request.VolumeM3);
        var tare = CalculateTare(request.GrossWeightKg, request.NetWeightKg, request.TareWeightKg);
        var palletTotal = CalculatePalletUnits(request.PalletTi, request.PalletHi, request.QuantityPerPack, request.TotalUnitsPerPallet);

        entity.SpecCode = code;
        entity.SpecName = request.SpecName.Trim();
        entity.ProductId = request.ProductId;
        entity.CustomerCode = request.CustomerCode?.Trim();
        entity.CustomerName = request.CustomerName?.Trim();
        entity.PackagingType = (request.PackagingType ?? "CARTON").ToUpperInvariant();
        entity.QuantityPerPack = request.QuantityPerPack > 0 ? request.QuantityPerPack : 1;
        entity.PackUnitId = request.PackUnitId;
        entity.LengthMm = request.LengthMm;
        entity.WidthMm = request.WidthMm;
        entity.HeightMm = request.HeightMm;
        entity.VolumeM3 = volume;
        entity.NetWeightKg = request.NetWeightKg;
        entity.GrossWeightKg = request.GrossWeightKg;
        entity.TareWeightKg = tare;
        entity.InnerPackQty = request.InnerPackQty;
        entity.MasterCartonQty = request.MasterCartonQty;
        entity.PalletTi = request.PalletTi;
        entity.PalletHi = request.PalletHi;
        entity.TotalUnitsPerPallet = palletTotal;
        entity.LabelTemplateId = request.LabelTemplateId;
        entity.Barcode = request.Barcode?.Trim();
        entity.PackagingMaterial = request.PackagingMaterial?.Trim();
        entity.Instructions = request.Instructions?.Trim();
        entity.IsDefault = request.IsDefault ? 1UL : 0UL;
        entity.Status = (request.Status ?? "ACTIVE").ToUpperInvariant();
        entity.UpdatedDate = DateTime.UtcNow;

        await _db.SaveChangesAsync(ct);
        return Result<PackingSpecificationDto>.Success(await GetByIdAsync(entity.Id, ct) ?? MapToDto(entity, null, null, null));
    }

    public async Task<Result<bool>> DeleteAsync(ulong id, CancellationToken ct = default)
    {
        var entity = await _db.PackingSpecifications.FirstOrDefaultAsync(s => s.Id == id, ct);
        if (entity == null)
        {
            return Result<bool>.Failure($"Packing specification #{id} not found.");
        }

        entity.IsDeleted = 1;
        entity.UpdatedDate = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);

        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> SetDefaultAsync(ulong id, CancellationToken ct = default)
    {
        var entity = await _db.PackingSpecifications.FirstOrDefaultAsync(s => s.Id == id, ct);
        if (entity == null)
        {
            return Result<bool>.Failure($"Packing specification #{id} not found.");
        }

        if (entity.ProductId.HasValue)
        {
            await UnsetOtherDefaultsAsync(entity.ProductId.Value, id, ct);
        }

        entity.IsDefault = 1;
        entity.UpdatedDate = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);

        return Result<bool>.Success(true);
    }

    private async Task UnsetOtherDefaultsAsync(ulong productId, ulong currentId, CancellationToken ct)
    {
        var others = await _db.PackingSpecifications
            .Where(s => s.ProductId == productId && s.Id != currentId && s.IsDefault == 1)
            .ToListAsync(ct);

        foreach (var other in others)
        {
            other.IsDefault = 0;
            other.UpdatedDate = DateTime.UtcNow;
        }
    }

    private static decimal CalculateVolume(decimal l, decimal w, decimal h, decimal manualVol)
    {
        if (manualVol > 0) return manualVol;
        if (l > 0 && w > 0 && h > 0)
        {
            return Math.Round((l * w * h) / 1_000_000_000m, 4);
        }
        return 0;
    }

    private static decimal CalculateTare(decimal gross, decimal net, decimal manualTare)
    {
        if (manualTare > 0) return manualTare;
        if (gross > net && net > 0)
        {
            return Math.Round(gross - net, 3);
        }
        return 0;
    }

    private static decimal? CalculatePalletUnits(int? ti, int? hi, decimal qtyPerPack, decimal? manualTotal)
    {
        if (manualTotal.HasValue && manualTotal.Value > 0) return manualTotal.Value;
        if (ti.HasValue && ti.Value > 0 && hi.HasValue && hi.Value > 0 && qtyPerPack > 0)
        {
            return (decimal)(ti.Value * hi.Value) * qtyPerPack;
        }
        return manualTotal;
    }

    private static PackingSpecificationDto MapToDto(
        PackingSpecification ps,
        Product? prod,
        Unit? unit,
        LabelTemplate? tmpl) => new()
    {
        Id = ps.Id,
        SpecCode = ps.SpecCode,
        SpecName = ps.SpecName,
        ProductId = ps.ProductId,
        ProductCode = prod?.ProductCode,
        ProductName = prod?.ProductName,
        CustomerCode = ps.CustomerCode,
        CustomerName = ps.CustomerName,
        PackagingType = ps.PackagingType,
        QuantityPerPack = ps.QuantityPerPack,
        PackUnitId = ps.PackUnitId,
        PackUnitName = unit?.UnitName,
        LengthMm = ps.LengthMm,
        WidthMm = ps.WidthMm,
        HeightMm = ps.HeightMm,
        VolumeM3 = ps.VolumeM3,
        NetWeightKg = ps.NetWeightKg,
        GrossWeightKg = ps.GrossWeightKg,
        TareWeightKg = ps.TareWeightKg,
        InnerPackQty = ps.InnerPackQty,
        MasterCartonQty = ps.MasterCartonQty,
        PalletTi = ps.PalletTi,
        PalletHi = ps.PalletHi,
        TotalUnitsPerPallet = ps.TotalUnitsPerPallet,
        LabelTemplateId = ps.LabelTemplateId,
        LabelTemplateName = tmpl?.TemplateName,
        Barcode = ps.Barcode,
        PackagingMaterial = ps.PackagingMaterial,
        Instructions = ps.Instructions,
        IsDefault = ps.IsDefault == 1,
        Status = ps.Status,
        CreatedDate = ps.CreatedDate
    };
}
