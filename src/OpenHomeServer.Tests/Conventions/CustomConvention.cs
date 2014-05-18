using Fixie.Conventions;
using System;
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