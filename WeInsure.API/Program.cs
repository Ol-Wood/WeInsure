using System.Text.Json.Serialization;
using FluentValidation;
using WeInsure.Application.Policy.Commands;
using WeInsure.Application.Policy.UseCases;
using WeInsure.Application.Policy.UseCases.Interfaces;
using WeInsure.Application.Policy.Validators;
using WeInsure.Application.Services;
using WeInsure.Data;
using WeInsure.Data.Repositories;
using WeInsure.Domain.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddSingleton<IValidator<SellPolicyCommand>, SellPolicyCommandValidator>();
builder.Services.AddScoped<IGetPolicyUseCase, GetPolicyUseCase>();
builder.Services.AddScoped<ISellPolicyUseCase, SellPolicyUseCase>();
builder.Services.AddScoped<IPolicyReferenceGenerator, PolicyReferenceGenerator>();

builder.Services.AddSingleton<IIdGenerator, IdGenerator>();

builder.Services.AddScoped<IPolicyRepository, PolicyRepository>();

var dbConnectionString = builder.Configuration.GetConnectionString("WeInsure");

if (builder.Environment.IsDevelopment() && string.IsNullOrEmpty(dbConnectionString))
{
    const Environment.SpecialFolder folder = Environment.SpecialFolder.LocalApplicationData;
    dbConnectionString = $"Data Source={Path.Join(Environment.GetFolderPath(folder), "weinsure.db")}";
}

builder.Services.AddDatabase(dbConnectionString);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapControllers();

app.UseHttpsRedirection();
app.Run();
