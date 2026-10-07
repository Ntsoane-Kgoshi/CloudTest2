using CoffeeNChill.Functions.Interfaces;
using CoffeeNChill.Functions.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

builder.Services.AddApplicationInsightsTelemetryWorkerService().ConfigureFunctionsApplicationInsights();

builder.Services.AddSingleton<ITableStorageService, TableStorageService>();
builder.Services.AddSingleton<IFileStorageService, FileStorageService>();

builder.Services.AddSingleton<IOrderQueueService, OrderQueueService>();
builder.Services.AddSingleton<IOrderTableService, OrderTableService>();


builder.Build().Run();
