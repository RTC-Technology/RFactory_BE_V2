using RFactory.Application.Modules.DeliveryNote.DTOs;
using RFactory.Application.Modules.PickingPlan.DTOs;
using RFactory.Shared.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFactory.Application.Modules.DeliveryNote.Services
{
    public interface IDeliveryNoteService
    {
        Task<List<DeliveryNoteDto>> GetAllAsync(CancellationToken ct = default);
        Task<DeliveryNoteDto?> GetByIdAsync(ulong id, CancellationToken ct = default);
        Task<Result<DeliveryNoteDto>> CreateAsync(DeliveryNoteRequest request, CancellationToken ct = default);
        Task<Result<DeliveryNoteDto>> UpdateAsync(ulong id, DeliveryNoteRequest request, CancellationToken ct = default);
        Task<Result> DeleteAsync(ulong id, CancellationToken ct = default);
    }
    public interface IDeliveryNoteItemService
    {
        Task<List<DeliveryNoteItemDto>> GetAllAsync(CancellationToken ct = default);
        Task<DeliveryNoteItemDto?> GetByIdAsync(ulong id, CancellationToken ct = default);
        Task<Result<DeliveryNoteItemDto>> CreateAsync(DeliveryNoteItemRequest request, CancellationToken ct = default);
        Task<Result<DeliveryNoteItemDto>> UpdateAsync(ulong id, DeliveryNoteItemRequest request, CancellationToken ct = default);
        Task<Result> DeleteAsync(ulong id, CancellationToken ct = default);
    }
    public interface IDeliveryNoteSenderService
    {
        Task<List<DeliveryNoteSenderDto>> GetAllAsync(CancellationToken ct = default);
        Task<DeliveryNoteSenderDto?> GetByIdAsync(ulong id, CancellationToken ct = default);
        Task<Result<DeliveryNoteSenderDto>> CreateAsync(DeliveryNoteSenderRequest request, CancellationToken ct = default);
        Task<Result<DeliveryNoteSenderDto>> UpdateAsync(ulong id, DeliveryNoteSenderRequest request, CancellationToken ct = default);
        Task<Result> DeleteAsync(ulong id, CancellationToken ct = default);
    }
    public interface IDeliveryNoteSourceService
    {
        Task<List<DeliveryNoteSourceDto>> GetAllAsync(CancellationToken ct = default);
        Task<DeliveryNoteSourceDto?> GetByIdAsync(ulong id, CancellationToken ct = default);
        Task<Result<DeliveryNoteSourceDto>> CreateAsync(DeliveryNoteSourceRequest request, CancellationToken ct = default);
        Task<Result<DeliveryNoteSourceDto>> UpdateAsync(ulong id, DeliveryNoteSourceRequest request, CancellationToken ct = default);
        Task<Result> DeleteAsync(ulong id, CancellationToken ct = default);
    }
    public interface IDeliveryNoteReceiverService
    {
        Task<List<DeliveryNoteReceiverDto>> GetAllAsync(CancellationToken ct = default);
        Task<DeliveryNoteReceiverDto?> GetByIdAsync(ulong id, CancellationToken ct = default);
        Task<Result<DeliveryNoteReceiverDto>> CreateAsync(DeliveryNoteReceiverRequest request, CancellationToken ct = default);
        Task<Result<DeliveryNoteReceiverDto>> UpdateAsync(ulong id, DeliveryNoteReceiverRequest request, CancellationToken ct = default);
        Task<Result> DeleteAsync(ulong id, CancellationToken ct = default);
    }
}
