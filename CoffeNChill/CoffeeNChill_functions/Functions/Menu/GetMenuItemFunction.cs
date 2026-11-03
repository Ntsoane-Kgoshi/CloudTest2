using CoffeeNChill.Functions.DTOs;
using CoffeeNChill.Functions.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Net;

namespace CoffeeNChill.Functions.Functions.MenuItems
{
    public class GetMenuItemFunction
    {
        private readonly ITableStorageService _storageService;

        // Constructor receives the table storage service through dependency injection
        public GetMenuItemFunction(ITableStorageService storageService)
        {
            _storageService = storageService;
        }

        // HTTP GET endpoint used to retrieve a specific menu item
        [Function("GetMenuItem")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "menu/item/{category}/{sku}")]
            HttpRequestData req,
            string category,
            string sku)
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

                // Check that a SKU was provided
                if (string.IsNullOrWhiteSpace(sku))
                {
                    var errorResponse = req.CreateResponse(HttpStatusCode.BadRequest);

                    await errorResponse.WriteAsJsonAsync(new
                    {
                        error = "SKU is required."
                    });

                    return errorResponse;
                }

                // Retrieve the menu item from table storage
                var item = await _storageService.GetMenuItemsAsync(category, sku);

                // Return 404 if the menu item does not exist
                if (item == null)
                {
                    var notFoundResponse = req.CreateResponse(HttpStatusCode.NotFound);

                    await notFoundResponse.WriteAsJsonAsync(new
                    {
                        error = "Menu item not found."
                    });

                    return notFoundResponse;
                }

                // Convert the storage model into a response DTO
                var itemResponse = new MenuItemResponse
                {
                    Category = item.PartitionKey,
                    SKU = item.RowKey,
                    Name = item.Name,
                    Description = item.Description,
                    Price = item.Price,
                    IsAvailable = item.IsAvailable
                };

                // Return the menu item
                var httpResponse = req.CreateResponse(HttpStatusCode.OK);

                await httpResponse.WriteAsJsonAsync(itemResponse);

                return httpResponse;
            }
            catch (Exception)
            {
                // Return an error if an unexpected problem occurs
                var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);

                await errorResponse.WriteAsJsonAsync(new
                {
                    error = "An unexpected error occurred while retrieving the menu item."
                });

                return errorResponse;
            }
        }
    }
}
