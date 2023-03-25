namespace MyDisk.RetryService;

public sealed class RetryServiceSettings
{
    public int MaxRetry { get; set; }
    public int PolicyTimeRetryMilliSeconds { get; set; }
}