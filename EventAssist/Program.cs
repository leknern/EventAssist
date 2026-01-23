using EventAssist;
using EventAssist.Authorization;
using EventAssist.Contexts;
using EventAssist.Hubs;
using EventAssist.Repositories;
using EventAssist.Repositories.Interfaces;
using EventAssist.Services;
using EventAssist.Services.Interfaces;
using Mapster;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DevConnection")));

builder.Services.AddSingleton<IAiModelConfigurationBuilder, AiModelConfigurationBuilder>();

builder.Services.AddScoped<IAiAgentService, AiAgentService>();
builder.Services.AddScoped<IAuthSecurityService, AuthSecurityService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddScoped<IStringContentFactory, StringContentFactory>();
builder.Services.AddScoped<ITokenProviderService, TokenProviderService>();
builder.Services.AddScoped<IAiFunctionExecutionService, AiFunctionExecutionService>();

builder.Services.AddScoped<IChatRepository, ChatRepository>();
builder.Services.AddScoped<IEventRepository, EventRepository>();
builder.Services.AddScoped<IMessageRepository, MessageRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddSignalR();

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Services.AddMapster();
MapsterInit.RegisterMappings();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JsonWebToken:Issuer"],
            ValidAudience = builder.Configuration["JsonWebToken:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JsonWebToken:SecretKey"]!))
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];

                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) &&
                    (path.StartsWithSegments("/chatHub") || path.StartsWithSegments("/eventHub")))
                {
                    context.Token = accessToken;
                }

                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireFullToken", policy =>
        policy.Requirements.Add(new RequireFullTokenRequirement()));
});

builder.Services.AddSingleton<IAuthorizationHandler, RequireFullTokenHandler>();

builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("DevFrontend",
        policy =>
        {
            policy.WithOrigins("http://localhost:9000")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
});

builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.Title = "EventAssist API";
        options.Theme = ScalarTheme.Default;
        options.ShowSidebar = true;
    });
}

app.UseCors("DevFrontend");

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.MapHub<ChatHub>("/chatHub");
app.MapHub<EventHub>("/eventHub");

app.Run();
