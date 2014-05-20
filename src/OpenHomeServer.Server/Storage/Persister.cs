using AspectCache;
using Newtonsoft.Json;
using System;
using System.IO;

namespace OpenHomeServer.Server.Storage
{
    public class Persister<T>
        where T: new()
    {
        private readonly string _path;

        public Persister(string applicationName)
        {
            _path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), applicationName, string.Concat(typeof(T).Name, ".json"));            
        }

        [Cached]
        public virtual T Get()
        {
            if (File.Exists(_path))
            {
                var text = File.ReadAllText(_path);
                return JsonConvert.DeserializeObject<T>(text);
            }
            return new T();            
        }

        [BustCache]
        public virtual void Save(T objectToSave)
        {
            var directory = Path.GetDirectoryName(_path);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var json = JsonConvert.SerializeObject(objectToSave);
            File.WriteAllText(_path, json);
        }
    }
} 