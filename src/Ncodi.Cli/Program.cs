using McMaster.Extensions.CommandLineUtils;
using System;
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
                run.OnExecute(()=>
                {
                    Console.WriteLine("File running result here");
                    return 1;
                });
            });
            app.OnExecute(() =>
            {
                Console.WriteLine("azuz");
                app.ShowHelp();
                return 1;
            });
            return app.Execute(args);
        }
    }
}
