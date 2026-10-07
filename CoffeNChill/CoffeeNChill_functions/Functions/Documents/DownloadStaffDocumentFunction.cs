using CoffeeNChill.Functions.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Net;

namespace CoffeeNChill.Functions.Functions.StaffDocuments
{
    public class DownloadStaffDocumentFunction
    {
        private readonly IFileStorageService _storage;

        public DownloadStaffDocumentFunction(IFileStorageService storage)
        {
            _storage = storage;
        }

        [Function("DownloadStaffDocument")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "documents/download/{fileName}")]
            HttpRequestData request,
            string fileName)
        {
            try
            {
                //Ensure a file name was supplied
                if (string.IsNullOrWhiteSpace(fileName))
                {
                    var badRequest = request.CreateResponse(HttpStatusCode.BadRequest);
                    await badRequest.WriteAsJsonAsync(new
                    {
                        message = "A file name is required."
                    });
                    return badRequest;
                }

                //Attempt to retrieve the document from storage
                Stream? documentStream = await _storage.DownloadDocumentAsync(fileName);

                //Return message when the document does not exist
                if (documentStream is null)
                {
                    var notFound = request.CreateResponse(HttpStatusCode.NotFound);
                    await notFound.WriteAsJsonAsync(new
                    {
                        message = "The requested document was not found."
                    });
                    return notFound;
                }

                //Build a successful download response
                var successResponse = request.CreateResponse(HttpStatusCode.OK);
                successResponse.Headers.Add("Content-Type", ResolveContentType(fileName));
                successResponse.Headers.Add("Content-Disposition", $"attachment; filename=\"{fileName}\"");

                //Stream the file content 
                await documentStream.CopyToAsync(successResponse.Body);
                await documentStream.DisposeAsync();

                return successResponse;
            }
            catch
            {
                //Failure response
                var errorResponse = request.CreateResponse(HttpStatusCode.InternalServerError);
                await errorResponse.WriteAsJsonAsync(new
                {
                    message = "An unexpected error occurred while retrieving the document."
                });
                return errorResponse;
            }
        }

        //Maps a file extension
        private static string ResolveContentType(string fileName)
        {
            string extension = Path.GetExtension(fileName).ToLowerInvariant();

            return extension switch
            {
                ".pdf" => "application/pdf",
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".xls" => "application/vnd.ms-excel",
                ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                ".ppt" => "application/vnd.ms-powerpoint",
                ".pptx" => "application/vnd.openxmlformats-officedocument.presentationml.presentation",
                ".txt" => "text/plain",
                ".csv" => "text/csv",
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                _ => "application/octet-stream"
            };
        }
    }
}