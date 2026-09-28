using RFactory.Application.Modules.DeliveryNote.DTOs;
using RFactory.Application.Modules.Organizations.DTOs;
using RFactory.Shared.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFactory.Application.Modules.Organizations.Services
{
    public interface ICompanyService
    {
        Task<List<CompanyDto>> GetAllAsync(CancellationToken ct = default);
        Task<CompanyDto?> GetByIdAsync(ulong id, CancellationToken ct = default);
        Task<Result<CompanyDto>> CreateAsync(CompanyRequest request, CancellationToken ct = default);
        Task<Result<CompanyDto>> UpdateAsync(ulong id, CompanyRequest request, CancellationToken ct = default);
        Task<Result> DeleteAsync(ulong id, CancellationToken ct = default);
    }
}
