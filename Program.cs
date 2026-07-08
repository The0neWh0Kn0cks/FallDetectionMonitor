using FallDetectionMonitor.Components;
using FallDetectionMonitor.Data;
using Microsoft.EntityFrameworkCore;
using FallDetectionMonitor.Services;

var builder = WebApplication.CreateBuilder(args);

// Database factory for Blazor Server components
builder.Services.AddDbContextFactory<FallDetectionDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("FallDetectionConnection")));
builder.Services.AddHostedService<MqttFallAlertService>();
builder.Services.AddScoped<MqttCommandPublisher>();

// Add services to the container
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);

app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();