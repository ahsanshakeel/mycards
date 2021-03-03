using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyCard.Web.Attributes
{
    public class ErrorHandlerAttribute: HandleErrorAttribute
    {
        ILogger logger;
        public ErrorHandlerAttribute()
        {
            this.logger = LogManager.GetCurrentClassLogger();
        }
        public override void OnException(ExceptionContext filterContext)
        {
            Exception ex = filterContext.Exception;
            logger.Log(LogLevel.Error, ex);
            filterContext.ExceptionHandled = true;
            HandleErrorInfo model = new HandleErrorInfo(filterContext.Exception, "Controller", "Action");

            filterContext.Result = new ViewResult()
            {
                ViewName = "Error1",
                ViewData = new ViewDataDictionary(model)
            };
        }
    }
}