using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DiamlerCodeChallenge.Models;
using DiamlerCodeChallenge.Services;
using Microsoft.AspNetCore.Hosting;

namespace DiamlerTL.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHostingEnvironment _env;
        IConvertService _converter;

        public HomeController(IHostingEnvironment env, IConvertService converter)
        {
            _env = env;
            _converter = converter;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [Route("{tempName}")]
        public ActionResult Get(string tempName, [FromQuery] int id = 0)
        {
            string htmlContent = GetHtmlTemplateContent(tempName);
            if (htmlContent == null)
                return NotFound();

            var conString = _converter.GetConvertedTemplte(htmlContent, id);
            return Content(conString, "text/html");
        }

        private string GetHtmlTemplateContent(string fileName)
        {
            try
            {
                string contentRootPath = _env.ContentRootPath;
                return System.IO.File.ReadAllText($"{contentRootPath}\\Views\\Templates\\{fileName}");
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
