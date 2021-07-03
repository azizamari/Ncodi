using Microsoft.AspNetCore.Mvc;
using Ncodi.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ncodi.Web.Controllers
{
    public class PlaygroundController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        [Route("OldPlayground")]
        public IActionResult OldPlaground()
        {
            return View("old");
        }
        [HttpPost("playground")]
        public IActionResult TryCodeBtn(TryCode coding)
        {
            return View("index", coding);
        }
    }
}
