using Microsoft.AspNetCore.Http;

namespace ExpenseSplit.Application.ServiceInterfaces;

public interface IFileService : IDependencyMarkerService
{
    Task<string> SaveFileAsync(IFormFile file,string fileType,string folderId);
}
