using System.Windows.Controls;

namespace Frootify.PluginSystem
{
    public interface IPlugin
    {
        string Name { get; }
        UserControl PluginControl { get; }
        void Initialize();
        void Execute();
        void Shutdown();
        void SubscribeToEvents(EventAggregator eventAggregator);
    }
}
