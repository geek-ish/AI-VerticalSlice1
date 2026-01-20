using System;
using System.Linq;
using CsvUpdateDemo.Infrastructure.IoC;
using SimpleInjector.Lifestyles;

namespace CsvUpdateDemo
{
    internal class Program
    {
        private static int Main(string[] args)
        {
            // Run smoke tests on startup (no DB required).
            RunSmokeTests();

            // Existing DI bootstrap
            var container = DependencyConfig.BuildContainer();
            using (AsyncScopedLifestyle.BeginScope(container))
            {
                container.Verify();
                Console.WriteLine("DI container verified.");
            }

            Console.WriteLine("Done.");
            return 0;
        }

        private static void RunSmokeTests()
        {
            Console.WriteLine("Smoke Tests");
            Console.WriteLine("===========");

            var results = CsvUpdateDemo.SmokeTests.SmokeTests.RunAll();
            var passed = results.Count(r => r.Passed);
            var failed = results.Count - passed;

            for (var i = 0; i < results.Count; i++)
            {
                var r = results[i];
                Console.WriteLine((r.Passed ? "[PASS] " : "[FAIL] ") + r.Name);
                if (!r.Passed)
                    Console.WriteLine("       " + r.Details);
            }

            Console.WriteLine();
            Console.WriteLine("Smoke Test Summary: " + passed + " passed, " + failed + " failed.");
            Console.WriteLine();
        }
    }
}
