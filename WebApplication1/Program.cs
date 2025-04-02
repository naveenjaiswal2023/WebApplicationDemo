using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models; // ✅ Add this for Swagger
using System.Text;
using WebApplication1.Filters;

var builder = WebApplication.CreateBuilder(args);

// ✅ Load JWT settings
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"];

if (string.IsNullOrWhiteSpace(secretKey) || Encoding.UTF8.GetBytes(secretKey).Length < 16)
{
    throw new InvalidOperationException("JWT Secret Key must be at least 16 bytes long.");
}

var key = Encoding.UTF8.GetBytes(secretKey);

// ✅ Configure Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        ClockSkew = TimeSpan.Zero
    };
});

// ✅ Configure Authorization
builder.Services.AddAuthorization();

// ✅ Register Filters
builder.Services.AddHttpContextAccessor(); // Required for IHttpContextAccessor
builder.Services.AddScoped<RequestResponseLoggingFilter>();
builder.Services.AddScoped<GlobalExceptionFilter>();
builder.Services.AddScoped<ResponseCacheFilter>();
builder.Services.AddScoped<ResponseFormattingFilter>();

// ✅ Register MemoryCache
builder.Services.AddMemoryCache();

// ✅ Register Swagger (Fix for the issue)
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

    // 🔑 Enable JWT authentication in Swagger UI
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Enter 'Bearer {token}'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            new string[] { }
        }
    });
});

// ✅ Register Controllers & Add Filters Globally
builder.Services.AddControllers(options =>
{
    options.Filters.Add<RequestResponseLoggingFilter>();
    options.Filters.Add<GlobalExceptionFilter>();
    options.Filters.Add<ResponseFormattingFilter>();
});

// ✅ Build App
var app = builder.Build();

// ✅ Middleware Configuration
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); // 🔥 Fix: Ensure Swagger is registered
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/error");
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
