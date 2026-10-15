using System.Globalization;
using Microsoft.EntityFrameworkCore;
using RFactory.Application.Modules.MasterData.DTOs;
using RFactory.Infrastructure.Data;
using RFactory.Infrastructure.Entities;
using RFactory.Shared.Results;

namespace RFactory.Application.Modules.MasterData.Services;

public class PackingNumberingRuleService : IPackingNumberingRuleService
{
    private readonly RFactoryContext _db;

    public PackingNumberingRuleService(RFactoryContext db)
    {
        _db = db;
    }

    public async Task<List<PackingNumberingRuleDto>> GetAllAsync(CancellationToken ct = default)
    {
        var entities = await _db.PackingNumberingRules
            .OrderBy(r => r.TargetScope)
            .ThenByDescending(r => r.IsDefault)
            .ThenBy(r => r.RuleName)
            .ToListAsync(ct);

        return entities.Select(MapToDto).ToList();
    }

    public async Task<PackingNumberingRuleDto?> GetByIdAsync(ulong id, CancellationToken ct = default)
    {
        var entity = await _db.PackingNumberingRules.FirstOrDefaultAsync(r => r.Id == id, ct);
        return entity == null ? null : MapToDto(entity);
    }

    public async Task<List<PackingNumberingRuleDto>> GetByScopeAsync(string scope, CancellationToken ct = default)
    {
        var sc = scope.ToUpperInvariant();
        var entities = await _db.PackingNumberingRules
            .Where(r => r.TargetScope == sc)
            .OrderByDescending(r => r.IsDefault)
            .ThenBy(r => r.RuleName)
            .ToListAsync(ct);

        return entities.Select(MapToDto).ToList();
    }

    public async Task<PackingNumberingRuleDto?> GetDefaultByScopeAsync(string scope, CancellationToken ct = default)
    {
        var sc = scope.ToUpperInvariant();
        var entity = await _db.PackingNumberingRules
            .FirstOrDefaultAsync(r => r.TargetScope == sc && r.IsDefault, ct);

        entity ??= await _db.PackingNumberingRules
            .FirstOrDefaultAsync(r => r.TargetScope == sc, ct);

        return entity == null ? null : MapToDto(entity);
    }

    public async Task<Result<PackingNumberingRuleDto>> CreateAsync(PackingNumberingRuleRequest request, CancellationToken ct = default)
    {
        var code = request.RuleCode?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(code))
        {
            return Result<PackingNumberingRuleDto>.Failure("Rule code is required.");
        }

        var exists = await _db.PackingNumberingRules.AnyAsync(r => r.RuleCode == code, ct);
        if (exists)
        {
            return Result<PackingNumberingRuleDto>.Failure($"Numbering rule '{code}' already exists.");
        }

        var scope = (request.TargetScope ?? "CARTON").ToUpperInvariant();
        if (request.IsDefault)
        {
            await UnsetOtherDefaultsAsync(scope, 0, ct);
        }

        var entity = new PackingNumberingRule
        {
            RuleCode = code,
            RuleName = request.RuleName.Trim(),
            TargetScope = scope,
            Prefix = request.Prefix?.Trim(),
            Suffix = request.Suffix?.Trim(),
            DateFormat = (request.DateFormat ?? "YYYYMMDD").ToUpperInvariant(),
            SequenceLength = request.SequenceLength > 0 ? request.SequenceLength : 4,
            CurrentSequence = request.CurrentSequence >= 0 ? request.CurrentSequence : 0,
            ResetPeriod = (request.ResetPeriod ?? "DAILY").ToUpperInvariant(),
            IncludeCheckDigit = request.IncludeCheckDigit,
            IsDefault = request.IsDefault,
            IsActive = request.IsActive,
            Description = request.Description?.Trim(),
            LastResetDate = DateTime.UtcNow,
            CreatedDate = DateTime.UtcNow
        };

        _db.PackingNumberingRules.Add(entity);
        await _db.SaveChangesAsync(ct);

        return Result<PackingNumberingRuleDto>.Success(MapToDto(entity));
    }

    public async Task<Result<PackingNumberingRuleDto>> UpdateAsync(ulong id, PackingNumberingRuleRequest request, CancellationToken ct = default)
    {
        var entity = await _db.PackingNumberingRules.FirstOrDefaultAsync(r => r.Id == id, ct);
        if (entity == null)
        {
            return Result<PackingNumberingRuleDto>.Failure($"Numbering rule #{id} not found.");
        }

        var code = request.RuleCode?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(code))
        {
            return Result<PackingNumberingRuleDto>.Failure("Rule code is required.");
        }

        var clash = await _db.PackingNumberingRules.AnyAsync(r => r.RuleCode == code && r.Id != id, ct);
        if (clash)
        {
            return Result<PackingNumberingRuleDto>.Failure($"Numbering rule '{code}' already exists.");
        }

        var scope = (request.TargetScope ?? entity.TargetScope).ToUpperInvariant();
        if (request.IsDefault && !entity.IsDefault)
        {
            await UnsetOtherDefaultsAsync(scope, id, ct);
        }

        entity.RuleCode = code;
        entity.RuleName = request.RuleName.Trim();
        entity.TargetScope = scope;
        entity.Prefix = request.Prefix?.Trim();
        entity.Suffix = request.Suffix?.Trim();
        entity.DateFormat = (request.DateFormat ?? "YYYYMMDD").ToUpperInvariant();
        entity.SequenceLength = request.SequenceLength > 0 ? request.SequenceLength : 4;
        entity.CurrentSequence = request.CurrentSequence >= 0 ? request.CurrentSequence : entity.CurrentSequence;
        entity.ResetPeriod = (request.ResetPeriod ?? "DAILY").ToUpperInvariant();
        entity.IncludeCheckDigit = request.IncludeCheckDigit;
        entity.IsDefault = request.IsDefault;
        entity.IsActive = request.IsActive;
        entity.Description = request.Description?.Trim();
        entity.UpdatedDate = DateTime.UtcNow;

        await _db.SaveChangesAsync(ct);
        return Result<PackingNumberingRuleDto>.Success(MapToDto(entity));
    }

    public async Task<Result<bool>> DeleteAsync(ulong id, CancellationToken ct = default)
    {
        var entity = await _db.PackingNumberingRules.FirstOrDefaultAsync(r => r.Id == id, ct);
        if (entity == null)
        {
            return Result<bool>.Failure($"Numbering rule #{id} not found.");
        }

        entity.IsDeleted = true;
        entity.UpdatedDate = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);

        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> SetDefaultAsync(ulong id, CancellationToken ct = default)
    {
        var entity = await _db.PackingNumberingRules.FirstOrDefaultAsync(r => r.Id == id, ct);
        if (entity == null)
        {
            return Result<bool>.Failure($"Numbering rule #{id} not found.");
        }

        await UnsetOtherDefaultsAsync(entity.TargetScope, id, ct);
        entity.IsDefault = true;
        entity.UpdatedDate = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);

        return Result<bool>.Success(true);
    }

    public async Task<Result<GeneratedPackingCodesDto>> GenerateCodesAsync(GeneratePackingCodeRequest request, CancellationToken ct = default)
    {
        PackingNumberingRule? rule = null;

        if (request.RuleId.HasValue && request.RuleId.Value > 0)
        {
            rule = await _db.PackingNumberingRules.FirstOrDefaultAsync(r => r.Id == request.RuleId.Value, ct);
        }
        else if (!string.IsNullOrWhiteSpace(request.TargetScope))
        {
            var sc = request.TargetScope.ToUpperInvariant();
            rule = await _db.PackingNumberingRules.FirstOrDefaultAsync(r => r.TargetScope == sc && r.IsDefault, ct)
                ?? await _db.PackingNumberingRules.FirstOrDefaultAsync(r => r.TargetScope == sc, ct);
        }

        if (rule == null)
        {
            return Result<GeneratedPackingCodesDto>.Failure("No active numbering rule found for code generation.");
        }

        var count = Math.Clamp(request.Count, 1, 100);
        var now = DateTime.UtcNow;

        // Check if reset period applies
        if (ShouldResetSequence(rule.ResetPeriod, rule.LastResetDate, now))
        {
            rule.CurrentSequence = 0;
            rule.LastResetDate = now;
        }

        var codes = new List<string>();
        for (int i = 0; i < count; i++)
        {
            rule.CurrentSequence++;
            var formatted = FormatCode(rule, rule.CurrentSequence, now);
            codes.Add(formatted);
        }

        rule.UpdatedDate = now;
        await _db.SaveChangesAsync(ct);

        return Result<GeneratedPackingCodesDto>.Success(new GeneratedPackingCodesDto
        {
            RuleCode = rule.RuleCode,
            TargetScope = rule.TargetScope,
            GeneratedCodes = codes,
            GeneratedAt = now
        });
    }

    public async Task<Result<string>> PreviewCodeAsync(ulong ruleId, CancellationToken ct = default)
    {
        var rule = await _db.PackingNumberingRules.FirstOrDefaultAsync(r => r.Id == ruleId, ct);
        if (rule == null)
        {
            return Result<string>.Failure($"Numbering rule #{ruleId} not found.");
        }

        var now = DateTime.UtcNow;
        var nextSeq = rule.CurrentSequence + 1;
        var preview = FormatCode(rule, nextSeq, now);

        return Result<string>.Success(preview);
    }

    private static bool ShouldResetSequence(string resetPeriod, DateTime? lastReset, DateTime now)
    {
        if (lastReset == null) return false;
        var last = lastReset.Value;

        return resetPeriod switch
        {
            "DAILY" => last.Date < now.Date,
            "MONTHLY" => last.Year < now.Year || last.Month < now.Month,
            "YEARLY" => last.Year < now.Year,
            _ => false
        };
    }

    private static string FormatCode(PackingNumberingRule rule, long seq, DateTime now)
    {
        var prefix = rule.Prefix ?? string.Empty;
        var suffix = rule.Suffix ?? string.Empty;

        // Date token
        var datePart = rule.DateFormat switch
        {
            "YYYYMMDD" => now.ToString("yyyyMMdd"),
            "YYMMDD" => now.ToString("yyMMdd"),
            "YYYYMM" => now.ToString("yyyyMM"),
            "YYWW" => $"{now:yy}W{ISOWeek.GetWeekOfYear(now):D2}",
            _ => string.Empty
        };

        var seqPart = seq.ToString().PadLeft(rule.SequenceLength, '0');
        var baseCode = $"{prefix}{datePart}{seqPart}{suffix}";

        if (rule.IncludeCheckDigit)
        {
            var checkDigit = ComputeGs1CheckDigit(baseCode);
            baseCode += checkDigit;
        }

        return baseCode;
    }

    private static string ComputeGs1CheckDigit(string input)
    {
        // Extracts numeric characters from input for GS1 modulo 10
        var digits = input.Where(char.IsDigit).Select(c => c - '0').ToList();
        if (digits.Count == 0) return "0";

        int sum = 0;
        bool multiplyByThree = true;

        for (int i = digits.Count - 1; i >= 0; i--)
        {
            sum += digits[i] * (multiplyByThree ? 3 : 1);
            multiplyByThree = !multiplyByThree;
        }

        int remainder = sum % 10;
        int checkDigit = (10 - remainder) % 10;
        return checkDigit.ToString();
    }

    private async Task UnsetOtherDefaultsAsync(string scope, ulong currentId, CancellationToken ct)
    {
        var others = await _db.PackingNumberingRules
            .Where(r => r.TargetScope == scope && r.Id != currentId && r.IsDefault)
            .ToListAsync(ct);

        foreach (var other in others)
        {
            other.IsDefault = false;
            other.UpdatedDate = DateTime.UtcNow;
        }
    }

    private static PackingNumberingRuleDto MapToDto(PackingNumberingRule r)
    {
        var now = DateTime.UtcNow;
        var preview = FormatCode(r, r.CurrentSequence > 0 ? r.CurrentSequence : 1, now);

        return new PackingNumberingRuleDto
        {
            Id = r.Id,
            RuleCode = r.RuleCode,
            RuleName = r.RuleName,
            TargetScope = r.TargetScope,
            Prefix = r.Prefix,
            Suffix = r.Suffix,
            DateFormat = r.DateFormat,
            SequenceLength = r.SequenceLength,
            CurrentSequence = r.CurrentSequence,
            ResetPeriod = r.ResetPeriod,
            LastResetDate = r.LastResetDate,
            IncludeCheckDigit = r.IncludeCheckDigit,
            IsDefault = r.IsDefault,
            IsActive = r.IsActive ?? false,
            Description = r.Description,
            SamplePreview = preview,
            CreatedDate = r.CreatedDate
        };
    }
}
