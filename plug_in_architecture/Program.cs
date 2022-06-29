using PlugInSDK;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace plug_in_architecture
{
    class Program
    {
        static void Main(string[] args)
        {
            var sdk = 
                AppDomain.CurrentDomain.GetAssemblies()
                .Single(asm=>(Path.GetFileName(asm.Location) == "PlugInSDK.dll"));
            Console.WriteLine(
                $"'{sdk.FullName}'\nAlready loaded from:\n{sdk.Location}\n");

            // FYI: Get an event whenever an assembly has been loaded.
            AppDomain.CurrentDomain.AssemblyLoad += (sender, e) =>
            {
                var name = e.LoadedAssembly.FullName;
                if (name.Split(",").First().Contains("PlugIn"))
                {
                    Console.WriteLine($"{name}\nLoaded on-demand from:\n{e.LoadedAssembly.Location}\n");
                }
            };

            // Path to arbitrary location of Plugins folder.
            var pluginPath =
                Path.Combine(
                    Path.GetDirectoryName(Assembly.GetEntryAssembly().Location),
                    "..",
                    "..",
                    "..",
                    "PlugIns"
                );

            List<IPlugin> plugins = new List<IPlugin>();
            foreach (var plugin in Directory.GetFiles(pluginPath, "*.dll", SearchOption.AllDirectories))
            {
                // Exclude copy of the SDK that gets put here when plugins are built.
                if(Path.GetFileName(plugin) == "PlugInSDK.dll") continue;

                var asm = Assembly.LoadFrom(plugin);
                Type plugInType = 
                    asm.ExportedTypes
                    .Where(type =>
                        type.GetTypeInfo().ImplementedInterfaces
                        .Any(intfc => intfc.Name == "IPlugin")
                    ).SingleOrDefault();

                if ((plugInType != null) && plugInType.IsClass)
                {
                    plugins.Add((IPlugin)Activator.CreateInstance(plugInType));
                }
            }
            Console.WriteLine("LIST OF PLUGINS");
            Console.WriteLine(string.Join(Environment.NewLine, plugins.Select(plugin => plugin.Name)));
            Console.ReadKey();
        }
    }
}
