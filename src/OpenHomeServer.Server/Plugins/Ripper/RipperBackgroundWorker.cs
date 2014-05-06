using CdRipper.Rip;
using CdRipper.Tagging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Disposables;
using System.Threading;
using System.Threading.Tasks;
using OpenHomeServer.Server.Plugins.Notifications;
using Serilog;

namespace OpenHomeServer.Server.Plugins.Ripper
{
    public class DiscStatus
    {
        private bool _isInserted;
        public String Name { get; private set; }

        public DiscStatus(DriveInfo drive)
        {
            Name = drive.Name;
            _isInserted = drive.IsReady;
        }

        public bool HasChanged(DriveInfo newStatus)
        {
            if (newStatus.Name != Name)
            {
                throw new InvalidOperationException("Drives do not match");
            }

            var hasChanged = _isInserted != newStatus.IsReady;
            _isInserted = newStatus.IsReady;
            return hasChanged;
        }
    }

    public class DiscChangeObservable : IObservable<DriveInfo>
    {
        readonly IList<DiscStatus> _status;

        public DiscChangeObservable()
        {
            _status = (from d in DriveInfo.GetDrives()
                       where d.DriveType == DriveType.CDRom
                       select new DiscStatus(d)).ToList();
        }

        public IDisposable Subscribe(IObserver<DriveInfo> observer)
        {
            var cts = new CancellationTokenSource();
            var token = cts.Token;

            Task.Run(() =>
            {
                while (!token.IsCancellationRequested)
                {
                    try
                    {
                        var disc = (from d in DriveInfo.GetDrives()
                                    where d.DriveType == DriveType.CDRom
                                    join s in _status on d.Name equals s.Name
                                    where s.HasChanged(d)
                                    select d).FirstOrDefault();

                        if (disc != null)
                        {
                            observer.OnNext(disc);
                        }
                        Thread.Sleep(TimeSpan.FromSeconds(1));
                    }
                    catch (Exception e)
                    {
                        observer.OnError(e);
                    }
                }
            }, token);
            
            return Disposable.Create(cts.Cancel);
        }
    }

    public class RipperBackgroundWorker : IRunAtStartUp, IDisposable
    {
        private readonly IDisposable _observer;

        public RipperBackgroundWorker(RipperService ripperService, ILogger logger)
        {
            _observer = new DiscChangeObservable().Subscribe(async disc =>
            {
                await Task.Run(() =>
                {
                    if (disc.IsReady)
                    {
                        switch (disc.DriveFormat)
                        {
                            case "CDFS":
                                ripperService.OnDiscInsertion(disc);
                                break;
                            case "UDF":
                                logger.Information("A DVD of BluRay had been inserted. This is not yet supported.");
                                break;
                            default:
                                logger.Information("An unknown disc format has been inserted in drive {drivename}: {driveformat}", disc.Name, disc.DriveFormat);
                                break;
                        }
                    }
                    else
                    {
                        ripperService.OnDiscEjected(disc);
                    }
                });                    
            });
        }

        public void Dispose()
        {
            if (_observer != null) _observer.Dispose();
        }
    }    
}
