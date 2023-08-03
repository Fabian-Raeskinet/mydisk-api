using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace MyDisks.Api.Maintenance;

[DefaultStatusCode(StatusCodes.Status503ServiceUnavailable)]
public class ServiceUnavailableResult : StatusCodeResult
{
    public ServiceUnavailableResult() : base(StatusCodes.Status503ServiceUnavailable)
    {
    }
}