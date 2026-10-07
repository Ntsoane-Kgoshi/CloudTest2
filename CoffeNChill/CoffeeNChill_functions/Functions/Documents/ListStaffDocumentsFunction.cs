using CoffeeNChill.Functions.DTOs;
using CoffeeNChill.Functions.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Net;

namespace CoffeeNChill.Functions.Functions.StaffDocuments
{
    public class ListStaffDocumentsFunction
    {
        private readonly IFileStorageService _storage;

        public ListStaffDocumentsFunction(IFileStorageService storage)
        {
            _storage = storage;
        }

        [Function("ListStaffDocuments")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "documents")]
            HttpRequestData request)
        {
            try
            {
                //Retrieve every staff document currently stored
                var allDocs = await _storage.GetAllDocumentsAsync();

                //Project each domain model into the response DTO
                var dtoList = allDocs
                    .Select(d => new StaffDocumentResponse
                    {
                        FileName = d.FileName,
                        Extension = d.Extension,
                        Type = d.Type,
                        FileSize = d.FileSize,
                        UploadDate = d.UploadDate,
                        ContainerName = d.ContainerName
                    })
                    .ToList();

                //Build and return a successful JSON response
                var okResponse = request.CreateResponse(HttpStatusCode.OK);
                await okResponse.WriteAsJsonAsync(dtoList);
                return okResponse;
            }
            catch
            {
                //Return an error on any unexpected failure
                var errorResponse = request.CreateResponse(HttpStatusCode.InternalServerError);
                await errorResponse.WriteAsJsonAsync(new
                {
                    message = "An error occurred while retrieving staff documents."
                });
                return errorResponse;
            }
        }
    }
}