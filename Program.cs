// ProductApi/Program.cs
using Microsoft.EntityFrameworkCore;
using ProductApi.Data;
using ProductApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer; 
using Microsoft.IdentityModel.Tokens;            
using System.Text;                                
using Microsoft.OpenApi.Models;                 

var builder = WebApplication.CreateBuilder(args);

// --- Đăng ký Services ---
builder.Services.AddControllers();
builder.Services.AddDbContext<ProductDbContext>(options =>
    options.UseInMemoryDatabase("ProductDb"));
builder.Services.AddScoped<IProductService, ProductService>();

// *** Bắt đầu cấu hình Authentication và Authorization ***
builder.Services.AddAuthentication(options =>
{
    // Đặt scheme mặc định là JWT Bearer
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options => // Cấu hình JWT Bearer
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true, // Kiểm tra Issuer
        ValidateAudience = true, // Kiểm tra Audience
        ValidateLifetime = true, // Kiểm tra thời hạn token
        ValidateIssuerSigningKey = true, // Quan trọng: Kiểm tra chữ ký token

        ValidIssuer = builder.Configuration["Jwt:Issuer"],      // Lấy từ appsettings.json
        ValidAudience = builder.Configuration["Jwt:Audience"],  // Lấy từ appsettings.json
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? // Lấy từ appsettings.json
               throw new InvalidOperationException("JWT Key is not configured."))) // !! Ném lỗi nếu Key bị thiếu !!
    };
});

// Đăng ký Authorization services (cần thiết ngay cả khi chỉ dùng [Authorize])
builder.Services.AddAuthorization();

// *** Kết thúc cấu hình Authentication và Authorization ***


// --- Cấu hình Swagger ---
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Product API", Version = "v1" });

    // *** Thêm cấu hình Security Definition cho Swagger UI ***
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header using the Bearer scheme. <br/>
                      Enter 'Bearer' [space] and then your token in the text input below. <br/>
                      Example: 'Bearer 12345abcdef'",
        Name = "Authorization", // Tên header
        In = ParameterLocation.Header, // Vị trí header
        Type = SecuritySchemeType.ApiKey, // Kiểu là API Key (dùng cho Bearer)
        Scheme = "Bearer" // Scheme là Bearer
    });

    // *** Thêm yêu cầu Security Requirement cho Swagger UI ***
    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer" // Phải khớp với tên trong AddSecurityDefinition
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>() // Danh sách các scope yêu cầu (để trống nếu không dùng scope)
        }
    });
});


// --- Xây dựng App ---
var app = builder.Build();

// --- Cấu hình Pipeline ---
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Product API V1");
        c.RoutePrefix = "swagger";
    });
}

// app.UseHttpsRedirection(); // Bỏ comment nếu muốn dùng HTTPS

// *** Thêm Authentication và Authorization middleware ***
// Quan trọng: UseAuthentication() PHẢI đứng trước UseAuthorization()
app.UseAuthentication(); // Xác thực request dựa trên token (nếu có)
app.UseAuthorization(); // Kiểm tra quyền truy cập vào endpoint có [Authorize]

app.MapControllers();
app.Run();