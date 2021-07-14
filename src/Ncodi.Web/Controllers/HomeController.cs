using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ncodi.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly string filePath;

        public HomeController(string filePath)
        {
            this.filePath = filePath;
        }
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        [Route("/docs")]
        public IActionResult Docs()
        {
            return View("docs");
        }
        [HttpGet]
        [Route("/samples")]
        public IActionResult Samples()
        {
            return View("samples");
        }

        [HttpGet]
        [Route("/download")]
        public FileContentResult Installer()
        {
            return File(System.IO.File.ReadAllBytes(filePath), "application/octet-stream", "ncodi.exe");
        }
    }
}
