using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
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

        [DisplayName("Rip music to"), Required]
        public string MusicCollectionRoot { get; set; }

        [DisplayName("Filename Mask"), Required]
        public string FileNameMask { get; set; }

        [DisplayName("Bitrate"), Required]
        public int BitRate { get; set; }
        
        public Mp3Settings.BitrateType BitRateType { get; set; }
    }
}
