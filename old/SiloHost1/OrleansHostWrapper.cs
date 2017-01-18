using System;
using System.Net;
using Orleans.Runtime.Configuration;

namespace SiloHost
{
    internal class OrleansHostWrapper : IDisposable
    {
        private Orleans.Runtime.Host.SiloHost _siloHost;

        public OrleansHostWrapper(string[] args)
        {
            ParseArguments(args);
            Init();
        }

        public bool Debug
        {
            get { return _siloHost != null && _siloHost.Debug; }
            set { _siloHost.Debug = value; }
        }

        public void Dispose()
        {
            _siloHost.Dispose();
            _siloHost = null;
        }

        public bool Run()
        {
            try
            {
                _siloHost.InitializeOrleansSilo();

                if (!_siloHost.StartOrleansSilo())
                {
                    throw new SystemException(
                        $"Failed to start Orleans silo '{_siloHost.Name}' as a {_siloHost.Type} node.");
                }

                Console.WriteLine($"Successfully started Orleans silo '{_siloHost.Name}' as a {_siloHost.Type} node.");
                return true;
            }
            catch (Exception exc)
            {
                _siloHost.ReportStartupError(exc);
                var msg = $"{exc.GetType().FullName}:\n{exc.Message}\n{exc.StackTrace}";
                Console.WriteLine(msg);
                return false;
            }
        }

        public bool Stop()
        {
            try
            {
                _siloHost.StopOrleansSilo();

                Console.WriteLine($"Orleans silo '{_siloHost.Name}' shutdown.");
            }
            catch (Exception exc)
            {
                _siloHost.ReportStartupError(exc);
                var msg = $"{exc.GetType().FullName}:\n{exc.Message}\n{exc.StackTrace}";
                Console.WriteLine(msg);
            }

            return false;
        }

        private void Init()
        {
            _siloHost.LoadOrleansConfig();
        }

        private bool ParseArguments(string[] args)
        {
            string deploymentId = null;

            var siloName = Dns.GetHostName(); // Default to machine name

            var argPos = 1;
            foreach (var arg in args)
            {
                if (arg.StartsWith("-") || arg.StartsWith("/"))
                {
                    switch (arg.ToLowerInvariant())
                    {
                        case "/?":
                        case "/help":
                        case "-?":
                        case "-help":
                            // Query usage help
                            return false;
                        default:
                            Console.WriteLine("Bad command line arguments supplied: " + arg);
                            return false;
                    }
                }

                if (arg.Contains("="))
                {
                    var parts = arg.Split('=');
                    if (string.IsNullOrEmpty(parts[1]))
                    {
                        Console.WriteLine("Bad command line arguments supplied: " + arg);
                        return false;
                    }
                    switch (parts[0].ToLowerInvariant())
                    {
                        case "deploymentid":
                            deploymentId = parts[1];
                            break;
                        default:
                            Console.WriteLine("Bad command line arguments supplied: " + arg);
                            return false;
                    }
                }

                // unqualified arguments below
                else if (argPos == 1)
                {
                    siloName = arg;
                    argPos++;
                }
                else
                {
                    // Too many command line arguments
                    Console.WriteLine("Too many command line arguments supplied: " + arg);
                    return false;
                }
            }

            var config = ClusterConfiguration.LocalhostPrimarySilo();
            config.AddMemoryStorageProvider();
            _siloHost = new Orleans.Runtime.Host.SiloHost(siloName, config);

            if (deploymentId != null)
                _siloHost.DeploymentId = deploymentId;

            return true;
        }

        public void PrintUsage()
        {
            Console.WriteLine(@"USAGE: 
    orleans host [<siloName> [<configFile>]] [DeploymentId=<idString>] [/debug]
Where:
    <siloName>      - Name of this silo in the Config file list (optional)
    DeploymentId=<idString> 
                    - Which deployment group this host instance should run in (optional)");
        }
    }
}