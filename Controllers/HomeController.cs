using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using projeler.Models;

namespace projeler.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly SiteStatisticsLogger _statsLogger;
    private readonly List<string> Page = new List<string>
    {
        "www.hangikredi.com",
        "www.hangikredikobi.com",
        "sipay.com.tr",
        "www.hesap.com",
        "www.getirfinans.com/"

    };

    public HomeController(ILogger<HomeController> logger, SiteStatisticsLogger statsLogger)
    {
        _logger = logger;
        _statsLogger = statsLogger;
    }

    public IActionResult Index()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    public async Task<IActionResult> CheckSiteStatus(string url)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        try
        {
            using var httpClient = new HttpClient();
            httpClient.Timeout = TimeSpan.FromSeconds(5);
            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/115.0.0.0 Safari/537.36"
            );
            var response = await httpClient.GetAsync("https://" + url);
            stopwatch.Stop();
            bool isSuccess = response.IsSuccessStatusCode;
            return Json(new
            {
                success = isSuccess,
                time = stopwatch.ElapsedMilliseconds
            });
        }
        catch
        {
            stopwatch.Stop();
            return Json(new
            {
                success = false,
                time = stopwatch.ElapsedMilliseconds
            });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetSteamGameInfo(string appId)
    {
        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("Cookie", "Steam_Language=english; birthtime=568022401; lastagecheckage=1-0-1990; currency=TRY");
        var apiUrl = $"https://store.steampowered.com/api/appdetails?appids={appId}";
        try
        {
            var response = await httpClient.GetAsync(apiUrl);
            var json = await response.Content.ReadAsStringAsync();
            return Content(json, "application/json");
        }
        catch
        {
            return Json(new { success = false });
        }
    }

    public IActionResult Steam()
    {
        return View();
    }

    public IActionResult WebsiteStatusMonitor()
    {
        ViewBag.Pages = Page;
        ViewBag.VisitCount = _statsLogger.GetVisitCount();
        return View();
    }
}
