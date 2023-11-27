using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using MyDisks.Api.Maintenance;

namespace MyDisks.Api.Filters;

public class MaintenanceFilterAttribute : ActionFilterAttribute, IResourceFilter
{
    public void OnResourceExecuting(ResourceExecutingContext context)
    {
        if (context.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor
            && controllerActionDescriptor.MethodInfo.CustomAttributes.Any(a
                => a.AttributeType == typeof(IgnoreMaintenanceFilterAttribute)))
            return;

        if (context.HttpContext.RequestServices.GetService(typeof(IOptions<MaintenanceSettings>)) is not
            IOptions<MaintenanceSettings> maintenanceSettings)
        {
            context.Result = new ServiceUnavailableResult();
            return;
        }

        if (MaintenanceHelper.IsMaintenance(maintenanceSettings) == MaintenanceStatus.Inactive)
            return;

        context.Result = new ServiceUnavailableResult();
    }

    public void OnResourceExecuted(ResourceExecutedContext context)
    {
    }
}