using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CdRipper.Tagging;

namespace OpenHomeServer.Server.Plugins.Ripper.Domain
{
    public class StatusTracker
    {
        private DriveInfo _drive;
        private IEnumerable<AlbumIdentification> _albums;
        private IEnumerable<TrackProgress> _progress;
        private AlbumIdentification _album;

        public delegate void RippingProgressDelegate(int trackNumber, double percentageComplete);

        public event Action<RippingStatusViewModel> OnStatusUpdated = delegate { };
        public event RippingProgressDelegate OnRippingProgress = delegate { };
        public event Action<DriveInfo> OnDiscInserted = delegate { };
        public event Action<DriveInfo, IEnumerable<AlbumIdentification>> OnAlbumIdentified = delegate { };
        public event Action<DriveInfo, AlbumIdentification> OnAlbumSelected = delegate { };

        private RippingStatusViewModel StatusUpdated()
        {
            var status = GetStatus();
            OnStatusUpdated(status);
            return status;
        }

        public RippingStatusViewModel Clear()
        {
            _drive = null;
            _album = null;
            _albums = null;
            _progress = null;

            return StatusUpdated();
        }

        public RippingStatusViewModel DiscInserted(DriveInfo drive)
        {
            _albums = null;
            _album = null;
            _progress = null;
            _drive = drive;

            OnDiscInserted(drive);
            return StatusUpdated();
        }

        public RippingStatusViewModel AlbumsIdentified(IEnumerable<AlbumIdentification> albums)
        {
            _progress = null;
            _album = null;
            _albums = albums.ToList();

            OnAlbumIdentified(_drive, _albums);
            return StatusUpdated();
        }

        public RippingStatusViewModel AlbumSelected(string albumId)
        {
            _album = _albums.First(a => a.Id == albumId);
            _progress = (from t in _album.Tracks
                select new TrackProgress(t.TrackNumber)).ToList();

            OnAlbumSelected(_drive, _album);
            return StatusUpdated();
        }

        public void RippingProgress(int trackNumber, double percentageComplete)
        {
            _progress.First(t => t.TrackNumber == trackNumber).UpdatePercentageComplete(percentageComplete);
            OnRippingProgress(trackNumber, percentageComplete);
        }

        public RippingStatusViewModel GetStatus()
        {
            return _progress == null ? new RippingStatusViewModel(_albums) : new RippingStatusViewModel(_album, _progress);
        }

        public bool CanRip()
        {
            return _album != null;
        }
    }
}