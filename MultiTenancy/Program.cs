using System.Text;
using Authentication_With_JWT.Helper;
using Authentication_With_JWT.Setting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MultiTenancy.ConfigureServices;
using MultiTenancy.Services.BrandServices;
using MultiTenancy.Services.CategoriesServices;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var configuration = builder.Configuration;
builder.Services.Configure<JWT>(configuration.GetSection("JWT"));
builder.Services.Configure<MailSettings>(configuration.GetSection("MailSettings"));
builder.Services.AddIdentity<AppUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICategoriesServices, CategoriesServices>();
builder.Services.AddScoped<IBrandServices,BrandServices>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ISendMail, SendMail>();
builder.Services.AddHttpContextAccessor();

// add tenant configuration
builder.Services.AddTenancy(builder.Configuration);

// add jwt configuration
builder.Services.AddAuthentication(op =>
{
    op.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    op.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(op =>
    {
        op.RequireHttpsMetadata = false;
        op.SaveToken = true;
        op.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configuration.GetSection("JWT:Key").Value)),
            ValidateIssuer = true,
            ValidIssuer = configuration.GetSection("JWT:Issuer").Value,
            ValidateAudience = true,
            ValidAudience = configuration.GetSection("JWT:Audience").Value,
            ValidateLifetime = true
        };
    });



builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
