using System.Text;
using Authentication_With_JWT.Helper;
using Authentication_With_JWT.Setting;
using Azure.AI.OpenAI;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using MultiTenancy.ConfigureServices;
using MultiTenancy.Services.OrderService;
using MultiTenancy.Services.paymobServices;
using MultiTenancy.Services.TrafficServices;
using X.Paymob.CashIn;

var builder = WebApplication.CreateBuilder(args);

// admin handel
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));
});

// Add services to the container.
var configuration = builder.Configuration;
builder.Services.Configure<JWT>(configuration.GetSection("JWT"));
builder.Services.Configure<MailSettings>(configuration.GetSection("MailSettings"));


// add tenant configuration
builder.Services.AddTenancy(builder.Configuration);
builder.Services.AddIdentity<AppUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICategoriesServices, CategoriesServices>();
builder.Services.AddScoped<IBrandServices, BrandServices>();
builder.Services.AddScoped<IWishListServices, WishListServices>();
builder.Services.AddScoped<IAddressServices, AddressServices>();
builder.Services.AddScoped<ICartServices, CartServices>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ISendMail, SendMail>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<ITrafficServices, TrafficServices>();
builder.Services.AddHttpClient<IPaymentService, PaymentService>();

builder.Services.AddHttpContextAccessor();



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
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
    });



builder.Services.AddControllers();


// Register OpenAI client
var openAiConfig = builder.Configuration.GetSection("OpenAI");
builder.Services.AddSingleton(sp =>
    new OpenAIClient(
        new Uri(openAiConfig["Endpoint"]),
        new Azure.AzureKeyCredential(openAiConfig["ApiKey"])
    )
);

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseCors(builder =>
    builder.AllowAnyOrigin()
           .AllowAnyMethod()
           .AllowAnyHeader()
);
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ProductCoverImages")),
    RequestPath = "/ProductCoverImages"
});

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "BrandImages")),
    RequestPath = "/BrandImages"
});
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "CategoryImages")),
    RequestPath = "/CategoryImages"
});

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ProductImages")),
    RequestPath = "/ProductImages"
});

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
