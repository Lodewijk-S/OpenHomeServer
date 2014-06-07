using System;
using System.ComponentModel;
using CdRipper.Encode;

namespace OpenHomeServer.Server.Plugins.Ripper.Domain
{
    public class RipperSettings
    {
        public RipperSettings()
        {
            MusicCollectionRoot = Environment.GetFolderPath(Environment.SpecialFolder.CommonMusic);
            FileNameMask = "{albumartist}\\{albumtitle}\\{tracknumber}-{title}.mp3";
            BitRate = 192;
            BitRateType = Mp3Settings.BitrateType.Variable;
        }

        [DisplayName("Rip music to")]
        public string MusicCollectionRoot { get; set; }

        [DisplayName("Filename Mask")]
        public string FileNameMask { get; set; }

        [DisplayName("Bitrate")]
        public int BitRate { get; set; }
        
        public Mp3Settings.BitrateType BitRateType { get; set; }
    }
}
