using Documentation.Web.DependencyResolution;
using StructureMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Documentation.Web.Helper
{
    public class DocumentationContainer : Container
    {

        public DocumentationContainer(Action<ConfigurationExpression> cfg)
            : base(cfg)
        {
        }
    }
    public static class DocumentationContainerHelper
    {
        private const string CacheKey = "DocumentationDependencyContainer";

        private static DocumentationContainer _container;

        private static DocumentationContainer PermenantContainer => _container ?? (_container = Build());

        public static DocumentationContainer Build()
        {
            var container = new DocumentationContainer(cfg =>
            {
                cfg.AddRegistry<DefaultRegistry>();
            });

            return container;
        }

        public static IContainer Container => HttpContext.Current == null ? PermenantContainer.GetNestedContainer() : GetContextFromContainer();

        private static IContainer GetContextFromContainer()
        {
            var currentContext = HttpContext.Current;
            var container = currentContext.Items[CacheKey] as IContainer;
            if (container == null)
                currentContext.Items[CacheKey] = container = PermenantContainer.GetNestedContainer();
            return container;
        }
    }
}