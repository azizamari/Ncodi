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
            var srouce = SourceText.From(String.Join(Environment.NewLine,code.Lines), "sasfile.ncodi");
            var syntaxTree = SyntaxTree.Parse(srouce);
            var compilation = new Compilation(syntaxTree);
            var result = compilation.Evaluate(new Dictionary<VariableSymbol, object>());
            if (!result.Diagnostics.Any())
            {
                if (result.Value != null)
                    return Ok(result.OutputLines);
                return Ok("Your code doesn't return any data");
            }
            else
            {
                var x = result.Diagnostics.ReturnDiagnostics();
                return Ok(result.Diagnostics.ReturnDiagnostics());
            }
        }
    }
}
