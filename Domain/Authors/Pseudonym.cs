namespace MyDisks.Domain.Authors;

public class Pseudonym : ValueObject
{
    private string Value { get; }

    private const int MaxLength = 30;

    public Pseudonym(string value)
    {
        if (!IsLengthValid(value))
        {
            throw new ArgumentException($"Pseudonym cannot exceed {MaxLength} Characters");
        }

        this.Value = value;
    }

    public static explicit operator Pseudonym(string pseudonym)
    {
        return new Pseudonym(pseudonym);
    }

    public static implicit operator string(Pseudonym pseudonym)
    {
        return pseudonym.Value;
    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    private bool IsLengthValid(string value) => value.Length <= MaxLength;
}