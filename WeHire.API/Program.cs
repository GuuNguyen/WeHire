using Microsoft.EntityFrameworkCore;
using WeHire.API.Configurations;
using WeHire.Application.Utilities.Middleware;
using WeHire.Domain.Entities;
using WeHire.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

var apiCorsPolicy = "ApiCorsPolicy";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: apiCorsPolicy,
        builder =>
        {
            builder.WithOrigins("http://localhost:3000")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
});

builder.Services.AddDbContext<WeHireDBContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("MyDB"));
});
builder.Services.RegisterJwtModule(builder.Configuration);
builder.Services.RegisterSwaggerModule();

builder.Services.InfrastructureRegister();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
app.UseSwagger();
app.UseSwaggerUI();
app.UseExceptionMiddleware();
app.UseApplicationSwagger();
app.UseApplicationJwt();
app.UseCors(apiCorsPolicy);
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
