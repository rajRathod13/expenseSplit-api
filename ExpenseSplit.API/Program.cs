using ExpenseSplit.Infrastructure;
using ExpenseSplit.Application;
using ExpenseSplit.API;
using ExpenseSplit.Common.ConfigurationSettings;
using Microsoft.OpenApi.Models;
using ExpenseSplit.Application.ServiceInterfaces;
using ExpenseSplit.API.Services;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddInsfrastructureServices(builder.Configuration);
builder.Services.AddApplicationServices();

//RabbitMQ
builder.Services.AddScoped<IMessageService, MessageService>();

builder.Services.AddMassTransit(conf =>
{
    conf.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host("localhost", "/", h => 
        {
            h.Username("myuser");
            h.Password("mypass");
        });
    });
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

    // Add security definition for Bearer token
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' followed by your JWT token"
    });

    // Add security requirement to all API endpoints
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                new string[] {}
            }
        });
});
builder.Services.AddCors(config =>
{
    config.AddPolicy("AllowAngularApp", options =>
    {
        options.WithOrigins("http://localhost:4200");
        options.AllowAnyHeader();
        options.AllowAnyMethod();
        options.AllowCredentials();
    });
});
builder.Services.Configure<FileSettings>(options =>
{
    options.WebRootPath = builder.Environment.WebRootPath;
    options.BaseFolder = "documents";
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseGlobalExceptionMiddleware();
app.UseHttpsRedirection();

app.UseCors("AllowAngularApp");

app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
