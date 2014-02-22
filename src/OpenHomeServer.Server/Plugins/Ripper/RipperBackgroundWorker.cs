using OpenHomeServer.Server.Messaging;
using OpenHomeServer.Server.Messaging.Hubs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Disposables;
using System.Threading;
using System.Threading.Tasks;

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

        public bool IsInserted(DriveInfo newStatus)
        {
            if (newStatus.Name != Name)
            {
                throw new InvalidOperationException("Drives do not match");
            }

            var hasChanged = _isInserted != newStatus.IsReady && newStatus.IsReady;
            _isInserted = newStatus.IsReady;
            return hasChanged;
        }
    }

    public class DiscInsertedObservable : IObservable<DriveInfo>
    {
        IList<DiscStatus> _status;

        public DiscInsertedObservable()
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
                                    where s.IsInserted(d)
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
        private IDisposable _observer;
        public RipperBackgroundWorker(HubContextFactory<MessagingHub> factory)
        {   
            _observer = new DiscInsertedObservable().Subscribe(async disc => 
            {
                await Task.Run(() => 
                { 
                    //Todo: Rip CD
                    switch(disc.DriveFormat)
                    {
                        case "CDFS":
                            Console.WriteLine("this is a cd");
                            break;
                        case "UDF":
                            Console.WriteLine("this is a dvd");
                            break;
                        default:
                            Console.WriteLine("Dont know: " + disc.DriveFormat);
                            break;
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
