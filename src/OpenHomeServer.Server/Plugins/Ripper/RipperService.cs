using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CdRipper.Encode;
using CdRipper.Rip;
using CdRipper.Tagging;
using OpenHomeServer.Server.Plugins.Notifications;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Infrastructure;
using System.Collections.Generic;
using OpenHomeServer.Server.Messaging;

namespace OpenHomeServer.Server.Plugins.Ripper
{
    public class RipperNotificationHub : Hub
    {
        public void UpdateRippingStatus(IEnumerable<TrackProgress> progress)
        {
            Clients.All.onRippingProgress(progress);
        }
    }

    public class RipperNotificator
    {
        private readonly IHubContext _hubContext;

        public RipperNotificator(IHubContextFactory hubContextFactory)
        {
            _hubContext = hubContextFactory.CreateHubContext<RipperNotificationHub>();
        }

        public void SendNotificationToAllClients(IEnumerable<TrackProgress> progress)
        {
            _hubContext.Clients.All.onRippingProgress(progress);
        }
    }

    public class TrackProgress
    {
        private int _percentageComplete;

        public TrackProgress(int trackNumber, int percentageComplete)
        {
            TrackNumber = trackNumber;
            _percentageComplete = percentageComplete;
        }

        public int TrackNumber { get; private set; }
        public int PercentageComplete { get { return _percentageComplete; } }

        public void UpdatePercentageComplete(int percentage)
        {
            Interlocked.Exchange(ref _percentageComplete, percentage);
        }
    }

    public class RipperService
    {
        private readonly RipperNotificator _notificator;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private RippingStatus _rippingStatus = RippingStatus.Idle;

        public RipperService(RipperNotificator notificator)
        {
            _notificator = notificator;
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public void StartRipping(DriveInfo disc, AlbumIdentification single)
        {
            var rippingTask = new Task(() => 
            {
                _rippingStatus = RippingStatus.Busy;
                DoRipping(disc, single); 
            }, _cancellationTokenSource.Token, TaskCreationOptions.LongRunning);
            rippingTask.ContinueWith(t => _rippingStatus = RippingStatus.Idle);
            rippingTask.Start();
        }

        public void CancelRipping()
        {
            _cancellationTokenSource.Cancel();
        }

        public RippingStatus GetCurrentStatus()
        {
            return _rippingStatus;
        }

        private async void DoRipping(DriveInfo disc, AlbumIdentification single)
        {
            using (var drive = CdDrive.Create(disc))
            {
                var toc = await drive.ReadTableOfContents();
                var listOfTracks = (from i in Enumerable.Range(1, toc.Tracks.Count())
                                   select new TrackProgress(i, 0)).ToArray();

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
                                listOfTracks[currentTrackNumber-1].UpdatePercentageComplete((int)(read/bytes)*100);
                                _notificator.SendNotificationToAllClients(listOfTracks);
                            });
                        }
                    }
                }
            }
        }
    }

    public enum RippingStatus
    {
        Idle,
        Busy,
        Canceled,
        Faulted
    }
}
