using GITA.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;


public class TopicInactiveService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<TopicInactiveService> _logger;
    //private readonly TimeSpan _checkInterval = TimeSpan.FromHours(24);  // Set your desired interval
    //private readonly int _daysUntilInactive = 30;  // Number of days after which a topic becomes inactive
    private readonly TimeSpan _checkInterval = TimeSpan.FromSeconds(20);
    private readonly double _daysUntilInactive = 0.001;

    public TopicInactiveService(IServiceScopeFactory scopeFactory, ILogger<TopicInactiveService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await MarkInactiveTopicsAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while marking topics as inactive.");
            }

            await Task.Delay(_checkInterval, stoppingToken);
        }
    }

    private async Task MarkInactiveTopicsAsync(CancellationToken stoppingToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var now = DateTime.UtcNow;
        var topics = await context.Topics
            .Include(t => t.Comments)
            .Where(t => t.Status == "active")
            .ToListAsync(stoppingToken);

        foreach (var topic in topics)
        {
            var lastCommentDate = topic.Comments.Any() ? topic.Comments.Max(c => c.CreatedDate) : topic.CreatedDate;
            if ((now - lastCommentDate).TotalDays > _daysUntilInactive)
            {
                topic.Status = "inactive";
                Console.WriteLine("Topic become inactive");
            }
        }

        await context.SaveChangesAsync(stoppingToken);
    }
}
