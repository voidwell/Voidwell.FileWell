using System;
using System.Threading.Tasks;
using Voidwell.FileWell.Data.Models;
using Voidwell.FileWell.Data.Repositories;

namespace Voidwell.FileWell.Services
{
    public class FileService : IFileService
    {
        private IFileRepository _repository;

        public FileService(IFileRepository fileRepository)
        {
            _repository = fileRepository;
        }

        public Task<FileRecord> GetFile(Guid fileId)
        {
            return _repository.GetFile(fileId);
        }

        public Task<FileRecord> UploadFile(FileRecord file, Guid userId)
        {
            file.UploadUserId = userId;

            return _repository.CreateFile(file);
        }

        public async Task<bool> DeleteFile(Guid fileId, Guid userId)
        {
            var file = await GetFile(fileId);
            if (file == null)
            {
                return false;
            }

            file.IsDeleted = true;
            file.DeletedDate = DateTime.UtcNow;
            file.DeleteUserId = userId;

            await _repository.UpdateFile(file);

            return true;
        }
    }
}
