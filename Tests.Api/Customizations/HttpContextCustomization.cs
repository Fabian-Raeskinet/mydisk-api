using AutoFixture;
using Microsoft.AspNetCore.Http;
using Moq;

namespace MyDisks.Tests.Api.Customizations;

public class HttpContextCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        
        fixture.Customize<Mock<HttpContext>>(builder => builder.Do(c => c.Setup(m => m.RequestServices)
            .Returns(() => fixture
                .Create<Mock<IServiceProvider>>()
                .Object)));
    }
}