using System;

namespace OpenHomeServer.Server.Plugins.Notification
{
    [Serializable]
    public class Notification
    {
        public string Message { get; private set; }
        public Uri Link { get; private set; }
        public Level Level { get; private set; }

        public Notification(string message, Uri link = null, Level level = Level.Info)
        {
            Link = link;
            Level = level;
            Message = message;
        }
    }

    public enum Level
    {
        Info = 0,
        Warning = 1,
        Error = 2
    }
}