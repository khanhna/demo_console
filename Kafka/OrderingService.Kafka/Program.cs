using Demo.Kafka.Contracts.Messages;
using DotNetCore.CAP;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

var configurationRoot = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.Development.json", optional: true)
    .Build();

// Add services to the container.
builder.Services.AddCap(x =>
{
    x.UseInMemoryStorage();
    
    x.UseKafka(options =>
    {
        options.Servers = "localhost:9093";
    });
    x.UseDashboard();
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/orders/{orderId}", async (Guid orderId, Guid customerId, int amount, [FromServices] ICapPublisher publisher) =>
    {
        // Simulate order processing
        var message = new OrderPlaced
        {
            OrderId = orderId,
            CustomerId = customerId,
            Amount = amount
        };
        await publisher.PublishAsync(OrderPlaced.MessageName, message);
        return Results.Ok($"Order {orderId} processed successfully.");
    })
    .WithName("PlaceOrder")
    .WithOpenApi();

app.Run();
