using RFactory.Application.Extensions;
using RFactory.BackgroundService.Extensions;
using RFactory.Infrastructure.Extensions;

var builder = Host.CreateApplicationBuilder(args);

builder.Services
    .AddInfrastructure(builder.Configuration)
    .AddApplication()
    .AddBackgroundServices();

var host = builder.Build();
host.Run();
