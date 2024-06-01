namespace Frootify.PluginSystem;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

public class PluginManager
{
    private readonly List<IPlugin> _plugins;
    public IReadOnlyList<IPlugin> Plugins => _plugins.AsReadOnly();
    private readonly EventAggregator _eventAggregator;

    public PluginManager(EventAggregator eventAggregator)
    {
        _plugins = new List<IPlugin>();
        _eventAggregator = eventAggregator;
    }

    /// <summary>
    /// Used for debugging/testing without having to compile a plugin
    /// </summary>
    public void LoadPlugin(IPlugin plugin)
    {
        _plugins.Add(plugin);
    }

    public void LoadPlugins(string directoryPath)
    {
        if (!Directory.Exists(directoryPath))
            Directory.CreateDirectory(directoryPath);

        var files = Directory.GetFiles(directoryPath, "*.dll");

        foreach (var file in files)
        {
            var assembly = Assembly.LoadFrom(file);

            var types = assembly.GetTypes()
                .Where(t => typeof(IPlugin).IsAssignableFrom(t) && !t.IsAbstract);

            foreach (var type in types)
            {
                var plugin = (IPlugin)Activator.CreateInstance(type);
                if (plugin != null)
                {
                    _plugins.Add(plugin);
                }
            }
        }
    }

    public void InitializeAllPlugins()
    {
        foreach (var plugin in _plugins)
        {
            plugin.Initialize();
        }
    }

    public void ExecuteAllPlugins()
    {
        foreach (var plugin in _plugins)
        {
            plugin.Execute();
        }
    }

    public void ShutdownAllPlugins()
    {
        var shutdownEvent = new ApplicationShutdownEvent();
        _eventAggregator.Publish(shutdownEvent);

        foreach (var plugin in _plugins)
        {
            plugin.Shutdown();
        }
    }
}