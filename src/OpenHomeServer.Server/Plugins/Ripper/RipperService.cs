using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CdRipper.Encode;
using CdRipper.Rip;
using CdRipper.Tagging;
using OpenHomeServer.Server.Plugins.Notifications;
using OpenHomeServer.Server.Plugins.Ripper.Domain;
using Serilog;

namespace OpenHomeServer.Server.Plugins.Ripper
{
    public class RipperService
    {
        private readonly RipperNotificator _notificator;
        private readonly Notificator _mainNotificator;
        private readonly ILogger _logger;
        private readonly ITagSource _tagSource;
        private readonly StatusTracker _tracker;

        private CancellationTokenSource _cancellationTokenSource;
        private Task _rippingTask;

        public RipperService(RipperNotificator notificator, Notificator mainNotificator, ILogger logger)
        {
            _notificator = notificator;
            _mainNotificator = mainNotificator;
            _logger = logger;
            _tagSource = new MusicBrainzTagSource(new MusicBrainzApi("http://musicbrainz.org"));
            _tracker = new StatusTracker();

            _tracker.OnStatusUpdated += s => _notificator.UpdateStatus(s);
            _tracker.OnRippingProgress += (t, p) => _notificator.UpdateProgress(t, p);

            _tracker.OnDiscInserted += d => IdentifyAlbum(d, true);
            _tracker.OnAlbumSelected += (d, a) => StartRipping(d, a);
        }

        public void OnDiscInsertion(DriveInfo drive)
        {
            _tracker.DiscInserted(drive);
        }

        public void OnDiscEjected(DriveInfo drive)
        {
            _tracker.Clear();
        }

        public async void IdentifyAlbum(DriveInfo drive, bool autoStart)
        {
            using (var cdDrive = CdDrive.Create(drive))
            {
                var albums = _tagSource.GetTags(await cdDrive.ReadTableOfContents()).ToList();
                _tracker.AlbumsIdentified(albums);

                if (autoStart && albums.Count == 1)
                {
                    SelectAlbum(albums.Single().Id);
                }
            }
        }

        public void SelectAlbum(string albumId)
        {
            _tracker.AlbumSelected(albumId);
        }

        public void StartRipping(DriveInfo drive, AlbumIdentification album)
        {
            if (IsRipping())
            {
                _logger.Debug("Cannot start two ripping processes at the same time.");
                return;
            }

            _rippingTask = new Task(() => 
            {
                try
                {
                    _cancellationTokenSource = new CancellationTokenSource();
                    RipAlbum(drive, album, _cancellationTokenSource.Token).Wait();
                }
                catch (Exception e)
                {
                    _logger.Error(e, "An exception occured while ripping");
                    throw e;
                }
            }, TaskCreationOptions.LongRunning);
            _rippingTask.ContinueWith(t =>
            {
                _tracker.Clear();
            });
            _rippingTask.Start();
        }

        public void CancelRipping()
        {
            if (_cancellationTokenSource != null && IsRipping())
            {
                _cancellationTokenSource.Cancel();
                _tracker.Clear();
            }
        }

        public RippingStatusViewModel GetCurrentStatus()
        {
            return _tracker.GetStatus();
        }

        private bool IsRipping()
        {
            return _rippingTask != null && (_rippingTask.IsCompleted == false || _rippingTask.Status == TaskStatus.Running || _rippingTask.Status == TaskStatus.WaitingToRun || _rippingTask.Status == TaskStatus.WaitingForActivation);
        }

        private async Task RipAlbum(DriveInfo drive, AlbumIdentification album, CancellationToken token)
        {   
            using (var cdDrive = CdDrive.Create(drive))
            {
                var toc = await cdDrive.ReadTableOfContents();
                foreach (var track in toc.Tracks)
                {
                    if (token.IsCancellationRequested)
                        break;

                    var trackid = album.Tracks.Single(t => t.TrackNumber == track.TrackNumber);
                    await RipTrack(cdDrive, track, trackid, token);
                }
                await cdDrive.Eject();
            }
        }

        private async Task RipTrack(ICdDrive cdDrive, Track track, TrackIdentification trackIdentification, CancellationToken token)
        {
            var currentTrackNumber = track.TrackNumber;

            using (var reader = new TrackReader(cdDrive))
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
                    reader.Progress += (read, bytes) =>
                    {
                        var percentageComplete = Math.Round(((double) read/(double) bytes)*100d, 0);
                        _tracker.RippingProgress(currentTrackNumber, percentageComplete);
                    };
                    await reader.ReadTrack(track, lame.Write, token);
                }
            }
        }
    }
}
