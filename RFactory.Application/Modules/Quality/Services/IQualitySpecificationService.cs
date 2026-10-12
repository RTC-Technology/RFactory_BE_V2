using RFactory.Application.Modules.Organizations.DTOs;
using RFactory.Application.Modules.Quality.DTOs;
using RFactory.Shared.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFactory.Application.Modules.Quality.Services
{
    public interface IQualitySpecificationService
    {
        Task<List<QualitySpecificationDto>> GetAllAsync(CancellationToken ct = default);
        Task<QualitySpecificationDto?> GetByIdAsync(ulong id, CancellationToken ct = default);
        Task<Result<QualitySpecificationDto>> CreateAsync(QualitySpecificationRequest request, CancellationToken ct = default);
        Task<Result<QualitySpecificationDto>> UpdateAsync(ulong id, QualitySpecificationRequest request, CancellationToken ct = default);
        Task<Result> DeleteAsync(ulong id, CancellationToken ct = default);
    }

    public interface IQualitySpecificationItemService
    {
        Task<List<QualitySpecificationItemDto>> GetAllAsync(CancellationToken ct = default);
        Task<QualitySpecificationItemDto?> GetByIdAsync(ulong id, CancellationToken ct = default);
        Task<Result<QualitySpecificationItemDto>> CreateAsync(QualitySpecificationItemRequest request, CancellationToken ct = default);
        Task<Result<QualitySpecificationItemDto>> UpdateAsync(ulong id, QualitySpecificationItemRequest request, CancellationToken ct = default);
        Task<Result> DeleteAsync(ulong id, CancellationToken ct = default);
    }

    public interface IQualitySpecificationProductService
    {
        Task<List<QualitySpecificationProductDto>> GetAllAsync(CancellationToken ct = default);
        Task<QualitySpecificationProductDto?> GetByIdAsync(ulong id, CancellationToken ct = default);
        Task<Result<QualitySpecificationProductDto>> CreateAsync(QualitySpecificationProductRequest request, CancellationToken ct = default);
        Task<Result<QualitySpecificationProductDto>> UpdateAsync(ulong id, QualitySpecificationProductRequest request, CancellationToken ct = default);
        Task<Result> DeleteAsync(ulong id, CancellationToken ct = default);
    }
}
