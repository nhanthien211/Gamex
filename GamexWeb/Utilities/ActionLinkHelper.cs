using System.Web.Mvc;

namespace GamexWeb.Utilities
{
    public static class ActionLinkHelper
    {
        public static MvcHtmlString If(this MvcHtmlString value, bool evaluation)
        {
            return evaluation ? value : MvcHtmlString.Empty;
        }
    }
}