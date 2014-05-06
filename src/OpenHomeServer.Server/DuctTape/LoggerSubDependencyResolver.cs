using Castle.Core;
using Castle.MicroKernel;
using Castle.MicroKernel.Context;
using Serilog;

namespace OpenHomeServer.Server.DuctTape
{
    public class LoggerSubDependencyResolver : ISubDependencyResolver
    {
        public bool CanResolve(CreationContext context, ISubDependencyResolver contextHandlerResolver, ComponentModel model, DependencyModel dependency)
        {
            return dependency.TargetType == typeof(ILogger);
        }

        public object Resolve(CreationContext context, ISubDependencyResolver contextHandlerResolver, ComponentModel model, DependencyModel dependency)
        {
            if (CanResolve(context, contextHandlerResolver, model, dependency))
            {
                if (dependency.TargetType == typeof(ILogger))
                {
                    return Log.ForContext(model.Implementation.GetType());
                }
            }
            return null;
        }
    }
}
