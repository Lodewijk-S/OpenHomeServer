using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CdRipper.Encode;
using CdRipper.Rip;
using CdRipper.Tagging;
using OpenHomeServer.Server.Plugins.Notifications;

namespace OpenHomeServer.Server.Plugins.Ripper
{
    public class RipperService
    {
        private readonly Notificator _notificator;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private RippingStatus _rippingStatus = RippingStatus.Idle;

        public RipperService(Notifications.Notificator notificator)
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
                foreach (var track in drive.ReadTableOfContents().Result.Tracks)
                {
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
                                _notificator.SendNotificationToAllClients(new Notification(string.Format("ripped {0} of {1}", read, bytes)));
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
