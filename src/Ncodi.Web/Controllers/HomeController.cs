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
    }
}
