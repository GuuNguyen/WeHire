using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Builder.Extensions;
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
            builder.WithOrigins("https://localhost:3000", "http://localhost:3000", "https://frontend-hiring-dev.vercel.app")
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
FirebaseApp.Create(new AppOptions()
{
    Credential = GoogleCredential.FromFile("Configurations/wehire-firebase-adminsdk.json")
});
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
