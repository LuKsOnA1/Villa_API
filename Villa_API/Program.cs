using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Villa_API.AutoMapper;
using Villa_API.Data;
using Villa_API.Repository;
using Villa_API.Repository.IRepository;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Api Versioning ...
builder.Services.AddApiVersioning(opt =>
{
    opt.AssumeDefaultVersionWhenUnspecified = true;
    opt.DefaultApiVersion = new ApiVersion(1, 0);
    opt.ReportApiVersions = true;

}).AddApiExplorer(opt =>
{
    opt.GroupNameFormat = "'v'VVV";
    opt.SubstituteApiVersionInUrl = true;
});

// Adding caching ...
builder.Services.AddResponseCaching();

builder.Services.AddControllers(opt =>
{
    opt.CacheProfiles.Add("Default30", new CacheProfile
    {
        Duration = 30
    });
})
    .AddNewtonsoftJson()
    .AddXmlDataContractSerializerFormatters();

// Adding AutoMapper config ...
builder.Services.AddAutoMapper(typeof(MappingConfig));

// Adding Repository config ...
builder.Services.AddScoped<IVillaRepository,VillaRepository>();
builder.Services.AddScoped<IVillaNumberRepository,VillaNumberRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Adding Authentication ...

var key = builder.Configuration.GetValue<string>("ApiSettings:Secret");

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(x =>
    {
        x.RequireHttpsMetadata = false;
        x.SaveToken = true;
        x.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{
    // Configuring Swagger to use Bearer Token
    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = 
            "JWT Authorization header using the Bearer scheme. \r\n\r\n" + 
            "Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\n" +
            "Example: \"Bearer 12345abcdef\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Scheme = "Bearer"
    });

    opt.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });

    // Modifying Swagger document for v1 ...
    opt.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1.0",
        Title = "Villa",
        Description = "API to manage Villa",
        TermsOfService = new Uri("https://example.com/terms"),
        Contact = new OpenApiContact
        {
            Name = "Example",
            Url = new Uri("https://example.com/contact")
        },
        License = new OpenApiLicense
        {
            Name = "Example License",
            Url = new Uri("https://example.com/license")
        }
    });

    // Modifying Swagger document for v2 ...
    opt.SwaggerDoc("v2", new OpenApiInfo
    {
        Version = "v2.0",
        Title = "Villa V2",
        Description = "API to manage Villa",
        TermsOfService = new Uri("https://example.com/terms"),
        Contact = new OpenApiContact
        {
            Name = "Example",
            Url = new Uri("https://example.com/contact")
        },
        License = new OpenApiLicense
        {
            Name = "Example License",
            Url = new Uri("https://example.com/license")
        }
    });
});

builder.Services.AddDbContext<ApplicationDbContext>(option =>
{
    option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(opt =>
    {
        opt.SwaggerEndpoint("/swagger/v1/swagger.json", "Villa_V1");
        opt.SwaggerEndpoint("/swagger/v2/swagger.json", "Villa_V2");
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
