using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Voidwell.FileWell.Data.Models
{
    [Table("File")]
    public class FileRecord
    {
        [Key]
        public Guid FileId { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
        public string FileMimeType { get; set; }
        public uint FileSize { get; set; } = 0;
        public List<string> Tags { get; set; }
        public Guid UploadUserId { get; set; }
        public DateTime UploadedDate { get; set; }
        public Guid? DeleteUserId { get; set; }
        public DateTime? DeletedDate { get; set; }
        public bool? IsDeleted { get; set; }
    }
}
