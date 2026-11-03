using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeNChill.Functions.DTOs
{
    public class StaffDocumentResponse
    {
        public string FileName { get; set; } = string.Empty;

        public string Extension { get; set; } = string.Empty;

        public string Type { get; set; } = string.Empty;

        public long FileSize { get; set; }

        public DateTime UploadDate { get; set; }

        public string ContainerName { get; set; } = string.Empty;
    }
}
