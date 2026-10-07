using CoffeeNChill.Functions.DTOs;
using CoffeeNChill.Functions.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using System.Net;

namespace CoffeeNChill.Functions.Functions.Documents
{
    public class UploadStaffDocumentFunction
    {
        private readonly IFileStorageService _storage;

        //Maximum file size: 10 MB
        private const long MaxBytes = 10 * 1024 * 1024;

        //Allowed document extensions
        private static readonly HashSet<string> ValidExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".pdf",
            ".doc",
            ".docx"
        };

        public UploadStaffDocumentFunction(IFileStorageService storage)
        {
            _storage = storage;
        }

        [Function("UploadStaffDocument")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "documents")]
            HttpRequest request)
        {
            try
            {
                //Validate content type
                if (!request.HasFormContentType)
                {
                    return BadRequest("The request made should use multipart or form-data.");
                }

                var formData = await request.ReadFormAsync();
                var uploadedFile = formData.Files["file"];

                //Read uploaded form
                if (uploadedFile is null)
                {
                    return BadRequest("Upload a file using the form-data field that isnamed 'file'.");
                }

                //Validate file size
                if (uploadedFile.Length == 0)
                {
                    return BadRequest("The uploaded file is empty.");
                }

                if (uploadedFile.Length > MaxBytes)
                {
                    return BadRequest("The uploaded file exceeds the maximum allowed file size of 10MB.");
                }

                //Validate file name
                if (string.IsNullOrWhiteSpace(uploadedFile.FileName))
                {
                    return BadRequest("The uploaded file must have a valid file name.");
                }

                //Validate file extension
                var ext = Path.GetExtension(uploadedFile.FileName);
                if (!ValidExtensions.Contains(ext))
                {
                    return BadRequest("Invalid file type. Only PDF, DOC and DOCX files are allowed.");
                }

                var savedDoc = await _storage.UploadDocumentAsync(uploadedFile);

                //Convert the Model to ResponseDTO
                var payload = new StaffDocumentResponse
                {
                    FileName = savedDoc.FileName,
                    Extension = savedDoc.Extension,
                    Type = savedDoc.Type,
                    FileSize = savedDoc.FileSize,
                    UploadDate = savedDoc.UploadDate,
                    ContainerName = savedDoc.ContainerName
                };

                return new OkObjectResult(payload);
            }
            catch
            {
                return new ObjectResult(new
                {
                    message = "An unexpected error occurred while uploading the document."
                })
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError
                };
            }
        }

        private static BadRequestObjectResult BadRequest(string message) =>
            new(new { message });
    }
}