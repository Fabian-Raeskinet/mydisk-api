namespace MyDisks.Contracts.Authors;

public class CreateAuthorCommand
{
    public required string Pseudonym { get; set; }
    public DateTime Birthdate { get; set; }
}