using Microsoft.Extensions.Logging;

public class SiteStatisticsLogger
{
    private readonly ILogger<SiteStatisticsLogger> _logger;
    private int _visitCount = 0;

    public SiteStatisticsLogger(ILogger<SiteStatisticsLogger> logger)
    {
        _logger = logger;
    }

    public void LogVisit(string pageName)
    {
        _visitCount++;
        _logger.LogInformation("Page visited: {PageName}. Total visits: {VisitCount}", pageName, _visitCount);
    }

    public int GetVisitCount() => _visitCount;
}