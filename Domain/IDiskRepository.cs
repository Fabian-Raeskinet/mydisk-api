using System.Linq.Expressions;
using MyDisks.Domain.Disks;

namespace MyDisks.Domain;

public interface IDiskRepository
{
    /// <summary>
    /// Retrieves the list of disks asynchronously.
    /// </summary>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation. The task result contains
    /// an <see cref="IEnumerable{Disk}"/> that represents the list of disks retrieved.
    /// </returns>
    public Task<IEnumerable<Disk>?> GetDisksAsync();

    /// <summary>
    /// Retrieves a disk that matches the given filter asynchronously.
    /// </summary>
    /// <param name="predicate">The filter to apply to the disk collection.</param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the disk that matches the filter, or null if no disk is found.
    /// </returns>
    public Task<Disk?> GetDiskByFilterAsync(Expression<Func<Disk, bool>> predicate);

    /// <summary>
    /// Creates a disk asynchronously.
    /// </summary>
    /// <param name="disk">The disk to create.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the unique identifier of the created disk.</returns>
    public Task CreateDiskAsync(Disk disk);

    /// <summary>
    /// Deletes a disk asynchronously.
    /// </summary>
    /// <param name="disk">
    /// The disk object to be deleted.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation. The task will return true if the disk is successfully deleted; otherwise, it will return false.
    /// </returns>
    public Task DeleteDiskAsync(Disk disk);

    /// <summary>
    /// Updates the disk asynchronously.
    /// </summary>
    /// <param name="disk">The disk object to update.</param>
    /// <returns>A task that represents the asynchronous update operation.</returns>
    public Task UpdateDiskAsync(Disk disk);
}
