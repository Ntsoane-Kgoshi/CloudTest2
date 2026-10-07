using CoffeeNChill.Functions.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Net;

namespace CoffeeNChill.Functions.Functions.StaffDocuments
{
    public class DeleteStaffDocumentFunction
    {
        private readonly IFileStorageService _storage;

        public DeleteStaffDocumentFunction(IFileStorageService storage)
        {
            _storage = storage;
        }

        [Function("DeleteStaffDocument")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "documents/{fileName}")]
            HttpRequestData request,
            string fileName)
        {
            try
            {
                //Ensure a file name was provided 
                if (string.IsNullOrWhiteSpace(fileName))
                {
                    var badRequest = request.CreateResponse(HttpStatusCode.BadRequest);
                    await badRequest.WriteAsJsonAsync(new
                    {
                        error = "A file name is required."
                    });
                    return badRequest;
                }

                //Attempt to remove the document from storage
                bool wasDeleted = await _storage.DeleteDocumentAsync(fileName);

                //Return message when the document does not exist
                if (!wasDeleted)
                {
                    var notFound = request.CreateResponse(HttpStatusCode.NotFound);
                    await notFound.WriteAsJsonAsync(new
                    {
                        error = $"Document '{fileName}' was not found."
                    });
                    return notFound;
                }

                //Build and return a successful response
                var successResponse = request.CreateResponse(HttpStatusCode.OK);
                await successResponse.WriteAsJsonAsync(new
                {
                    message = $"Document '{fileName}' was deleted successfully."
                });
                return successResponse;
            }
            catch
            {
                //Failure response
                var errorResponse = request.CreateResponse(HttpStatusCode.InternalServerError);
                await errorResponse.WriteAsJsonAsync(new
                {
                    error = "An unexpected error occurred while deleting the document."
                });
                return errorResponse;
            }
        }
    }
}