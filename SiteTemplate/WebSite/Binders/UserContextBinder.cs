using System.Web.Mvc;

namespace WebSite
{
    public class UserContextBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            return controllerContext.HttpContext.Session[SysContext.ConstKeys.Session_UserContext] as UserContext;
        }
    }
}