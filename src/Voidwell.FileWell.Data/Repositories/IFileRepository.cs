using System;
using System.Threading.Tasks;
using Voidwell.FileWell.Data.Models;

namespace Voidwell.FileWell.Data.Repositories
{
    public interface IFileRepository
    {
        Task<FileRecord> GetFile(Guid fileId);
        Task<FileRecord> CreateFile(FileRecord record);
        Task<FileRecord> UpdateFile(FileRecord record);
    }
}