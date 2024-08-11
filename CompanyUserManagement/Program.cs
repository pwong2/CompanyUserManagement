using CompanyUserManagement.Middlewares;
using CompanyUserManagement.Repositories;
using CompanyUserManagement.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddSingleton<IUserRepository, UserRepository>();
builder.Services.AddSingleton<IAuthService, AuthService>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

// Configure JWT authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Secret"]))
        };
    });

// Add authorization services
builder.Services.AddAuthorization();

// Add Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(); 

// Add controllers
builder.Services.AddControllers();

var app = builder.Build();

// Configure middleware and endpoints
app.UseMiddleware<ThrottlingMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

// Configure Swagger UI
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "CompanyUserManagement v1");
        c.RoutePrefix = "swagger"; 
    });
}

app.MapControllers();

app.Run();
