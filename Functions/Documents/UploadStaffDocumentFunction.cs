using CoffeeNChill.Functions.DTOs;
using CoffeeNChill.Functions.Interfaces;
using CoffeeNChill.Functions.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using System.Net;

namespace CoffeeNChill.Functions.Functions.Documents
{
    public class UploadStaffDocumentFunction
    {
        private readonly IFileStorageService _fileStorageService;

        // Maximum file size: 10 MB
        private const long MaxFileSize = 10 * 1024 * 1024;

        // Allowed document extensions
        private static readonly string[] AllowedExtensions =
        {
            ".pdf",
            ".doc",
            ".docx"
        };

        public UploadStaffDocumentFunction(IFileStorageService fileStorageService)
        {
            _fileStorageService = fileStorageService;
        }

        [Function("UploadStaffDocument")]
        public async Task<IActionResult> Run(
            [HttpTrigger(
                AuthorizationLevel.Anonymous,
                "post",
                Route = "documents")]
            HttpRequest req)
        {
            try
            {
                // --------------------------------------------------
                // 1. Validate content type
                // --------------------------------------------------

                if (!req.HasFormContentType)
                {
                    return new BadRequestObjectResult(new
                    {
                        message = "The request must use multipart/form-data."
                    });
                }

                // --------------------------------------------------
                // 2. Read uploaded form
                // --------------------------------------------------

                var form = await req.ReadFormAsync();

                IFormFile? file = form.Files["file"];

                // --------------------------------------------------
                // 3. Validate that a file was supplied
                // --------------------------------------------------

                if (file == null)
                {
                    return new BadRequestObjectResult(new
                    {
                        message = "Please upload a file using the form-data field named 'file'."
                    });
                }

                // --------------------------------------------------
                // 4. Validate file size
                // --------------------------------------------------

                if (file.Length == 0)
                {
                    return new BadRequestObjectResult(new
                    {
                        message = "The uploaded file is empty."
                    });
                }

                if (file.Length > MaxFileSize)
                {
                    return new BadRequestObjectResult(new
                    {
                        message = "The uploaded file exceeds the maximum allowed size of 10 MB."
                    });
                }

                // --------------------------------------------------
                // 5. Validate file name
                // --------------------------------------------------

                if (string.IsNullOrWhiteSpace(file.FileName))
                {
                    return new BadRequestObjectResult(new
                    {
                        message = "The uploaded file must have a valid file name."
                    });
                }

                // --------------------------------------------------
                // 6. Validate file extension
                // --------------------------------------------------

                string extension = Path.GetExtension(file.FileName).ToLowerInvariant();

                if (!AllowedExtensions.Contains(extension))
                {
                    return new BadRequestObjectResult(new
                    {
                        message = "Invalid file type. Only PDF, DOC and DOCX files are allowed."
                    });
                }

                // --------------------------------------------------
                // 7. Upload the document
                // --------------------------------------------------

                StaffDocument document = await _fileStorageService.UploadDocumentAsync(file);

                // --------------------------------------------------
                // 8. Convert Model to Response DTO
                // --------------------------------------------------

                var responseDto = new StaffDocumentResponse
                {
                    FileName = document.FileName,
                    Extension = document.Extension,
                    Type = document.Type,
                    FileSize = document.FileSize,
                    UploadDate = document.UploadDate,
                    ContainerName = document.ContainerName
                };

                // --------------------------------------------------
                // 9. Return successful response
                // --------------------------------------------------

                return new OkObjectResult(responseDto);
            }
            catch (Exception ex)
            {
                // --------------------------------------------------
                // 10. Handle unexpected errors
                // --------------------------------------------------

                return new ObjectResult(new
                {
                    message = "An unexpected error occurred while uploading the document."
                })
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError
                };
            }
        }
    }
}

