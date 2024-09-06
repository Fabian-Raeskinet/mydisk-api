using MyDisks.Domain.Authors;

namespace MyDisks.Services.Authors;

public class CreateAuthorCommandHandler : ICommandHandler<CreateAuthorCommandRequest>
{
    public CreateAuthorCommandHandler(IAuthorRepository authorRepository)
    {
        AuthorRepository = authorRepository;
    }

    public IAuthorRepository AuthorRepository { get; set; }

    public Task Handle(CreateAuthorCommandRequest request, CancellationToken cancellationToken)
    {
        var author = new Author { Pseudonym = new Pseudonym(request.Pseudonym), Birthdate = request.Birthdate };
        return AuthorRepository.AddAsync(author);
    }
}