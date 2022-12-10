using MyDisk.Api;
using MyDisk.Api.App_Start;
using MyDisk.Conf;
using MyDisk.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddApiServices();
builder.Services.AddApplicationMvc(builder.Configuration);
builder.Services.AddApplicationServices();
builder.Services.AddServicesLayout();
builder.Services.AddInfrastructureServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseCors(builder =>
    {
        builder.AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader();
    }
);

app.MapControllers();

app.Run();
