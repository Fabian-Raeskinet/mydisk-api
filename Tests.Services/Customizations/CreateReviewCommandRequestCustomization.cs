using AutoFixture;
using MyDisks.Services.Reviews;

namespace MyDisks.Tests.Services.Customizations;

public class CreateReviewCommandRequestCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        fixture.Customize<CreateReviewCommandRequest>(r => r
            .With(x => x.Title, Guid.NewGuid().ToString().Substring(30))
            .With(x => x.Note, 2.5));
    }
}