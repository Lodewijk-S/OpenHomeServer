using System;
using Fixie;

namespace OpenHomeServer.Tests.Conventions
{
    public class CustomConvention : Convention
    {
        public CustomConvention()
        {
            Classes
                .NameEndsWith("Tests");

            HideExceptionDetails
                .For<Exception>()
                .For(typeof(Shouldly.ShouldBeStringTestExtensions));
        }   
    }
}