using Azure;
using Azure.Data.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoffeeNChill.Functions.Models;

/// A single menu item as stored in the "MenuItems" Azure Table.
/// PartitionKey is the item's category (e.g. "Hot Drinks"); RowKey is
/// the item's unique SKU (e.g. "COF-001"). This is the storage model —
/// it never travels over HTTP directly, see MenuItemResponse for that.

namespace CoffeeNChill.Functions.Models
{
    public class MenuItem : ITableEntity
    {
        /// <summary>Menu category. Serves as the Azure Table PartitionKey.</summary>
        public string PartitionKey { get; set; } = string.Empty;

        /// <summary>Unique SKU for the menu item. Serves as the Azure Table RowKey.</summary>
        public string RowKey { get; set; } = string.Empty;

        /// <summary>Display name for the menu item.</summary>
        public string Name { get; set; } = string.Empty;
        
        /// <summary>Description for the menu item.</summary>
        public string Description { get; set; } = string.Empty;
        
        /// <summary>Price for the menu item.</summary>
        public double Price { get; set; }
        
        /// <summary>Indicates whether the menu item is available.</summary>
        public bool IsAvailable { get; set; }

        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }

        
    }
}
