using System;
using System.ComponentModel;

namespace OpenHomeServer.Server.Plugins.Ripper.Domain
{
    public class RipperSettings
    {
        public RipperSettings()
        {
            MusicCollectionRoot = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
        }

        [DisplayName("Rip music to")]
        public string MusicCollectionRoot { get; set; }
    }
}
