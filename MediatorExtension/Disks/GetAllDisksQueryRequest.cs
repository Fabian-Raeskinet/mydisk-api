using MyDisk.Contracts.Disks;

namespace MediatorExtension.Disks;

public class GetAllDisksQueryRequest : Request<GetAllDisksQuery, List<DiskResponse>> { }