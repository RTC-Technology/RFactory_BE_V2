using RFactory.Application.Modules.MasterData.DTOs;
using RFactory.Shared.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFactory.Application.Modules.MasterData.Services
{
    public interface ICustomerService
    {
        Task<List<CustomerDto>> GetAllAsync(CancellationToken ct = default);
        Task<CustomerDto?> GetByIdAsync(ulong id, CancellationToken ct = default);
        Task<Result<CustomerDto>> CreateAsync(CustomerRequest request, CancellationToken ct = default);
        Task<Result<CustomerDto>> UpdateAsync(ulong id, CustomerRequest request, CancellationToken ct = default);
        Task<Result> DeleteAsync(ulong id, CancellationToken ct = default);
    }

    public interface ICustomerContactService
    {
        Task<List<CustomerContactDto>> GetAllAsync(CancellationToken ct = default);
        Task<CustomerContactDto?> GetByIdAsync(ulong id, CancellationToken ct = default);
        Task<Result<CustomerContactDto>> CreateAsync(CustomerContactRequest request, CancellationToken ct = default);
        Task<Result<CustomerContactDto>> UpdateAsync(ulong id, CustomerContactRequest request, CancellationToken ct = default);
        Task<Result> DeleteAsync(ulong id, CancellationToken ct = default);
    }
}
