using GestionCongeFrais.Data;
using Microsoft.EntityFrameworkCore;
using GestionCongeFrais.Services;
using GestionCongeFrais.Models;
using Microsoft.AspNetCore.Identity;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;   


var builder = WebApplication.CreateBuilder(args);

// 1-JWT config
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
            )
        };
    });
//2 Services généraux ajouté 

builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<IHolidayService, HolidayService>();
builder.Services.AddScoped<ILeaveTypeService, LeaveTypeService>();
builder.Services.AddScoped<IExpenseTypeService, ExpenseTypeService>();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ILeaveRequestService, LeaveRequestService>();
builder.Services.AddScoped<IExpenseRequestService, ExpenseRequestService>();

// 3 DbContext ajouté//
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = true;
    });

// Configuration CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowVueApp", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:3001", "http://localhost:3002", "http://localhost:3003", "http://localhost:3004", "http://localhost:8080") // Ports typiques pour Vue.js
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



var app = builder.Build();



//ajouté 

void CreateDefaultAdmin(IServiceProvider services)
{
    using var scope = services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<User>>();

    if (!context.Users.Any(u => u.Role == "Admin"))
    {
        var admin = new User
        {
            FullName = "Administrateur Système",
            Email = "admin@entreprise.com",
            Role = "Admin"
        };
        admin.PasswordHash = passwordHasher.HashPassword(admin, "Admin123!");
        context.Users.Add(admin);
        context.SaveChanges();
    }
}
// Après app.Build() mais avant app.Run():
CreateDefaultAdmin(app.Services);


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Utiliser CORS
app.UseCors("AllowVueApp");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Créer l'admin par défaut si nécessaire
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<User>>();

    // Créer l'admin par défaut si nécessaire
    if (!context.Users.Any(u => u.Role == "Admin"))
    {
        var admin = new User
        {
            FullName = "Administrateur Système",
            Email = "admin@entreprise.com",
            Role = "Admin",
            PasswordChanged = true // L'admin a déjà changé son mot de passe
        };
        admin.PasswordHash = passwordHasher.HashPassword(admin, "Admin123!");
        context.Users.Add(admin);
        context.SaveChanges();
    }
}

app.Run();
