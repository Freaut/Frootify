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

    public void DiscoverPlugins(bool clearExisting = false, IEnumerable<IPlugin> inMemoryPlugins = null, string pluginDir = null)
    {
        if (clearExisting)
        {
            _plugins.Clear();
        }

        // Load in-memory plugins
        if (inMemoryPlugins != null)
        {
            foreach (var plugin in inMemoryPlugins)
            {
                if (!Plugins.Contains(plugin))
                {
                    _plugins.Add(plugin);
                }
            }
        }

        // Load plugins from directory
        if (pluginDir != null && Directory.Exists(pluginDir))
        {
            LoadPlugins(pluginDir);
        }

        // Discover plugins marked with [Plugin] attribute in loaded assemblies
        DiscoverAttributeMarkedPlugins();
    }

    private void DiscoverAttributeMarkedPlugins()
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();

        foreach (var assembly in assemblies)
        {
            var pluginTypes = assembly.GetTypes()
                .Where(t => t.GetCustomAttribute<PluginAttribute>() != null)
                .Where(t => typeof(IPlugin).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

            foreach (var type in pluginTypes)
            {
                if (Activator.CreateInstance(type) is IPlugin plugin)
                {
                    if (!Plugins.Any(p => p.GetType() == type))
                    {
                        _plugins.Add(plugin);
                    }
                }
            }
        }
    }

    public void UpdatePlugins(MainWindow instance)
    {
        foreach (var plugin in Plugins)
        {
            // Assuming each plugin has a method to refresh itself, e.g., Reload or Update method
            // You might need to define an interface method for this purpose
            // plugin.Reload(); // or plugin.Update();
        }
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