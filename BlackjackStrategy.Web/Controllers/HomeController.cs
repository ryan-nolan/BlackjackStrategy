using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using BlackjackStrategy.Web.Models;
using BlackjackLogic;

namespace BlackjackStrategy.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index() => View();


        [HttpPost]
        public IActionResult Index(SimulatorGameOptions options) //Try async Task<IActionResult> for longer simulations
        {
            if (string.IsNullOrEmpty(options.StrategyName) || string.IsNullOrWhiteSpace(options.StrategyName))
            {
                options.StrategyName = "basicstrategy";
            }

            if (options.HandsToBePlayed > 100 || options.HandsToBePlayed < 0)
            {
                options.HandsToBePlayed = 100;
            }

            options.FilePath = null;
            //SimulatorWeb sim = new SimulatorWeb(options);
            //return View("Simulation", sim.RunGameAsync().Result);
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
