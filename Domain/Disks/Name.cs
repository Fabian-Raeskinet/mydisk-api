namespace MyDisks.Domain.Disks;

public struct Name
{
    public string Value { get; }

    public Name(string value)
    {
        if (value.Length > 30)
        {
            throw new ArgumentException("Name cannot exceed 30 characters");
        }
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
}