# RFactory MES Backend — Database First Architecture

## Tổng quan kiến trúc

```
RFactory.slnx
├── RFactory.Shared           — ApiResponse, Result, PagedResult, BusinessException, IUser, JwtOptions, Constants
├── RFactory.Infrastructure   — Entities (scaffold), DbContext, Configurations, Persistence (Repos + Interfaces), Dapper, Interceptors
├── RFactory.Application      — Modules/{MasterData,Equipment,...}/{Services,DTOs,Mappings}
├── RFactory.BackgroundService — OEE workers, AlertEvaluator, hosted services
└── RFactory.API              — Controllers (gom theo module), Middleware, DI, Program.cs
```

### Sơ đồ dependency (một chiều, không vòng lặp)

```
Shared
  ↑
Infrastructure → Shared
  ↑
Application   → Infrastructure + Shared
  ↑                ↑
API           → Application + Infrastructure + Shared
BackgroundService → Application + Infrastructure + Shared
```

> **Điểm lệch có chủ đích**: Repository interfaces (`IRepository<T>`, `I*Repository`, `IProcedureExecutor`)
> nằm trong **Infrastructure** (cạnh entity và implementation), không ở Application. Lý do: nếu interface
> ở Application thì Application cần reference Infrastructure để dùng entity → circular reference. Đặt xuống
> Infrastructure giữ dependency graph một chiều sạch.

---

## Yêu cầu

- .NET 8 SDK
- MySQL 8.x (database `rtc_factory` đang chạy khi scaffold)

---

## Cấu hình ban đầu

1. Copy `RFactory.API/appsettings.json` → `appsettings.Development.json` (git-ignored).
2. Điền connection string và JWT secret:

```json
{
  "ConnectionStrings": {
    "RFactoryConnection": "Server=localhost;Port=3306;Database=rtc_factory;User=root;Password=YOUR_PASSWORD;"
  },
  "Jwt": {
    "SecretKey": "AT_LEAST_32_CHARACTER_SECRET_KEY_HERE",
    "Issuer": "RFactory",
    "Audience": "RFactory",
    "ExpireMinutes": 480
  }
}
```

---

## Workflow Database First (EF Core Scaffold)

> Thực hiện khi schema DB thay đổi. **Không dùng migrations** — DB là source of truth.

```bash
# Từ thư mục gốc solution
dotnet ef dbcontext scaffold \
  "Server=localhost;Port=3306;Database=rtc_factory;User=root;Password=YOUR_PASSWORD;" \
  Pomelo.EntityFrameworkCore.MySql \
  --project RFactory.Infrastructure \
  --startup-project RFactory.API \
  --context RFactoryDbContext \
  --context-dir Data \
  --output-dir Entities \
  --no-onconfiguring \
  --use-database-names \
  --force
```

Sau khi scaffold:
- File sinh ra trong `Entities/` là `partial class` — **không sửa trực tiếp**.
- Mọi thêm interface (`IAuditableEntity`, `ISoftDelete`) hoặc computed property đặt trong `Entities/FactoryPartial.cs` (hoặc thêm file `<EntityName>Partial.cs` mới).
- Cấu hình Fluent API thêm vào `Data/Configurations/` (kế thừa `IEntityTypeConfiguration<T>`).

---

## Cấu trúc chi tiết

### RFactory.Shared
```
Abstractions/   IUser, IAuditableEntity, ISoftDelete
Api/            ApiResponse<T>, ApiResponseFactory
Constants/      AppConstants (CorsPolicy, Paging)
Exceptions/     BusinessException, NotFoundException
Results/        Result, Result<T>, PagedResult<T>
Security/       JwtOptions
```

### RFactory.Infrastructure
```
Entities/                   — File scaffold (partial class), đừng sửa tay
Entities/<Name>Partial.cs   — Extend entity: implement IAuditableEntity / ISoftDelete
Data/
  RFactoryDbContext.cs       — partial, auto-apply soft-delete query filter
  Configurations/            — IEntityTypeConfiguration<T> cho từng entity
  Interceptors/
    AuditableEntityInterceptor.cs — Auto-fill CreatedBy/Date, UpdatedBy/Date + soft-delete
Persistence/
  IRepository<T>             — Generic CRUD contract
  GenericRepository<T>       — EF Core implementation (no-tracking reads)
  I<Name>Repository          — Entity-specific interface
  <Name>Repository           — Entity-specific implementation
Dapper/
  IProcedureExecutor         — Stored procedure / raw SQL contract
  ProcedureExecutor          — MySqlConnector + Dapper implementation
Extensions/
  DependencyInjection.cs     — AddInfrastructure(IConfiguration)
```

### RFactory.Application
```
Modules/
  MasterData/
    DTOs/       FactoryDto, CreateFactoryRequest, UpdateFactoryRequest
    Mappings/   MasterDataProfile (AutoMapper)
    Services/   IFactoryService, FactoryService
  Equipment/    (placeholder)
  Process/      (placeholder)
  Product/      (placeholder)
  Planning/     (placeholder)
  Production/   (placeholder)
  Quality/      (placeholder)
  Maintenance/  (placeholder — map nhóm Downtime tạm thời)
  OEE/          (placeholder)
  Report/       (placeholder)
Extensions/
  DependencyInjection.cs     — AddApplication()
```

Khi thêm module mới:
1. Tạo folder `Modules/<Module>/{Services,DTOs,Mappings}`.
2. Tạo DTO, Profile (AutoMapper tự scan assembly), Service + Interface.
3. Đăng ký service trong `Extensions/DependencyInjection.cs`.

### RFactory.BackgroundService
```
Workers/
  SampleWorker.cs   — Mẫu hosted service; thay bằng OEE workers thực tế
Extensions/
  DependencyInjection.cs — AddBackgroundServices()
Program.cs
```

### RFactory.API
```
Controllers/
  MasterData/
    FactoryController.cs   — CRUD mẫu, response envelope ApiResponse<T>
Middleware/
  GlobalExceptionMiddleware.cs — Map BusinessException → 400, NotFoundException → 404
Infrastructure/
  CurrentUser.cs           — IUser implementation (đọc từ JWT claims)
Extensions/
  DependencyInjection.cs   — AddApi(): JWT auth, Swagger, CORS, CurrentUser
Program.cs
appsettings.json
```

---

## Chạy API

```bash
cd RFactory.API
dotnet run
# Swagger UI: https://localhost:{port}/swagger
```

---

## Build toàn solution

```bash
dotnet build RFactory.slnx
```

---

## Thêm entity/module mới (ví dụ: Machine)

1. **Scaffold** lại (hoặc tạo tay entity `partial class Machine` trong `Entities/`).
2. Thêm `MachinePartial.cs` trong `Entities/` → implement `IAuditableEntity`, `ISoftDelete`.
3. Thêm `MachineConfiguration.cs` trong `Data/Configurations/`.
4. Thêm `DbSet<Machine>` vào `RFactoryDbContext` (hoặc để scaffold tự sinh).
5. Tạo `IMachineRepository` + `MachineRepository` trong `Persistence/`.
6. Đăng ký repository trong `Infrastructure/Extensions/DependencyInjection.cs`.
7. Tạo module trong `Application/Modules/Equipment/` (hoặc module phù hợp).
8. Thêm Controller trong `API/Controllers/`.

---

## Ghi chú bảo mật

- `appsettings.Development.json` phải được thêm vào `.gitignore` — chứa connection string và JWT secret.
- AutoMapper 14.0.0 (last free version) — warning CVE-2026-32933 đã được suppress trong csproj vì giải pháp trả phí (v15+). Vulnerability chỉ xảy ra với self-referential object graph sâu hơn ~25.000 cấp (không thực tế trong ngữ cảnh MES này).

