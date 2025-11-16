using System.Text;
using api;
using api.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// setup cors
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins("http://localhost:5173")
                                .AllowAnyHeader()
                                .AllowAnyMethod();
                      });
});


// entity framework in memory db
builder.Services.AddDbContext<CustomerDb>(options =>
    options.UseInMemoryDatabase("CustomerDb"));
builder.Services.AddDbContext<InvoiceDb>(options =>
    options.UseInMemoryDatabase("InvoiceDb"));
builder.Services.AddDbContext<PaymentDb>(options =>
    options.UseInMemoryDatabase("PaymentDb"));

// configure config
builder.Services.Configure<JwtConfig>(builder.Configuration.GetSection("Jwt"));

// add JWT authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    string? issuer = builder.Configuration["Jwt:Issuer"];
    string? audience = builder.Configuration["Jwt:Audience"];
    string? key = builder.Configuration["Jwt:Key"];
    if (string.IsNullOrEmpty(issuer) || string.IsNullOrEmpty(audience) || string.IsNullOrEmpty(key))
    {
        throw new Exception("Invalid JWT properties");
    }

    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = issuer,
        ValidAudience = audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
    };
});

// Register Datagen service
builder.Services.AddScoped<Datagen>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    // Generate sample data on startup
    using var scope = app.Services.CreateScope();

    System.Console.WriteLine("Generating sample data...");
    var datagen = scope.ServiceProvider.GetRequiredService<Datagen>();
    datagen.GenerateCustomers();
    datagen.GenerateInvoices();
    System.Console.WriteLine("Sample data generation complete.");
}
app.UseCors(MyAllowSpecificOrigins);
app.UseHttpsRedirection();
app.MapControllers();
app.Run();