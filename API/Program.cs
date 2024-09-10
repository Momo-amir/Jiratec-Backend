using DAL.Data;
using Repository.Interfaces;
using Repository.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using API.Services; 

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 25)) // Sets MySQL version, if updated please update here!!
    ));

builder.Services.AddScoped<UserService>();
// Register the IUserService with its implementation UserService
builder.Services.AddScoped<IUserService, UserService>();

// Register the Interface Repositories with its implementation Repository
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped <ICommentRepository, CommentRepository>();
builder.Services.AddScoped<IActivityLogRepository, ActivityLogRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();


// Add CORS policy - TODO NARROW DOWN ORIGINS IF EVER IN PRODUCTION 
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", corsBuilder =>
    {
        corsBuilder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});
// referenceloophandling to ignore the loop reference
// Add controllers
builder.Services.AddControllers();

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "JiraTecAPI", Version = "v1" });
});

var app = builder.Build();

// Configure the HTTP request pipeline.

// Use Swagger middleware to generate the Swagger JSON endpoint
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); // Generates the Swagger JSON
    app.UseSwaggerUI(c => 
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "JiraTecApi-V1"); // Configures the Swagger UI
    });
}

// Enable CORS
app.UseCors("AllowAllOrigins");

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
