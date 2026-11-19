using AtmSystem.Application;
using AtmSystem.Core.Sessions;
using AtmSystem.Infrastructure;
using AtmSystem.WebApi.Options;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.Configure<SystemPasswordOptions>(
    builder.Configuration.GetSection("SystemPassword"));

builder.Services.AddApplication();

builder.Services.AddInfrastructure();

builder.Services.AddSingleton<ISessionFactory, SessionFactory>();

WebApplication app = builder.Build();

app.MapControllers();

app.Run();