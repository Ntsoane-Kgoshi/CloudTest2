using CoffeeNChill.Functions.DTOs;
using CoffeeNChill.Functions.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Net;

namespace CoffeeNChill.Functions.Functions.MenuItems
{
    public class GetMenuItemByCategoryFunction
    {
        private readonly ITableStorageService _storageService;

        // Constructor receives the table storage service through dependency injection
        public GetMenuItemByCategoryFunction(ITableStorageService storageService)
        {
            _storageService = storageService;
        }

        // HTTP GET endpoint used to retrieve menu items by category
        [Function("GetMenuItemsByCategory")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "menu/category/{category}")]
            HttpRequestData req,
            string category)
        {

            try
            {
                // Check that a category was provided
                if (string.IsNullOrWhiteSpace(category))
                {
                    var errorResponse = req.CreateResponse(HttpStatusCode.BadRequest);

                    await errorResponse.WriteAsJsonAsync(new
                    {
                        error = "Category is required."
                    });

                    return errorResponse;
                }

                // Retrieve menu items from Azure Table Storage
                var items = await _storageService.GetMenuItemsByCategoryAsync(category);

                // Convert the storage entities into response DTOs
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
                // Return an error response if something goes wrong
                var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);

                await errorResponse.WriteAsJsonAsync(new
                {
                    error = "An unexpected error occurred while retrieving menu items by category."
                });

                return errorResponse;
            }
        }
    }
}