using System.Text;
using DomainServices;
using DonainModel;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Configuration;
using System.Text.Json;
using API.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;
using Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

    // Define the BearerAuth scheme
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

builder.Services
    .AddScoped<ICarRepository, CarRepository>()
    .AddScoped<IReservationRepository, ReservationRepository>()
    .AddScoped<IRequestRepository, RequestRepository>()
    .AddScoped<IUserRepository, UserRepository>()
    .AddScoped<DbSeeder>()
    .AddScoped<IEmailService, EmailService>();


// Entity
builder.Services.AddDbContext<AppDbContext>(options => {
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("Default"), 
        sqlServerOptionsAction: sqlOptions => {
            sqlOptions.EnableRetryOnFailure();
    });
});

// Identity
builder.Services.AddDbContext<SecurityDbContext>(options => {
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("Security"),
        sqlServerOptionsAction: sqlOptions => { sqlOptions.EnableRetryOnFailure(); }
    );
});



builder.Services.Configure<ApiBehaviorOptions>(options => {
    options.SuppressModelStateInvalidFilter = true;
});

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<SecurityDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options => {
    options.TokenValidationParameters = new TokenValidationParameters {
        RequireExpirationTime = true,
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateIssuerSigningKey = true,
        ValidateLifetime = true,
        IssuerSigningKey =
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration.GetSection("Jwt:Key").Value!)),
    };
    
    options.Events = new JwtBearerEvents
    {
        OnChallenge = context =>
        {
            context.HandleResponse();
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";
            
            var body = new 
            {
                statusCode = 401,
                message = "Unauthorized",
                data = new { },
            };
            
            var bodyText = JsonSerializer.Serialize(body);
            return context.Response.WriteAsync(bodyText);
        },
        OnForbidden = context =>
        {
        context.Response.StatusCode = StatusCodes.Status403Forbidden;
        context.Response.ContentType = "application/json";

        var body = new
        {
        statusCode = 403,
        message = "Forbidden",
        data = new { },
    };

    var bodyText = JsonSerializer.Serialize(body);
    return context.Response.WriteAsync(bodyText);
    }
    };
});

//Add roles
builder.Services.AddAuthorization(options => {
    options.AddPolicy("ADMIN", policy => policy.RequireClaim("role", "ADMIN"));
    options.AddPolicy("USER", policy => policy.RequireClaim("role", "USER"));
});

var app = builder.Build();
app.UseCors(builder => builder
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader()
);

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var roles = new[] { "ADMIN", "USER" };
 
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }
    
    var dbSeeder = scope.ServiceProvider.GetRequiredService<DbSeeder>();
    await dbSeeder.Seed();
}


// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
