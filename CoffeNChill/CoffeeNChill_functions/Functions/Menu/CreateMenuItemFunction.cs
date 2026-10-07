using Azure;
using CoffeeNChill.Functions.DTOs;
using CoffeeNChill.Functions.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Net;
using System.Text.Json;

namespace CoffeeNChill.Functions.Functions.MenuItems
{
    public class CreateMenuItemFunction
    {
        private readonly ITableStorageService _storageService;

        public CreateMenuItemFunction(ITableStorageService storageService)
        {
            _storageService = storageService;
        }

        [Function("CreateMenuItem")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "menu")]
            HttpRequestData req)
        {
            try
            {
                // Read and deserialize the request body
                var menuRequest = await JsonSerializer.DeserializeAsync<CreateMenuItemRequest>(
                    req.Body,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                // Check that a valid request body was provided
                if (menuRequest == null)
                {
                    var errorResponse = req.CreateResponse(HttpStatusCode.BadRequest);

                    await errorResponse.WriteAsJsonAsync(new
                    {
                        error = "Invalid request body."
                    });

                    return errorResponse;
                }

                // Validate Category
                if (string.IsNullOrWhiteSpace(menuRequest.Category))
                {
                    var errorResponse = req.CreateResponse(HttpStatusCode.BadRequest);

                    await errorResponse.WriteAsJsonAsync(new
                    {
                        error = "Category is required."
                    });

                    return errorResponse;
                }

                // Validate SKU
                if (string.IsNullOrWhiteSpace(menuRequest.SKU))
                {
                    var errorResponse = req.CreateResponse(HttpStatusCode.BadRequest);

                    await errorResponse.WriteAsJsonAsync(new
                    {
                        error = "SKU is required."
                    });

                    return errorResponse;
                }

                // Validate Name
                if (string.IsNullOrWhiteSpace(menuRequest.Name))
                {
                    var errorResponse = req.CreateResponse(HttpStatusCode.BadRequest);

                    await errorResponse.WriteAsJsonAsync(new
                    {
                        error = "Name is required."
                    });

                    return errorResponse;
                }

                // Validate Description
                if (string.IsNullOrWhiteSpace(menuRequest.Description))
                {
                    var errorResponse = req.CreateResponse(HttpStatusCode.BadRequest);

                    await errorResponse.WriteAsJsonAsync(new
                    {
                        error = "Description is required."
                    });

                    return errorResponse;
                }

                // Validate Price
                if (menuRequest.Price <= 0)
                {
                    var errorResponse = req.CreateResponse(HttpStatusCode.BadRequest);

                    await errorResponse.WriteAsJsonAsync(new
                    {
                        error = "Price must be greater than zero."
                    });

                    return errorResponse;
                }

                // Create the menu item in table storage
                var createdItem = await _storageService.CreateMenuItemAsync(menuRequest);

                // Convert the storage model into a response DTO
                var itemResponse = new MenuItemResponse
                {
                    Category = createdItem.PartitionKey,
                    SKU = createdItem.RowKey,
                    Name = createdItem.Name,
                    Description = createdItem.Description,
                    Price = createdItem.Price,
                    IsAvailable = createdItem.IsAvailable
                };

                // Return the newly created menu item
                var httpResponse = req.CreateResponse(HttpStatusCode.Created);

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
            catch (RequestFailedException ex)
                when (ex.Status == (int)HttpStatusCode.Conflict)
            {
                // Return a conflict when the menu item already exists
                var conflictResponse = req.CreateResponse(HttpStatusCode.Conflict);

                await conflictResponse.WriteAsJsonAsync(new
                {
                    error = "A menu item with the same category and SKU already exists."
                });

                return conflictResponse;
            }
            catch (Exception)
            {
                // Return an error if an unexpected problem occurs
                var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);

                await errorResponse.WriteAsJsonAsync(new
                {
                    error = "An unexpected error occurred while creating the menu item."
                });

                return errorResponse;
            }
        }
    }
}
