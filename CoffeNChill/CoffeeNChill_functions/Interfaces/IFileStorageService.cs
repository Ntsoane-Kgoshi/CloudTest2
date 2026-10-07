using CoffeeNChill.Functions.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Handles all persistence for staff documents against the "staff-docs"
/// Azure File Share. Kept behind an interface so the HTTP-triggered
/// Functions depend only on this contract, not on
/// Azure.Storage.Files.Shares directly.
/// </summary>


namespace CoffeeNChill.Functions.Interfaces
{
    /// <summary> /// Provides methods for managing staff documents in Azure File Storage. /// </summary>
    public interface IFileStorageService
    {
        /// <summary> /// Gets all staff documents. /// </summary>
        Task<List<StaffDocument>> GetAllDocumentsAsync();
        /// <summary> /// Uploads a staff document. /// </summary>
        Task<StaffDocument> UploadDocumentAsync(IFormFile file);
        /// <summary> /// Downloads a staff document by file name. /// </summary>
        Task<Stream?> DownloadDocumentAsync(string fileName);
        /// <summary> /// Deletes a staff document by file name. /// </summary>
        Task<bool> DeleteDocumentAsync(string fileName);

    }
}
