namespace Chub.ApiExplorer.Web.Extensions
{
    using System;
    using Microsoft.AspNetCore.Mvc;

    public static class ControllerExtensions
    {
        public static string GetControllerName(this Controller controller)
        {
            string name = controller.GetType().Name;

            if (name.EndsWith("Controller"))
            {
                int index = name.LastIndexOf("Controller", StringComparison.InvariantCultureIgnoreCase);
                name = name.Substring(0, index);
            }

            return name;
        }
    }
}
