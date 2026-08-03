using RFactory.API.Extensions;
using RFactory.API.Middleware;
using RFactory.Application.Extensions;
using RFactory.Infrastructure.Extensions;
using RFactory.Shared.Constants;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddInfrastructure(builder.Configuration)
    .AddApplication()
    .AddApi(builder.Configuration);

var app = builder.Build();

app.UseMiddleware<GlobalExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "RFactory MES API v1"));
}

app.UseHttpsRedirection();
app.UseCors(AppConstants.CorsPolicy);
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
