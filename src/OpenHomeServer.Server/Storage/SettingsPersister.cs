using System;
using System.IO;
using Newtonsoft.Json;

namespace OpenHomeServer.Server.Storage
{
    public class Settings
    {
        public Uri MusicCollectionRoot { get; set; }
    }

    public class Persister<T>
    {
        private readonly string _path;

        public Persister()
        {
            _path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "OpenHomeServer", string.Concat(typeof(T).Name, ".json"));
        }

        public T ReadFromDrive()
        {
            var text = File.ReadAllText(_path);
            return JsonConvert.DeserializeObject<T>(text);
        }

        public void FlushToDrive(T objectToPersist)
        {
            var json = JsonConvert.SerializeObject(objectToPersist);
            File.WriteAllText(_path, json);
        }
    }
}