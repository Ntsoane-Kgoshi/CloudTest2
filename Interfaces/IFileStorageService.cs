using CoffeeNChill.Functions.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeNChill.Functions.Interfaces
{
    public interface IFileStorageService
    {
        Task<List<StaffDocument>> GetAllDocumentsAsync();

        Task<StaffDocument> UploadDocumentAsync(IFormFile file);

        Task<Stream?> DownloadDocumentAsync(string fileName);

        Task<bool> DeleteDocumentAsync(string fileName);

    }
}
