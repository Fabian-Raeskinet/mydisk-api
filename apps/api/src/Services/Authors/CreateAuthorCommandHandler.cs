using MyDisks.Domain.Authors;

namespace MyDisks.Services.Authors;

public class CreateAuthorCommandHandler : ICommandHandler<CreateAuthorCommandRequest>
{
    public CreateAuthorCommandHandler(IUnitOfWork unitOfWork)
    {
        UnitOfWork = unitOfWork;
    }

    private IUnitOfWork UnitOfWork { get; }

    public Task Handle(CreateAuthorCommandRequest request, CancellationToken cancellationToken)
    {
        var author = new Author { Pseudonym = new Pseudonym(request.Pseudonym), Birthdate = request.Birthdate };
        UnitOfWork.AuthorRepository.AddAsync(author);
        return UnitOfWork.CommitAsync();
    }
}