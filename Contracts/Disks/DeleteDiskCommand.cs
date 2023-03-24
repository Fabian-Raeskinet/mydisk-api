namespace MyDisk.Contracts.Disks;

public class DeleteDiskCommand
{
    public DeleteDiskByProperty Property { get; set; }
    public string? Value { get; set; }
}