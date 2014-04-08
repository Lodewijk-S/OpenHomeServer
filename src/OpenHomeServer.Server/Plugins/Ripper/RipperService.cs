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

    public class RipperService
    {
        private readonly RipperNotificator _notificator;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private AlbumProgress _currentStatus;

        public RipperService(RipperNotificator notificator)
        {
            _notificator = notificator;
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public void StartRipping(DriveInfo disc, AlbumIdentification album)
        {
            var rippingTask = new Task(() => 
            {
                _currentStatus = new AlbumProgress(album);
                _notificator.UpdateStatus(_currentStatus);
                DoRipping(disc, album).Wait();
            }, _cancellationTokenSource.Token, TaskCreationOptions.LongRunning);
            rippingTask.ContinueWith(t =>
            {
                _currentStatus = null;
                _notificator.UpdateStatus(_currentStatus);
            });
            rippingTask.Start();
        }

        public void CancelRipping()
        {
            _cancellationTokenSource.Cancel();
        }

        public AlbumProgress GetCurrentStatus()
        {
            return _currentStatus;
        }

        private async Task DoRipping(DriveInfo disc, AlbumIdentification single)
        {
            using (var drive = CdDrive.Create(disc))
            {
                var toc = await drive.ReadTableOfContents();
                
                foreach (var track in toc.Tracks)
                {
                    var currentTrackNumber = track.TrackNumber;

                    using (var reader = new TrackReader(drive))
                    {
                        using (var lame = new LameMp3Encoder(new EncoderSettings
                        {
                            Track = single.Tracks.Single(t => t.TrackNumber == track.TrackNumber),
                            Output = new OutputLocation
                            {
                                BaseDirectory = @"C:\Users\lodesioe\Desktop\encoding\OpenHomeserverRipping\",
                                FileNameMask = "{albumartist}\\{albumtitle}\\{tracknumber}-{title}.mp3"
                            }
                        }))
                        {
                            await reader.ReadTrack(track, lame.Write, (read, bytes) =>
                            {
                                var percentageComplete = Math.Round(((double)read/(double)bytes)*100d, 0);
                                _currentStatus.Tracks.ElementAt(currentTrackNumber-1).UpdatePercentageComplete(percentageComplete);
                                _notificator.UpdateProgress(currentTrackNumber, percentageComplete);
                            });
                        }
                    }
                }
            }
        }
    }
}
