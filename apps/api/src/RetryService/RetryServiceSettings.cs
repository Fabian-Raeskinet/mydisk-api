namespace MyDisks.RetryService;

public sealed class RetryServiceSettings
{
    public int RetryCount { get; set; }
    public int TimeRetryMilliSeconds { get; set; }
}