



using MultiTenancy.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<ITenantService, TenantService>();
builder.Services.AddScoped<IProductService, ProductService>();


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

TenantSettings tenantSettings = new ();
builder.Configuration.GetSection(nameof(TenantSettings)).Bind(tenantSettings);

var defaultDbProvider = tenantSettings.Defaults.DBProvider;

if (defaultDbProvider.ToLower() == "mssql")
{
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
    {
        options.UseSqlServer();
    });
}

foreach (var tenant in tenantSettings.Tenants)
{
    var connectionString = tenant.ConnectionString ?? tenantSettings.Defaults.ConnectionString;
    using var scope = builder.Services.BuildServiceProvider().CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    dbContext.Database.SetConnectionString(connectionString);

    if (dbContext.Database.GetPendingMigrations().Any())
    {
        dbContext.Database.Migrate();
    }
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
