using OpenHomeServer.Server.Storage;
using Shouldly;
using System;
using System.IO;

namespace OpenHomeServer.Tests.Storage
{
    public class PersisterTests : IDisposable
    {
        private static readonly string ApplicationName = "OpenHomeServer.Tests";
        private static readonly string ExpectedFileName = "SomeClassToPersist.json";
        private readonly string ExpectedPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ApplicationName);        

        private Persister<SomeClassToPersist> CreatePersister()
        {
            return new Persister<SomeClassToPersist>(ApplicationName);
        }

        public void ShouldCreateAFileInTheRoamingProfile()
        {
            Directory.Exists(ExpectedPath).ShouldBe(false);
            File.Exists(Path.Combine(ExpectedPath, ExpectedFileName)).ShouldBe(false);

            CreatePersister();

            Directory.Exists(ExpectedPath).ShouldBe(true);
            File.Exists(Path.Combine(ExpectedPath, ExpectedFileName)).ShouldBe(true);
        }

        public void GettingAnUnsavedClassReturnsDefaultValues()
        {
            var persister = CreatePersister();
            var settings = persister.Get();

            settings.ShouldNotBe(null);
            settings.SomeString.ShouldBe(default(string));
        }

        public void SettingValuesShouldPersistThem()
        {
            var persister = CreatePersister();
            persister.Save(new SomeClassToPersist
            {
                SomeString = "test"
            });

            var settings = persister.Get();
            settings.SomeString.ShouldBe("test");
        }

        public void Dispose()
        {
            File.Delete(Path.Combine(ExpectedPath, ExpectedFileName));
            Directory.Delete(ExpectedPath);
        }

        private class SomeClassToPersist
        {
            public string SomeString { get; set; }
        }
    }    
}
