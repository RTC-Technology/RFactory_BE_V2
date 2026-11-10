using System.Collections.Generic;
using System.Threading.Tasks;
using RFactory.Application.Modules.Production.DTOs;

namespace RFactory.Application.Modules.Production.Services;

public interface IDetailedProductionPlanService
{
    Task<List<DetailedProductionPlanDto>> GetAllAsync(DetailedProductionPlanFilterDto? filter = null);
    Task<DetailedProductionPlanDto?> GetByIdAsync(ulong id);
    Task<DetailedProductionPlanDto> CreateAsync(DetailedProductionPlanCreateRequest request, string currentUser);
    Task<DetailedProductionPlanDto> UpdateAsync(ulong id, DetailedProductionPlanUpdateRequest request, string currentUser);
    Task<bool> DeleteAsync(ulong id, string currentUser);
    Task<DetailedProductionPlanDto> ChangeStatusAsync(ulong id, int newStatus, string currentUser, string? remark = null);

    Task<List<MasterProductionPlanDto>> GetMasterPlansAsync();
    Task<MasterProductionPlanDto?> GetMasterPlanByIdAsync(ulong masterPlanId);

    PlanAllocationCalculationResultDto CalculateAllocation(PlanAllocationCalculationRequest request);
    ValidateQuantitiesResultDto ValidateQuantities(ValidateQuantitiesRequest request);

    Task<GenerateOrdersResultDto> GenerateProductionOrdersAsync(GenerateOrdersFromPlanRequest request, string currentUser);
}
