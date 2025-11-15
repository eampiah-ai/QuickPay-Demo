using api.Domain;
using api.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// entity framework in memory db
builder.Services.AddDbContext<CustomerDb>(options =>
    options.UseInMemoryDatabase("CustomerDb"));
builder.Services.AddDbContext<InvoiceDb>(options =>
    options.UseInMemoryDatabase("InvoiceDb"));
builder.Services.AddDbContext<PaymentDb>(options =>
    options.UseInMemoryDatabase("PaymentDb"));

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
    System.Console.WriteLine("Sample data generation complete.");
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();