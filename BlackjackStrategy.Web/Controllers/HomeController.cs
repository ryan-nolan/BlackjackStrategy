﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using BlackjackStrategy.Web.Models;
using BlackjackLogic;
using BlackjackLogic.Game;

namespace BlackjackStrategy.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(SimulatorGameOptions options)
        {
            if (options.StrategyName == null || options.StrategyName == string.Empty || string.IsNullOrWhiteSpace(options.StrategyName))
            {
                options.StrategyName = "basicstrategy";
            }
            return View("Simulation", options);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}