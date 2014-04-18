using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CdRipper.Encode;
using CdRipper.Rip;
using CdRipper.Tagging;

namespace OpenHomeServer.Server.Plugins.Ripper
{
    public class TrackProgress
    {
        private double _percentageComplete;

        public TrackProgress(TrackIdentification track)
        {
            TrackNumber = track.TrackNumber;
            Title = track.Title;
            Artist = track.Artist;
            _percentageComplete = 0;
        }

        public int TrackNumber { get; private set; }
        public string Title { get; private set; }
        public string Artist { get; private set; }
        public double PercentageComplete { get { return _percentageComplete; } }

        public void UpdatePercentageComplete(double percentage)
        {
            Interlocked.Exchange(ref _percentageComplete, percentage);
        }
    }

    public class AlbumProgress
    {
        public AlbumProgress(AlbumIdentification album)
        {
            AlbumArtist = album.AlbumArtist;
            AlbumTitle = album.AlbumTitle;
            Tracks = (from t in album.Tracks
                select new TrackProgress(t)).ToList();
        }

        public string AlbumTitle { get; private set; }
        public string AlbumArtist { get; private set; }
        public IEnumerable<TrackProgress> Tracks { get; private set; }
    }

    public class RippingStatus
    {
        public RippingStatus(IEnumerable<AlbumIdentification> identifiedAlbums)
        {
            AlbumIdentifications = identifiedAlbums.ToList();
            if (AlbumIdentifications.Count() == 1)
            {
                SelectAlbum(AlbumIdentifications.First());
            }
        }

        public AlbumProgress Progress { get; private set; }
        public IEnumerable<AlbumIdentification> AlbumIdentifications { get; private set; }
        public AlbumIdentification SelectedAlbum { get; private set; }

        public bool CanRip{get { return Progress != null; }}

        public void SelectAlbum(AlbumIdentification album)
        {
            SelectedAlbum = album;
            Progress = new AlbumProgress(album);
        }
    }

    public class RipperService
    {
        private readonly RipperNotificator _notificator;
        private CancellationTokenSource _cancellationTokenSource;
        private RippingStatus _currentStatus;

        public RipperService(RipperNotificator notificator)
        {
            _notificator = notificator;
        }

        public async Task StartRipping(DriveInfo disc)
        {   
            using (var drive = CdDrive.Create(disc))
            {
                var tagSource = new MusicBrainzTagSource(new MusicBrainzApi("http://musicbrainz.org"));
                var albums = tagSource.GetTags(await drive.ReadTableOfContents()).ToList();
                _currentStatus = new RippingStatus(albums);
            }

            if (_currentStatus.CanRip)
            {
                StartRipping(disc, _currentStatus.SelectedAlbum);
            }
        }

        private void StartRipping(DriveInfo disc, AlbumIdentification album)
        {
            var rippingTask = new Task(() => 
            {
                try
                {
                    _cancellationTokenSource = new CancellationTokenSource();
                    _notificator.UpdateStatus(_currentStatus);
                    DoRipping(disc, album, _cancellationTokenSource.Token).Wait();
                }
                catch (Exception e)
                {
                    throw e;
                }
            }, TaskCreationOptions.LongRunning);
            rippingTask.ContinueWith(t =>
            {
                _currentStatus = null;
                _notificator.UpdateStatus(_currentStatus);
            });
            rippingTask.Start();
        }

        public void CancelRipping()
        {
            if(_cancellationTokenSource != null)
                _cancellationTokenSource.Cancel();
        }

        public RippingStatus GetCurrentStatus()
        {
            return _currentStatus;
        }
        
        private async Task DoRipping(DriveInfo disc, AlbumIdentification album, CancellationToken token)
        {
            using (var drive = CdDrive.Create(disc))
            {
                var toc = await drive.ReadTableOfContents();
                foreach (var track in toc.Tracks)
                {
                    if (token.IsCancellationRequested)
                        break;

                    var trackid = album.Tracks.Single(t => t.TrackNumber == track.TrackNumber);
                    await RipTrack(drive, track, trackid, token);
                }
                await drive.Eject();
            }
        }

        private async Task RipTrack(ICdDrive drive, Track track, TrackIdentification trackIdentification, CancellationToken token)
        {
            var currentTrackNumber = track.TrackNumber;

            using (var reader = new TrackReader(drive))
            {
                using (var lame = new LameMp3Encoder(new EncoderSettings
                {
                    Track = trackIdentification,
                    Output = new OutputLocation
                    {
                        BaseDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), @"encoding\OpenHomeserverRipping\"),
                        FileNameMask = "{albumartist}\\{albumtitle}\\{tracknumber}-{title}.mp3"
                    }
                }))
                {
                    await reader.ReadTrack(track, lame.Write, (read, bytes) =>
                    {
                        var percentageComplete = Math.Round(((double)read / (double)bytes) * 100d, 0);
                        _currentStatus.Progress.Tracks.ElementAt(currentTrackNumber - 1).UpdatePercentageComplete(percentageComplete);
                        _notificator.UpdateProgress(currentTrackNumber, percentageComplete);
                    }, token);
                }
            }
        }
    }
}
