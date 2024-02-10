namespace MyDisks.Domain.Authors;

public struct Pseudonym
{
    public string Value { get; set; }

    public Pseudonym(string value)
    {
        if (value.Length > 30)
        {
            throw new ArgumentException("Pseudonym cannot exceed 30 Characters");
        }

        Value = value;
    }

    public static explicit operator Pseudonym(string pseudonym)
    {
        return new Pseudonym(pseudonym);
    }

    public static implicit operator string(Pseudonym pseudonym)
    {
        return pseudonym.Value;
    }
}