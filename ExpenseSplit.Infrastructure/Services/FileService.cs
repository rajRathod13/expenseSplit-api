using ExpenseSplit.Application.ServiceInterfaces;
using ExpenseSplit.Common.ConfigurationSettings;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace ExpenseSplit.Infrastructure.Services;

public class FileService : IFileService
{
    private readonly FileSettings _fileSettings;
    public FileService(IOptions<FileSettings> fileSettings) {
        _fileSettings = fileSettings.Value;
    }
    public async Task<string> SaveFileAsync(IFormFile file, string fileType, string folderId)
    {
        if (file == null || file.Length == 0)
            return string.Empty;

        string folderPath = Path.Combine(_fileSettings.WebRootPath, _fileSettings.BaseFolder, fileType, folderId);
        if(Directory.Exists(folderPath))
            Directory.Delete(folderPath, true);

        Directory.CreateDirectory(folderPath);
        var fileName = Path.GetRandomFileName() + Path.GetExtension(file.FileName);
        var fullPath = Path.Combine(folderPath, fileName);

        using var stream = new FileStream(fullPath, FileMode.Create);
        await file.CopyToAsync(stream);

        var relativePath = Path.Combine(_fileSettings.BaseFolder,fileType,folderId,fileName).Replace("\\","/");
        return relativePath;
    }
}
