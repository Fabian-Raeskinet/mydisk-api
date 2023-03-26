using AutoFixture;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MyDisk.Tests.Api.Customizations;

public class ExceptionContextCustomization : ICustomization
{
    public void Customize(IFixture fixture) =>
        fixture.Customize<ExceptionContext>(c =>
            c.FromFactory(() => new ExceptionContext(fixture.Create<ActionContext>(), new List<IFilterMetadata>())));
}