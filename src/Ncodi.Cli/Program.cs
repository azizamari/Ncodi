using McMaster.Extensions.CommandLineUtils;
using Ncodi.CodeAnalysis;
using Ncodi.CodeAnalysis.IO;
using Ncodi.CodeAnalysis.Symbols;
using Ncodi.CodeAnalysis.Syntax;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Ncodi.Cli
{
    class Program
    {
        static int Main(string[] args)
        {
            var app = new CommandLineApplication
            {
                Name = "ncodi",
                Description = "Command line interface to run ncodi coes",
            };
            app.HelpOption(inherited: true);

            app.Command("run", run =>
            {
                var pathArgument = run.Argument("path", "path of the file you want to execute").IsRequired();
                run.OnExecute(()=>
                {
                    var path = AddFileExtension(pathArgument.Value);
                    if (File.Exists(path))
                    {
                        Console.WriteLine($"Executing file '{path}'");
                        var syntaxTree = SyntaxTree.Load(path);
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
                    }
                    else
                    {
                        Console.WriteLine($"Error: file '{pathArgument.Value}' not found");
                    }
                    return 1;
                });
            });
            app.OnExecute(() =>
            {
                WriteLogo();
                Console.WriteLine();
                app.ShowHelp();
                return 1;
            });
            return app.Execute(args);
        }
        private static string AddFileExtension(string path)
        {
            if (path.EndsWith(".ncodi"))
            {
                return path;
            }
            return $"{path}.ncodi";
        }
        private static void WriteLogo()
        {
            Console.WriteLine(@"
 __    __                            __  __ 
|  \  |  \      By Aziz Amari       |  \|  \
| $$\ | $$  _______   ______    ____| $$ \$$
| $$$\| $$ /       \ /      \  /      $$|  \
| $$$$\ $$|  $$$$$$$|  $$$$$$\|  $$$$$$$| $$
| $$\$$ $$| $$      | $$  | $$| $$  | $$| $$
| $$ \$$$$| $$_____ | $$__/ $$| $$__| $$| $$
| $$  \$$$ \$$     \ \$$    $$ \$$    $$| $$
 \$$   \$$  \$$$$$$$  \$$$$$$   \$$$$$$$ \$$ 
            ");
        }
    }
}
