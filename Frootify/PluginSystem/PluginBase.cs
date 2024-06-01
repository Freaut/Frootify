using System;
using System.Diagnostics;
using System.Windows.Controls;

namespace Frootify.PluginSystem
{
    public abstract class PluginBase : IPlugin
    {
        protected EventAggregator? _eventAggregator;
        public abstract string Name { get; }
        public abstract UserControl PluginControl { get; set; }

        public virtual void Initialize()
        {
            Debug.WriteLine($"{Name} plugin initialized.");
        }

        public abstract void Execute();

        public virtual void Shutdown()
        {
            Debug.WriteLine($"{Name} plugin shutdown.");
        }

        public void SubscribeToEvents(EventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }
    }
}