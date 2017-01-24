using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using DasMulli.Win32.ServiceUtils;
using Microsoft.Extensions.DependencyInjection;
using System.Text.RegularExpressions;

namespace Destec.CoreApi
{
    public class Program
    {
        private const string RunAsServiceFlag = "--run-as-service";
        private const string RegisterServiceFlag = "--register-service";
        private const string UnregisterServiceFlag = "--unregister-service";
        private const string InteractiveFlag = "--interactive";

        private const string ServiceName = "DestecService";
        private const string ServiceDisplayName = "Destec Service App";
        private const string ServiceDescription = "Aplicação de execução de atividades ";

        public static void Main(string[] args)
        {
            try
            {
                if (Debugger.IsAttached || args.Contains("--debug"))
                {
                    var config = new ConfigurationBuilder()
                    .AddCommandLine(args)
                    .AddEnvironmentVariables(prefix: "ASPNETCORE_")
                    .Build();

                    var host = new WebHostBuilder()
                        .UseUrls("http://localhost:5000", "http://0.0.0.0:5000")
                        .UseConfiguration(config)
                        .UseKestrel()
                        .UseContentRoot(Directory.GetCurrentDirectory())
                        .UseStartup<Startup>()
                        .Build();

                    host.Run();
                }
                else if (args.Contains(RunAsServiceFlag))
                {
                    RunAsService(args);
                }
                else if (args.Contains(RegisterServiceFlag))
                {
                    RegisterService();
                }
                else if (args.Contains(UnregisterServiceFlag))
                {
                    UnregisterService();
                }
                else if (args.Contains(InteractiveFlag))
                {
                    RunInteractive(args);
                }
                else
                {
                    DisplayHelp();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error ocurred: {ex.Message}");
            }
        }

        private static void RunAsService(string[] args)
        {
            var testService = new DestecService(args.Where(a => a != RunAsServiceFlag).ToArray());
            var serviceHost = new Win32ServiceHost(testService);
            serviceHost.Run();
        }

        private static void RunInteractive(string[] args)
        {
            var testService = new DestecService(args.Where(a => a != InteractiveFlag).ToArray());
            testService.Start(new string[0], () => { });
            Console.WriteLine("Running interactively, press enter to stop.");
            Console.ReadLine();
            testService.Stop();
        }

        private static void RegisterService()
        {
            // Environment.GetCommandLineArgs() includes the current DLL from a "dotnet my.dll --register-service" call, which is not passed to Main()
            var remainingArgs = Environment.GetCommandLineArgs()
                .Where(arg => arg != RegisterServiceFlag)
                .Select(EscapeCommandLineArgument)
                .Append(RunAsServiceFlag);

            var host = Process.GetCurrentProcess().MainModule.FileName;

            if (!host.EndsWith("dotnet.exe", StringComparison.OrdinalIgnoreCase))
            {
                // For self-contained apps, skip the dll path
                remainingArgs = remainingArgs.Skip(1);
            }

            var fullServiceCommand = host + " " + string.Join(" ", remainingArgs);
            
            // Note that when the service is already registered and running, it will be reconfigured but not restarted
            new Win32ServiceManager()
                .CreateService(
                    ServiceName,
                    ServiceDisplayName,
                    ServiceDescription,
                    fullServiceCommand,
                    Win32ServiceCredentials.LocalSystem,
                    autoStart: true,
                    startImmediately: false,
                    errorSeverity: ErrorSeverity.Normal
                );

            Console.WriteLine($@"Successfully registered and started service ""{ServiceDisplayName}"" (""{ServiceDescription}"")");
        }

        private static void UnregisterService()
        {
            new Win32ServiceManager()
                .DeleteService(ServiceName);

            Console.WriteLine($@"Successfully unregistered service ""{ServiceDisplayName}"" (""{ServiceDescription}"")");
        }

        private static void DisplayHelp()
        {
            Console.WriteLine(ServiceDescription);
            Console.WriteLine();
            Console.WriteLine("This demo application is intened to be run as windows service. Use one of the following options:");
            Console.WriteLine("  --register-service        Registers and starts this program as a windows service named \"" + ServiceDisplayName + "\"");
            Console.WriteLine("                            All additional arguments will be passed to ASP.NET Core's WebHostBuilder.");
            Console.WriteLine("  --unregister-service      Removes the windows service creatd by --register-service.");
            Console.WriteLine("  --interactive             Runs the underlying asp.net core app. Useful to test arguments.");
        }

        private static string EscapeCommandLineArgument(string arg)
        {
            // http://stackoverflow.com/a/6040946/784387
            arg = Regex.Replace(arg, @"(\\*)" + "\"", @"$1$1\" + "\"");
            arg = "\"" + Regex.Replace(arg, @"(\\+)$", @"$1$1") + "\"";
            return arg;
        }
    }

    internal class DestecService : IWin32Service
    {
        private readonly string[] commandLineArguments;
        private IWebHost webHost;
        private bool stopRequestedByWindows;

        public string ServiceName => "DestecService";

        public DestecService(string[] commandLineArguments)
        {
            this.commandLineArguments = commandLineArguments;
        }

        public void Start(string[] args, ServiceStoppedCallback serviceStoppedCallback)
        {
            var config = new ConfigurationBuilder()
                .AddCommandLine(args)
                .AddEnvironmentVariables(prefix: "ASPNETCORE_")
                .Build();

            webHost = new WebHostBuilder()
                .UseUrls("http://localhost:5000", "http://0.0.0.0:5000")
                .UseConfiguration(config)
                .UseKestrel()
                .UseContentRoot(AppContext.BaseDirectory)
                .UseStartup<Startup>()
                .Build();
            
            webHost
                .Services
                .GetRequiredService<IApplicationLifetime>()
                .ApplicationStopped
                .Register(() =>
                {
                    if (stopRequestedByWindows == false)
                    {
                        serviceStoppedCallback();
                    }
                });

            webHost.Start();

            return;
        }
        
        public void Stop()
        {
            stopRequestedByWindows = true;
            webHost.Dispose();
        }
    }
}
