using RFactory.Application.Modules.GoodsReceipt.DTOs;
using RFactory.Application.Modules.Product.DTOs;
using RFactory.Shared.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFactory.Application.Modules.GoodsReceipt.Services
{
    public interface IGoodsReceiptServices
    {
        Task<List<GoodsReceiptDto>> GetAllAsync(CancellationToken ct = default);
        Task<GoodsReceiptDto?> GetByIdAsync(ulong id, CancellationToken ct = default);
        Task<Result<GoodsReceiptDto>> CreateAsync(CreateGoodsReceiptRequest request, CancellationToken ct = default);
        Task<Result<GoodsReceiptDto>> UpdateAsync(ulong id, UpdateGoodsReceiptRequest request, CancellationToken ct = default);
        Task<Result> DeleteAsync(ulong id, CancellationToken ct = default);
    }

    //public interface IGoodsReceiptDetailServices
    //{
    //    Task<List<GoodsReceiptDetailDto>> GetAllAsync(CancellationToken ct = default);
    //    Task<GoodsReceiptDetailDto?> GetByIdAsync(ulong id, CancellationToken ct = default);
    //    Task<Result<GoodsReceiptDetailDto>> CreateAsync(CreateProductTypeRequest request, CancellationToken ct = default);
    //    Task<Result<GoodsReceiptDetailDto>> UpdateAsync(ulong id, UpdateProductTypeRequest request, CancellationToken ct = default);
    //    Task<Result> DeleteAsync(ulong id, CancellationToken ct = default);
    //}
}
