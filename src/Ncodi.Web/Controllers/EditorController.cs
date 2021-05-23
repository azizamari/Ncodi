using Microsoft.AspNetCore.Mvc;
using Ncodi.CodeAnalysis;
using Ncodi.CodeAnalysis.IO;
using Ncodi.CodeAnalysis.Symbols;
using Ncodi.CodeAnalysis.Syntax;
using Ncodi.CodeAnalysis.Text;
using Ncodi.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ncodi.Web.Controllers
{
    [Route("api")]
    [ApiController]
    public class EditorController: ControllerBase
    {
        [HttpPost("{controller}/run")]
        public IActionResult Run(CodeDto code)
        {
            var srouce = SourceText.From(code.Code, "file.ncodi");
            var syntaxTree = SyntaxTree.Parse(srouce);
            var compilation = new Compilation(syntaxTree);
            var result = compilation.Evaluate(new Dictionary<VariableSymbol, object>());
            if (!result.Diagnostics.Any())
            {
                if (result.Value != null)
                    Console.Out.WriteLine(result.Value);
            }
            else
            {
                Console.WriteLine("Error List:");
                Console.Error.WriteDiagnostics(result.Diagnostics);
            }
            return Ok();
        }
    }
}
