using Confluent.Kafka;
using Transaction_Service.Models;

var builder = WebApplication.CreateBuilder(args);

var producerConfig = new ProducerConfig
{
    BootstrapServers = builder.Configuration["Kafka:BootstrapServers"]
};
builder.Services.AddProducerConfig(producerConfig);
var _serviceDbContextCon = builder.Configuration.GetConnectionString("ServiceDbContext");
if (_serviceDbContextCon != null)
{
    builder.Services.AddServiceDbContext(_serviceDbContextCon);
}

builder.Services.Configure<ConnectionStringsModel>(builder.Configuration.GetSection("ConnectionStrings"));

builder.Services.AddCustomServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Transaction Service API v1"));
//}

app.UseHttpsRedirection();

app.UseAuthorization();


app.MapControllers();

app.Run();
