namespace MyDisks.Domain.Disks;

public class Name : ValueObject
{
    public string Value { get; }
    private const int MaxLength = 30;

    public Name(string value)
    {
        if (string.IsNullOrEmpty(value))
            throw new ArgumentNullException(nameof(value));

        if (!IsLengthValid(value))
            throw new ArgumentException($"Name cannot exceed {MaxLength} characters");

        Value = value;
    }

    public static explicit operator Name(string name)
    {
        return new Name(name);
    }

    public static implicit operator string(Name name)
    {
        return name.Value;
    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    private bool IsLengthValid(string value)
    {
        return value.Length <= MaxLength;
    }
}