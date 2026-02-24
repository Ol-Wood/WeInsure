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

builder.Services.AddDatabase(builder.Configuration.GetConnectionString("Database"));


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapControllers();

app.UseHttpsRedirection();
app.Run();
