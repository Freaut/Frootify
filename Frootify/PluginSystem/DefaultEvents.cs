namespace Frootify.PluginSystem
{
    public class ApplicationStartedEvent : IEvent { }
    public class ApplicationShutdownEvent : IEvent { }
    public class NotificationPopupEvent : IEvent { }
    public class SongChangedEvent : IEvent
    {
        public Song CurrentSong { get; set; }
        public SongChangedEvent(Song song)
        {
            CurrentSong = song;
        }
    }
    public class SongPausedEvent : IEvent { }
    public class PlaylistCreatedEvent : IEvent { }
    public class PlaylistUpdatedEvent : IEvent { }
    public class PlaylistDeletedEvent : IEvent { }
}
