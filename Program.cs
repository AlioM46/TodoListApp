using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Text;
using TodoListApi.Data;
using TodoListApi.Interfaces;
using TodoListApi.Middleware;
using TodoListApi.Options;
using TodoListApi.Services;

var builder = WebApplication.CreateBuilder(args);


// Edited
// Add services to the container.

builder.Services.AddDbContext<AppDbContext>((options) =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
});

// ------------------- Extract Appsettings.json to "jwtOptions" + DI the jwtOptions Class  -------------------- \\ 


// Inject Services

builder.Services.AddScoped<IUser, UserServices>();
builder.Services.AddScoped<IAuthentication, AuthenticationServices>();
builder.Services.AddScoped<ITask, TaskServices>();




var jwtOptions = builder.Configuration.GetSection("JWT").Get<JwtOptions>();
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("JWT"));

// ------------------- Add JWT Authentication Configuration -------------------- \\ 


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
  .AddJwtBearer(options =>
  {
      options.SaveToken = true;
      options.TokenValidationParameters = new TokenValidationParameters
      {
          ValidateIssuer = true,
          ValidateAudience = true,
          ValidateLifetime = true,
          ValidateIssuerSigningKey = true,
          ClockSkew = TimeSpan.Zero, // strict expiration
          ValidIssuer = jwtOptions.Issuer,
          ValidAudience = jwtOptions.Audience,
          IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SigningKey))
      };
  });





builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthentication(); // 🔐 JWT must be authenticated first

app.UseMiddleware<ForbidNonActiveMiddleware>(); // 👮 Your custom logic

app.UseAuthorization(); // 🔒 Checks roles/policies/claims

app.MapControllers();

app.Run();
