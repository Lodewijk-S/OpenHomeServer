using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenHomeServer.Server.Plugins.Ripper.Domain
{
    public class RipperSettings
    {
        public RipperSettings()
        {
            MusicCollectionRoot = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
        }

        public string MusicCollectionRoot { get; set; }
    }
}
