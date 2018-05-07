using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Documentation.Web.Helper
{
    public class DocumentationControllerFactory : DefaultControllerFactory
    {
        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            return DocumentationContainerHelper.Container.GetInstance(controllerType) as Controller;
        }
    }
}