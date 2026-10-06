using CoffeeNChill.Functions.DTOs;
using CoffeeNChill.Functions.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Net;
using System.Text.Json;

namespace CoffeeNChill.Functions.Functions.MenuItems
{
    public class UpdateMenuItemFunction
    {
        private readonly ITableStorageService _storageService;

        // Constructor receives the table storage service through dependency injection
        public UpdateMenuItemFunction(ITableStorageService storageService)
        {
            _storageService = storageService;
        }

        // HTTP PUT endpoint used to update a menu item
        [Function("UpdateMenuItem")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "menu/{category}/{sku}")]
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

                // Read and deserialize the request body
                var updateRequest = await JsonSerializer.DeserializeAsync<UpdateMenuItemRequest>(
                    req.Body,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                // Check that a valid request body was provided
                if (updateRequest == null)
                {
                    var errorResponse = req.CreateResponse(HttpStatusCode.BadRequest);

                    await errorResponse.WriteAsJsonAsync(new
                    {
                        error = "Invalid request body."
                    });

                    return errorResponse;
                }

                // Validate Name
                if (string.IsNullOrWhiteSpace(updateRequest.Name))
                {
                    var errorResponse = req.CreateResponse(HttpStatusCode.BadRequest);

                    await errorResponse.WriteAsJsonAsync(new
                    {
                        error = "Name is required."
                    });

                    return errorResponse;
                }

                // Validate Description
                if (string.IsNullOrWhiteSpace(updateRequest.Description))
                {
                    var errorResponse = req.CreateResponse(HttpStatusCode.BadRequest);

                    await errorResponse.WriteAsJsonAsync(new
                    {
                        error = "Description is required."
                    });

                    return errorResponse;
                }

                // Validate Price
                if (updateRequest.Price <= 0)
                {
                    var errorResponse = req.CreateResponse(HttpStatusCode.BadRequest);

                    await errorResponse.WriteAsJsonAsync(new
                    {
                        error = "Price must be greater than zero."
                    });

                    return errorResponse;
                }

                // Update the menu item in table storage
                var updatedItem = await _storageService.UpdateMenuItemAsync(
                    category,
                    sku,
                    updateRequest);

                // Return 404 if the menu item does not exist
                if (updatedItem == null)
                {
                    var notFoundResponse = req.CreateResponse(HttpStatusCode.NotFound);

                    await notFoundResponse.WriteAsJsonAsync(new
                    {
                        error = "Menu item not found."
                    });

                    return notFoundResponse;
                }

                // Convert the updated storage model into a response DTO
                var itemResponse = new MenuItemResponse
                {
                    Category = updatedItem.PartitionKey,
                    SKU = updatedItem.RowKey,
                    Name = updatedItem.Name,
                    Description = updatedItem.Description,
                    Price = updatedItem.Price,
                    IsAvailable = updatedItem.IsAvailable
                };

                // Return the updated menu item
                var httpResponse = req.CreateResponse(HttpStatusCode.OK);

                await httpResponse.WriteAsJsonAsync(itemResponse);

                return httpResponse;
            }
            catch (JsonException)
            {
                // Return an error when the request contains invalid JSON
                var errorResponse = req.CreateResponse(HttpStatusCode.BadRequest);

                await errorResponse.WriteAsJsonAsync(new
                {
                    error = "The request body contains invalid JSON."
                });

                return errorResponse;
            }
            catch (Exception)
            {
                // Return an error if an unexpected problem occurs
                var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);

                await errorResponse.WriteAsJsonAsync(new
                {
                    error = "An unexpected error occurred while updating the menu item."
                });

                return errorResponse;
            }
        }
    }
}
