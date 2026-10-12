using RFactory.Application.Modules.MasterData.DTOs;
using RFactory.Shared.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFactory.Application.Modules.MasterData.Services
{
    public interface ICountryService
    {
        Task<List<CountryDto>> GetAllAsync(CancellationToken ct = default);
        Task<CountryDto?> GetByIdAsync(ulong id, CancellationToken ct = default);
        Task<Result<CountryDto>> CreateAsync(CountryRequest request, CancellationToken ct = default);
        Task<Result<CountryDto>> UpdateAsync(ulong id, CountryRequest request, CancellationToken ct = default);
        Task<Result> DeleteAsync(ulong id, CancellationToken ct = default);
    }
    public interface IProvinceService
    {
        Task<List<ProvinceDto>> GetAllAsync(CancellationToken ct = default);
        Task<ProvinceDto?> GetByIdAsync(ulong id, CancellationToken ct = default);
        Task<Result<ProvinceDto>> CreateAsync(ProvinceRequest request, CancellationToken ct = default);
        Task<Result<ProvinceDto>> UpdateAsync(ulong id, ProvinceRequest request, CancellationToken ct = default);
        Task<Result> DeleteAsync(ulong id, CancellationToken ct = default);
    }
    public interface IDistrictService
    {
        Task<List<DistrictDto>> GetAllAsync(CancellationToken ct = default);
        Task<DistrictDto?> GetByIdAsync(ulong id, CancellationToken ct = default);
        Task<Result<DistrictDto>> CreateAsync(DistrictRequest request, CancellationToken ct = default);
        Task<Result<DistrictDto>> UpdateAsync(ulong id, DistrictRequest request, CancellationToken ct = default);
        Task<Result> DeleteAsync(ulong id, CancellationToken ct = default);
    }
    public interface IWardService
    {
        Task<List<WardDto>> GetAllAsync(CancellationToken ct = default);
        Task<WardDto?> GetByIdAsync(ulong id, CancellationToken ct = default);
        Task<Result<WardDto>> CreateAsync(WardRequest request, CancellationToken ct = default);
        Task<Result<WardDto>> UpdateAsync(ulong id, WardRequest request, CancellationToken ct = default);
        Task<Result> DeleteAsync(ulong id, CancellationToken ct = default);
    }
}
