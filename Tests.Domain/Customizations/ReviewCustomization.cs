using AutoFixture;
using MyDisks.Domain.Authors;
using MyDisks.Domain.Reviews;

namespace MyDisks.Tests.Domain.Customizations;

public class ReviewCustomization: ICustomization
{
    public void Customize(IFixture fixture)
    {
        fixture.Customize<Review>(r => r
            .With(x => x.Title, Guid.NewGuid().ToString().Substring(30))
            .With(x => x.Status, ReviewStatus.Pending));
    }
}