using CoffeeNChill.Functions.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Net;

namespace CoffeeNChill.Functions.Functions.MenuItems
{
    public class DeleteMenuItemFunction
    {
        private readonly ITableStorageService _storageService;

        // Constructor receives the table storage service through dependency injection
        public DeleteMenuItemFunction(ITableStorageService storageService)
        {
            _storageService = storageService;
        }

        // HTTP DELETE endpoint used to delete a menu item
        [Function("DeleteMenuItem")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "menu/{category}/{sku}")]
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

                // Attempt to delete the menu item from table storage
                bool deleted = await _storageService.DeleteMenuItemAsync(category, sku);

                // Return 404 if the menu item does not exist
                if (!deleted)
                {
                    var notFoundResponse = req.CreateResponse(HttpStatusCode.NotFound);

                    await notFoundResponse.WriteAsJsonAsync(new
                    {
                        error = "Menu item not found."
                    });

                    return notFoundResponse;
                }

                // Return 204 when the menu item is successfully deleted
                return req.CreateResponse(HttpStatusCode.NoContent);
            }
            catch (Exception)
            {
                // Return an error if an unexpected problem occurs
                var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);

                await errorResponse.WriteAsJsonAsync(new
                {
                    error = "An unexpected error occurred while deleting the menu item."
                });

                return errorResponse;
            }
        }
    }
}
