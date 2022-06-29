using System;

// Sign this assembly with a Strong Name Key (.snk)
namespace PlugInSDK
{
    public interface IPlugin
    {
        public string Name { get; }
    }
    public class PlugIn : IPlugin
    {
        public PlugIn(string name) => Name = name;

        public string Name { get; }
    }
}
