// ProductApi/Program.cs
using Microsoft.EntityFrameworkCore;
using ProductApi.Data; 
using ProductApi.Services; 

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();


builder.Services.AddDbContext<ProductDbContext>(options =>
    options.UseInMemoryDatabase("ProductDb")); 


builder.Services.AddScoped<IProductService, ProductService>();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Product API", Version = "v1" });
}); 



var app = builder.Build(); 


if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); 
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Product API V1"); 
        c.RoutePrefix = "swagger"; 
    });
}




app.UseAuthorization();
app.MapControllers();
app.Run();