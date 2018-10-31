using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Voidwell.FileWell.Data.Models;

namespace Voidwell.FileWell.Data.Repositories
{
    public class FileRepository : IFileRepository
    {
        private readonly FileDbContext _dbContext;

        public FileRepository(FileDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<FileRecord> GetFile(Guid fileId)
        {
            return _dbContext.Files.AsNoTracking()
                .FirstOrDefaultAsync(a => a.FileId == fileId);
        }

        public async Task<FileRecord> CreateFile(FileRecord record)
        {
            _dbContext.Files.Add(record);
            await _dbContext.SaveChangesAsync();

            return record;
        }

        public async Task<FileRecord> UpdateFile(FileRecord record)
        {
            _dbContext.Files.Update(record);
            await _dbContext.SaveChangesAsync();

            return record;
        }
    }
}
