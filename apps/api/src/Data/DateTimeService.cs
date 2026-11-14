using MyDisks.Domain;

namespace MyDisks.Data;

public class DateTimeService : IDateTime
{
    public DateTime Now => DateTime.Now;
}