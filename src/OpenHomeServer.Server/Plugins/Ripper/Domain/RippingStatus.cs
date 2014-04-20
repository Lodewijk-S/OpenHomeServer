using System.Collections.Generic;
using System.IO;
using System.Linq;
using CdRipper.Tagging;

namespace OpenHomeServer.Server.Plugins.Ripper.Domain
{
    public class RippingStatus
    {
        public string DriveName { get; private set; }
        public IDictionary<int, TrackProgress> Progress { get; private set; }
        public IDictionary<string, AlbumIdentification> AlbumIdentifications { get; private set; }
        public AlbumIdentification SelectedAlbum { get; private set; }

        public RippingStatus(DriveInfo drive, IEnumerable<AlbumIdentification> identifiedAlbums)
        {
            DriveName = drive.Name;
            AlbumIdentifications = identifiedAlbums.ToDictionary(a => a.Id);

            if (AlbumIdentifications.Count() == 1)
            {
                SelectAlbum(AlbumIdentifications.Keys.First());
            }
        }

        public bool CanRip { get { return SelectedAlbum != null; } }

        public void SelectAlbum(string albumId)
        {   
            SelectedAlbum = AlbumIdentifications[albumId];
            Progress = (from track in SelectedAlbum.Tracks
                        select new TrackProgress(track.TrackNumber)).ToDictionary(t => t.TrackNumber);
        }
    }
}