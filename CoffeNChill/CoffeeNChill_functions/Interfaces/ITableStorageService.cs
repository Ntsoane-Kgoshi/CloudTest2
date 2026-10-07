using CoffeeNChill.Functions.DTOs;
using CoffeeNChill.Functions.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Handles all persistence for menu items against Azure Table Storage.
/// Kept behind an interface so the HTTP-triggered Functions depend only
/// on this contract, not on Azure.Data.Tables directly — that's the
/// separation of storage concerns the Functions layer relies on.
/// </summary>

namespace CoffeeNChill.Functions.Interfaces
{
    /// <summary> /// Provides methods for managing menu items in Azure Table Storage. /// </summary>
    public interface ITableStorageService
    {
        /// <summary> /// Creates a new menu item. /// </summary>
        Task<MenuItem> CreateMenuItemAsync(CreateMenuItemRequest request);
        /// <summary> /// Gets all menu items. /// </summary>
        Task<List<MenuItem>> GetAllMenuItemsAsync();
        /// <summary> /// Gets menu items by category. /// </summary>
        Task<List<MenuItem>> GetMenuItemsByCategoryAsync(string category);
        /// <summary> /// Gets a menu item by category and SKU. /// </summary>
        Task<MenuItem> GetMenuItemsAsync(string category, string sku);
        /// <summary> /// Updates an existing menu item. /// </summary>
        Task<MenuItem?> UpdateMenuItemAsync(string category, string sku, UpdateMenuItemRequest request);
        /// <summary> /// Deletes a menu item by category and SKU. /// </summary>
        Task<bool> DeleteMenuItemAsync(string category, string sku);   

    }
}
