using System;
using Orleans;
using Orleans.Runtime.Configuration;

namespace SiloHost
{
    /// <summary>
    ///     Orleans test silo host
    /// </summary>
    public class Program
    {
        private static OrleansHostWrapper _hostWrapper;

        private static void Main(string[] args)
        {
            // The Orleans silo environment is initialized in its own app domain in order to more
            // closely emulate the distributed situation, when the client and the server cannot
            // pass data via shared memory.
            var hostDomain = AppDomain.CreateDomain(
                "OrleansHost",
                null,
                new AppDomainSetup
                {
                    AppDomainInitializer = InitSilo,
                    AppDomainInitializerArguments = args
                });

            var config = ClientConfiguration.LocalhostSilo();
            GrainClient.Initialize(config);

            // TODO: once the previous call returns, the silo is up and running.
            //       This is the place your custom logic, for example calling client logic
            //       or initializing an HTTP front end for accepting incoming requests.

            Console.WriteLine("Orleans Silo is running.\nPress Enter to terminate...");
            Console.ReadLine();

            hostDomain.DoCallBack(ShutdownSilo);
        }

        private static void InitSilo(string[] args)
        {
            _hostWrapper = new OrleansHostWrapper(args);

            if (!_hostWrapper.Run())
            {
                Console.Error.WriteLine("Failed to initialize Orleans silo");
            }
        }

        private static void ShutdownSilo()
        {
            if (_hostWrapper == null)
            {
                return;
            }

            _hostWrapper.Dispose();
            GC.SuppressFinalize(_hostWrapper);
        }
    }
}