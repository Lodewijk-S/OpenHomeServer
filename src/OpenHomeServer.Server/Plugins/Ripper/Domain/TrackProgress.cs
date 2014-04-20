using System.Threading;
using CdRipper.Tagging;

namespace OpenHomeServer.Server.Plugins.Ripper.Domain
{
    public class TrackProgress
    {
        private double _percentageComplete;

        public TrackProgress(int trackNumber)
        {
            TrackNumber = trackNumber;
            _percentageComplete = 0;
        }

        public int TrackNumber { get; private set; }
        public double PercentageComplete { get { return _percentageComplete; } }

        public void UpdatePercentageComplete(double percentage)
        {
            Interlocked.Exchange(ref _percentageComplete, percentage);
        }
    }
}