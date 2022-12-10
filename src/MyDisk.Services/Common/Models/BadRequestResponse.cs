namespace MyDisk.Services.Common.Models
{
    public class BadRequestResponse
    {
        public BadRequestError? Error { get; set; }
    }

    public class BadRequestError
    {
        public string[]? Messages { get; set; }
    }

}
