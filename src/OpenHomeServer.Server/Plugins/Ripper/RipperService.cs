using System.IO;
using System.Linq;
using CdRipper.Encode;
using CdRipper.Rip;
using CdRipper.Tagging;

namespace OpenHomeServer.Server.Plugins.Ripper
{
    public class RipperService
    {
        public void Start(DriveInfo disc, DiscIdentification single)
        {
            using (var drive = CdDrive.Create(disc))
            {
                foreach (var track in drive.ReadTableOfContents().Result.Tracks)
                {
                    using (var reader = new TrackReader(drive))
                    {
                        using (var lame = new LameMp3Encoder(new EncoderSettings
                        {
                            Track = single.Tracks.Single(t => t.TrackNumber == track.TrackNumber)
                        }))
                        {
                            reader.ReadTrack(track, lame.Write, (read, bytes) => { }).Wait();
                        }
                    }
                }
            }
        }

        public RippingStatus GetCurrentStatus()
        {
            return new RippingStatus();
        }
    }

    public class RippingStatus
    {
        public DriveStatus Drive { get; private set; }
        public TableOfContents Disc { get; private set; }
    }

    public enum DriveStatus
    {
        Empty,
        Idle,
        Busy
    }
}
