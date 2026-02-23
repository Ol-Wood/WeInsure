using WeInsure.Application.Policy.UseCases;
using WeInsure.Application.Policy.UseCases.Interfaces;
using WeInsure.Application.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddScoped<IGetPolicyUseCase, GetPolicyUseCase>();
builder.Services.AddScoped<ISellPolicyUseCase, SellPolicyUseCase>();
builder.Services.AddScoped<IPolicyReferenceGenerator, PolicyReferenceGenerator>();

builder.Services.AddSingleton<IIdGenerator, IdGenerator>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.Run();
