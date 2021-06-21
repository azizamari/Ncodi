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
using System.Net;
using System.Text;
using System.Threading;
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
            string[] output=new string[]{"Code hase no output"};
            var task = Task.Run(() =>
            {
                try
                {
                    var srouce = SourceText.From(String.Join(Environment.NewLine, code.Lines), "fileName.ncodi");
                    var syntaxTree = SyntaxTree.Parse(srouce);
                    var compilation = new Compilation(syntaxTree);
                    var result = compilation.Evaluate(new Dictionary<VariableSymbol, object>(), false);
                    while (compilation.needInput)
                    {
                        compilation.AddInput(Console.ReadLine());
                        result = compilation.Evaluate(new Dictionary<VariableSymbol, object>(), false);
                    }
                        if (!result.Diagnostics.Any())
                    {
                        if (result.OutputLines.Count()!=0)
                        {
                            output = result.OutputLines.ToArray();
                            return;
                        }
                        output = new string[] { "Your code doesn't return any data" };
                        return;
                    }
                    else
                    {
                        output= result.Diagnostics.ReturnDiagnostics().Split('\n');
                        return;
                    }
                }
                catch
                {
                    output = new string[] { "Can't execute this code because it causes an internal error, this is probably our mistake." };
                    return;
                }
            });

            bool isCompletedSuccessfully = task.Wait(TimeSpan.FromMilliseconds(1000));

            if (isCompletedSuccessfully)
            {
                return Ok(output);
            }
            else
            {
                return Ok(new string[] { "Time limit 1 second exceeded" });
            }
        }
    }
}
