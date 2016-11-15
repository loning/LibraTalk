/*
Project Orleans Cloud Service SDK ver. 1.0
 
Copyright (c) Microsoft Corporation
 
All rights reserved.
 
MIT License

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and 
associated documentation files (the ""Software""), to deal in the Software without restriction,
including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense,
and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so,
subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO
THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS
OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using System;
using System.Net;
using Orleans.Runtime.Host;

namespace LibraProgramming.Grains.Host
{
    internal class OrleansHostWrapper : IDisposable
    {
        private SiloHost silo;

        public bool Debug
        {
            get
            {
                return silo != null && silo.Debug;
            }
            set
            {
                silo.Debug = value;
            }
        }

        public OrleansHostWrapper(string[] args)
        {
            ParseArguments(args);
            Init();
        }

        public bool Run()
        {
            var ok = false;

            try
            {
                silo.InitializeOrleansSilo();

                ok = silo.StartOrleansSilo();

                if (ok)
                {
                    Console.WriteLine($"Successfully started Orleans silo '{silo.Name}' as a {silo.Type} node.");
                }
                else
                {
                    throw new SystemException($"Failed to start Orleans silo '{silo.Name}' as a {silo.Type} node.");
                }
            }
            catch (Exception exc)
            {
                silo.ReportStartupError(exc);
                Console.WriteLine($"{exc.GetType().FullName}:\n{exc.Message}\n{exc.StackTrace}");
            }

            return ok;
        }

        public bool Stop()
        {
            var ok = false;

            try
            {
                silo.StopOrleansSilo();
                Console.WriteLine($"Orleans silo '{silo.Name}' shutdown.");
            }
            catch (Exception exc)
            {
                silo.ReportStartupError(exc);
                Console.WriteLine($"{exc.GetType().FullName}:\n{exc.Message}\n{exc.StackTrace}");
            }

            return ok;
        }

        private void Init()
        {
            silo.LoadOrleansConfig();
        }

        private bool ParseArguments(string[] args)
        {
            string deploymentId = null;

            var configFileName = "DevTestServerConfiguration.xml";
            var siloName = Dns.GetHostName(); // Default to machine name
            var argPos = 1;

            for (var i = 0; i < args.Length; i++)
            {
                var a = args[i];

                if (a.StartsWith("-") || a.StartsWith("/"))
                {
                    switch (a.ToLowerInvariant())
                    {
                        case "/?":
                        case "/help":
                        case "-?":
                        case "-help":
                            // Query usage help
                            return false;
                        default:
                            Console.WriteLine("Bad command line arguments supplied: " + a);
                            return false;
                    }
                }

                if (a.Contains("="))
                {
                    var split = a.Split('=');

                    if (String.IsNullOrEmpty(split[1]))
                    {
                        Console.WriteLine("Bad command line arguments supplied: " + a);
                        return false;
                    }

                    switch (split[0].ToLowerInvariant())
                    {
                        case "deploymentid":
                            deploymentId = split[1];
                            break;
                        case "deploymentgroup":
                            // TODO: Remove this at some point in future
                            Console.WriteLine("Ignoring deprecated command line argument: " + a);
                            break;
                        default:
                            Console.WriteLine("Bad command line arguments supplied: " + a);
                            return false;
                    }
                }
                // unqualified arguments below
                else if (argPos == 1)
                {
                    siloName = a;
                    argPos++;
                }
                else if (argPos == 2)
                {
                    configFileName = a;
                    argPos++;
                }
                else
                {
                    // Too many command line arguments
                    Console.WriteLine("Too many command line arguments supplied: " + a);
                    return false;
                }
            }

            silo = new SiloHost(siloName)
            {
                ConfigFileName = configFileName
            };

            if (deploymentId != null)
            {
                silo.DeploymentId = deploymentId;
            }

            return true;
        }

        public void PrintUsage()
        {
            Console.WriteLine(
@"USAGE: 
    orleans host [<siloName> [<configFile>]] [DeploymentId=<idString>] [/debug]
Where:
    <siloName>      - Name of this silo in the Config file list (optional)
    <configFile>    - Path to the Config file to use (optional)
    DeploymentId=<idString> 
                    - Which deployment group this host instance should run in (optional)");
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool dispose)
        {
            silo.Dispose();
            silo = null;
        }
    }
}
