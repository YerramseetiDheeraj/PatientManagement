using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using PatientManagement.Data;
using PatientManagement.Mapping;
using PatientManagement.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add DbContext
builder.Services.AddDbContext<PatientContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("PatientManagementDB")));

// Add repository
builder.Services.AddTransient<IPatientRepository, PatientRepository>();

// Add controllers
builder.Services.AddControllers().AddNewtonsoftJson();

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "PatientManagement", Version = "v1" });
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyHeader()
               .AllowAnyMethod();
    });
});

builder.Services.AddAutoMapper(typeof(PatientProfile).Assembly);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
