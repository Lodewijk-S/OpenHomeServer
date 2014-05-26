﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace OpenHomeServer.Runner
{
    //With inspitation from https://github.com/jbogard/NServiceBus.MessageRouting/blob/master/src/ServiceHost/Program.cs
    class Program
    {
        private const string BinPath = @"..\..\..\OpenHomeServer.Server\bin\Debug";
        private static Timer _reloadTimer;
        private static readonly List<FileSystemWatcher> Watchers = new List<FileSystemWatcher>();
        private static readonly object Sync = new Object();
        private static AppDomain _domain;


        static void Main(string[] args)
        {
            LoadServices();

            while (ProcessKey(Console.ReadKey().KeyChar))
            {
            }

            UnloadServices();
        }

        private static bool ProcessKey(char keyChar)
        {
            switch (keyChar)
            {
                case '.':
                    return false;
                case 'r':
                    Console.WriteLine("Recycling services...");
                    if (_reloadTimer != null)
                    {
                        _reloadTimer.Dispose();
                        _reloadTimer = null;
                    }
                    UnloadServices();
                    LoadServices();
                    break;
            }
            return true;
        }

        private static void LoadServices()
        {
            Watchers.Add(CreateWatcher(BinPath, "*.dll"));
            Watchers.Add(CreateWatcher(BinPath, "*.config"));

            var domainInfo = new AppDomainSetup
            {
                ConfigurationFile = "OpenHomeServer.Server.exe.config",
                ApplicationBase = BinPath,
                ShadowCopyFiles = "true",
                ApplicationName = "OpenHomeServer",
            };

            _domain = AppDomain.CreateDomain("OpenHomeServer", null, domainInfo);

            var assemblyName = AssemblyName.GetAssemblyName(BinPath + @"\OpenHomeServer.Server.exe");
            Task.Run(() =>
            {
                try
                {
                    _domain.ExecuteAssemblyByName(assemblyName);
                }catch(AppDomainUnloadedException){}
            });
            
        }

        private static void UnloadServices()
        {
            Console.WriteLine("Unloading services...");
            foreach (var watcher in Watchers)
            {
                watcher.EnableRaisingEvents = false;
                watcher.Dispose();
            }
            Watchers.Clear();
            
            Task.Run(() =>UnloadAppDomain(_domain)).Wait();

            _domain = null;
        }

        private static void UnloadAppDomain(AppDomain domain)
        {
            int idx = 1;
            const int maxTries = 10;
            while (true)
            {
                try
                {
                    AppDomain.Unload(domain);
                    break;
                }
                catch (CannotUnloadAppDomainException)
                {
                    Console.WriteLine("Could not unload AppDomain '{0}' at this time.", domain.FriendlyName);
                    if (++idx == maxTries)
                        break;
                    Console.WriteLine("Trying again ({0}/{1})...", idx, maxTries);
                }
            }
        }

        private static FileSystemWatcher CreateWatcher(string path, string filter)
        {
            var watcher = new FileSystemWatcher
            {
                Path = path,
                IncludeSubdirectories = true,
                Filter = filter,
                NotifyFilter =
                    NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.CreationTime |
                    NotifyFilters.Size
            };
            watcher.Changed += OnDirectoryChanged;
            watcher.Created += OnDirectoryChanged;
            watcher.Error += OnError;
            watcher.EnableRaisingEvents = true;
            return watcher;
        }

        private static void OnError(object sender, ErrorEventArgs e)
        {
            Console.WriteLine("FilesystemWatcher error {0}", e.GetException().Message);
        }

        private static void OnDirectoryChanged(object sender, FileSystemEventArgs e)
        {
            lock (Sync)
            {   
                if (_reloadTimer == null)
                {
                    Console.WriteLine("Filesystem change detected, queueing restart...");
                    _reloadTimer = new Timer(OnReloadTimerElapsed, null, TimeSpan.FromSeconds(10),
                                             TimeSpan.FromMilliseconds(-1));
                }
            }
        }

        private static void OnReloadTimerElapsed(object state)
        {
            try
            {
                UnloadServices();
                LoadServices();
            }
            finally
            {
                _reloadTimer = null;
            }
        }
    }
}
