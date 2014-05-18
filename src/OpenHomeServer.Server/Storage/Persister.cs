using System;
using System.IO;
using Newtonsoft.Json;

namespace OpenHomeServer.Server.Storage
{
    public class Persister<T>
        where T : new()
    {
        private static readonly object Door = new object();
        private readonly string _path;
        

        public Persister(string applicationName)
        {
            _path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), applicationName, string.Concat(typeof(T).Name, ".json"));
            if (!File.Exists(_path))
            {
                var directory = Path.GetDirectoryName(_path);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                FlushToDrive(new T());
            }            
            SetReadOnlyCopy(ReadFromDrive());
        }

        private T _readonlyCopy;
        private void SetReadOnlyCopy(T value)
        {
            lock (Door) { _readonlyCopy = value; }
        }

        public void Reset()
        {
            FlushToDrive(new T());
        }

        public T GetValue()
        {
            return _readonlyCopy;
        }

        public void OpenForChanging(Action<T> changeObject)
        {
            var objectToPersist = ReadFromDrive();
            changeObject(objectToPersist);
            FlushToDrive(objectToPersist);
        }

        private T ReadFromDrive()
        {
            var text = File.ReadAllText(_path);
            return JsonConvert.DeserializeObject<T>(text);
        }

        private void FlushToDrive(T objectToPersist)
        {
            var json = JsonConvert.SerializeObject(objectToPersist);
            File.WriteAllText(_path, json);
            SetReadOnlyCopy(objectToPersist);
        }
    }
}