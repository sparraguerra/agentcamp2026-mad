using System.Text;
using System.IO;
using Dapr.AI.Conversation;
using Dapr.AI.Conversation.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using DndCopilot.Api.Services;
using DndCopilot.Core.Interfaces;
using DndCopilot.Core.Services;
using DndCopilot.Infrastructure.Data;
using DndCopilot.Infrastructure.Repositories;
using DndCopilot.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "D&D Copilot API",
        Version = "v1",
        Description = "A turn-based D&D adventure game API"
    });
});

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

// Configure Database with a stable absolute path
var dbPath = Path.Combine(builder.Environment.ContentRootPath, "dndcopilot.db");
builder.Services.AddDbContext<GameDbContext>(options =>
    options.UseSqlite($"Data Source={dbPath}"));

// Configure JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"] ?? "YourSecretKeyForDevelopmentOnlyChangeInProduction123!";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"] ?? "DndCopilotApi",
        ValidAudience = jwtSettings["Audience"] ?? "DndCopilotClient",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };
});

builder.Services.AddAuthorization();

// Register services
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<DiceRoller>();
builder.Services.AddScoped<CombatService>();
builder.Services.AddScoped<GameService>();

// Register NPC Agent services with Dapr Conversation API by default.
// Fallback to mock only on connection failures.
var daprHttpPort = builder.Configuration["Dapr:HttpPort"] ?? "3500";
var daprComponentName = builder.Configuration["Dapr:ConversationComponent"] ?? "openai";

builder.Services.AddDaprConversationClient();
builder.Services.AddScoped<DaprConversationAiClient>(sp => new DaprConversationAiClient(
    sp.GetRequiredService<DaprConversationClient>(),
    sp.GetRequiredService<ILogger<DaprConversationAiClient>>(),
    daprComponentName));
builder.Services.AddScoped<MockFoundryAiClient>();
builder.Services.AddScoped<IFoundryAiClient>(sp => new FallbackFoundryAiClient(
    sp.GetRequiredService<DaprConversationAiClient>(),
    sp.GetRequiredService<MockFoundryAiClient>(),
    sp.GetRequiredService<ILogger<FallbackFoundryAiClient>>()));
builder.Services.AddHttpClient<IDaprStateClient, DaprStateClient>(
    (httpClient, sp) => new DaprStateClient(
        httpClient,
        sp.GetRequiredService<ILogger<DaprStateClient>>(),
        daprHttpPort));

builder.Services.AddScoped<IDaprEventPublisher, StubDaprEventPublisher>();
builder.Services.AddScoped<INpcAgentService, NpcAgentService>();

// Load NPC profiles from docs/NPCS.md
var npcsDocPath = Path.Combine(builder.Environment.ContentRootPath, "..", "..", "docs", "NPCS.md");
if (!File.Exists(npcsDocPath))
    npcsDocPath = Path.Combine(builder.Environment.ContentRootPath, "docs", "NPCS.md");
var npcRegistry = NpcProfileRegistry.LoadFromFile(npcsDocPath);
builder.Services.AddSingleton<INpcProfileRegistry>(npcRegistry);

// Load Action profiles from docs/ACTIONS.md
var actionsDocPath = Path.Combine(builder.Environment.ContentRootPath, "..", "..", "docs", "ACTIONS.md");
if (!File.Exists(actionsDocPath))
    actionsDocPath = Path.Combine(builder.Environment.ContentRootPath, "docs", "ACTIONS.md");
var actionRegistry = ActionProfileRegistry.LoadFromFile(actionsDocPath);
builder.Services.AddSingleton<IActionRegistry>(actionRegistry);
builder.Services.AddScoped<IActionService, ActionService>();

// Register repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ICharacterRepository, CharacterRepository>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Initialize database
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<GameDbContext>();

    if (app.Environment.IsDevelopment())
    {
        // Development: create schema if missing but do not wipe data, so user IDs remain stable
        dbContext.Database.EnsureCreated();
    }
    else
    {
        // Production: apply migrations (requires migrations to be created)
        dbContext.Database.Migrate();
    }

    DbInitializer.Initialize(dbContext);
}

app.Run();
