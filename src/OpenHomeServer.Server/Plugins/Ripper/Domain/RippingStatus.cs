using System.Collections.Generic;
using System.Linq;
using CdRipper.Tagging;

namespace OpenHomeServer.Server.Plugins.Ripper.Domain
{
    public class RippingStatusViewModel
    {
        public RippingStatusViewModel(IEnumerable<AlbumIdentification> albums)
        {
            Albums = (from a in albums ?? Enumerable.Empty<AlbumIdentification>()
                let tacks = from t in a.Tracks
                                  select new TrackStatusViewModel(t.Title, t.Artist, t.TrackNumber, 0)
                      select new AlbumStatusViewModel(a.AlbumTitle, a.AlbumArtist, tacks.ToList())).ToList();
        }

        public RippingStatusViewModel(AlbumIdentification album, IEnumerable<TrackProgress> progress)
        {
            var tracks = from t in album != null ? album.Tracks : Enumerable.Empty<TrackIdentification>()
                         let pc = progress.First(p => p.TrackNumber == t.TrackNumber).PercentageComplete
                         select new TrackStatusViewModel(t.Title, t.Artist, t.TrackNumber, pc);

            Albums = album == null ? null : new[]
            {
                new AlbumStatusViewModel(album.AlbumTitle, album.AlbumArtist, tracks.ToList()) 
            };
        }

        public IEnumerable<AlbumStatusViewModel> Albums { get; private set; }
    }

    public class AlbumStatusViewModel
    {
        public AlbumStatusViewModel(string title, string artist, IEnumerable<TrackStatusViewModel> tracks)
        {
            Tracks = tracks;
            Artist = artist;
            Title = title;
        }

        public string Title { get; private set; }
        public string Artist { get; private set; }
        public IEnumerable<TrackStatusViewModel> Tracks { get; private set; }
    }

    public class TrackStatusViewModel
    {
        public TrackStatusViewModel(string title, string artist, int trackNumber, double percentageComplete)
        {
            PercentageComplete = percentageComplete;
            TrackNumber = trackNumber;
            Artist = artist;
            Title = title;
        }

        public string Title { get; private set; }
        public string Artist { get; private set; }
        public int TrackNumber { get; private set; }
        public double PercentageComplete { get; private set; }
    }
}