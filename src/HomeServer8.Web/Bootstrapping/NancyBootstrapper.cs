using Nancy;
using Nancy.Conventions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace HomeServer8.Web.Bootstrapping
{
    public class NancyBootstrapper : DefaultNancyBootstrapper
    {
        protected override IRootPathProvider RootPathProvider
        {
            get
            {
                return new CustomRootPathProvider();
            }
        }

        protected override void ConfigureConventions(NancyConventions nancyConventions)
        {
            base.ConfigureConventions(nancyConventions);
            nancyConventions.StaticContentsConventions.Add(
                StaticContentConventionBuilder.AddDirectory("Scripts", @"Scripts")                
            );            
        }
    }

    public class CustomRootPathProvider : IRootPathProvider
    {
        readonly string _rootPath;

        public CustomRootPathProvider()
        {
            _rootPath = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).Parent.EnumerateDirectories("HomeServer8").First().FullName;
        }

        public string GetRootPath()
        {
            return _rootPath;
        }
    }
}