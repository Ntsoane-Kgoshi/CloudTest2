using CoffeeNChill.Functions.DTOs;
using CoffeeNChill.Functions.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Net;

namespace CoffeeNChill.Functions.Functions.MenuItems
{
    public class GetAllMenuItemsFunction
    {
        private readonly ITableStorageService _storageService;

        // Constructor receives the table storage service through dependency injection
        public GetAllMenuItemsFunction(ITableStorageService storageService)
        {
            _storageService = storageService;
        }

        // HTTP GET endpoint used to retrieve all menu items
        [Function("GetAllMenuItems")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "menu")]
            HttpRequestData req)
        {
            try
            {
                // Retrieve all menu items from table storage
                var items = await _storageService.GetAllMenuItemsAsync();

                // Convert the storage models into response DTOs
                var itemResponses = items.Select(menuItem => new MenuItemResponse
                {
                    Category = menuItem.PartitionKey,
                    SKU = menuItem.RowKey,
                    Name = menuItem.Name,
                    Description = menuItem.Description,
                    Price = menuItem.Price,
                    IsAvailable = menuItem.IsAvailable
                }).ToList();

                // Create a successful HTTP response
                var httpResponse = req.CreateResponse(HttpStatusCode.OK);

                await httpResponse.WriteAsJsonAsync(itemResponses);

                return httpResponse;
            }
            catch (Exception)
            {
                // Return an error if an unexpected problem occurs
                var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);

                await errorResponse.WriteAsJsonAsync(new
                {
                    error = "An unexpected error occurred while retrieving menu items."
                });

                return errorResponse;
            }
        }
    }
}
