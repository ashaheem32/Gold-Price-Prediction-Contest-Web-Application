using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

var conn = builder.Configuration.GetConnectionString("DefaultConnection")
           ?? throw new Exception("Connection string not found");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySQL(conn));

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();

// Background service: auto-expire winners after 7 days
builder.Services.AddHostedService<WinnerExpiryService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        context.Database.EnsureCreated();

        // Add WonAt column if it doesn't exist (safe migration)
        try
        {
            await context.Database.ExecuteSqlRawAsync(
                "ALTER TABLE ContestEntries ADD COLUMN WonAt datetime(6) NULL");
        }
        catch { /* Column already exists */ }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred creating the DB.");
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();
app.UseSession();
app.UseRouting();
app.MapRazorPages();
app.Run();

// ============================================
// Background service to expire winners after 7 days
// ============================================
public class WinnerExpiryService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<WinnerExpiryService> _logger;

    public WinnerExpiryService(IServiceScopeFactory scopeFactory, ILogger<WinnerExpiryService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Small delay to let app start up
        await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var expiredWinners = await context.ContestEntries
                    .Where(e => e.IsWinner && e.WonAt != null && e.WonAt < DateTime.Now.AddDays(-7))
                    .ToListAsync(stoppingToken);

                if (expiredWinners.Any())
                {
                    foreach (var winner in expiredWinners)
                    {
                        winner.IsWinner = false;
                        winner.WonAt = null;
                        _logger.LogInformation("Auto-expired winner: {Name} (ID: {Id})", winner.AccountName, winner.Id);
                    }
                    await context.SaveChangesAsync(stoppingToken);
                }
            }
            catch (OperationCanceledException)
            {
                // Graceful shutdown
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in winner expiry service");
            }

            try
            {
                // Check every hour
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }
    }
}
