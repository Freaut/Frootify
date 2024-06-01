using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frootify.PluginSystem
{
    public class PluginLoader
    {
        public PluginManager _pluginManager { get; set; }

        public PluginLoader(EventAggregator eventAggregator)
        {
            _pluginManager = new PluginManager(eventAggregator);
        }

        public void Start(EventAggregator _eventAggregator)
        {
            foreach (var plugin in _pluginManager.Plugins)
            {
                plugin.SubscribeToEvents(_eventAggregator);
            }

            _pluginManager.InitializeAllPlugins();
            _pluginManager.ExecuteAllPlugins();

            var applicationStartedEvent = new ApplicationStartedEvent();
            _eventAggregator.Publish(applicationStartedEvent);
        }

        public void Dispose(EventAggregator _eventAggregator)
        {
            _eventAggregator.Publish(new ApplicationShutdownEvent());
            _pluginManager.ShutdownAllPlugins();
        }
    }
}
