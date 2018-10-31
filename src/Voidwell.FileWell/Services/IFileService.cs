using System;
using System.Threading.Tasks;
using Voidwell.FileWell.Data.Models;

namespace Voidwell.FileWell.Services
{
    public interface IFileService
    {
        Task<FileRecord> GetFile(Guid fileId);
        Task<FileRecord> UploadFile(FileRecord file, Guid userId);
        Task<bool> DeleteFile(Guid fileId, Guid userId);
    }
}