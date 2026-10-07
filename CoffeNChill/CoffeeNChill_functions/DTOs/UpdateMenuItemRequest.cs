using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeNChill.Functions.DTOs
{
    public class UpdateMenuItemRequest
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public double? Price { get; set; }
        public bool? IsAvailable { get; set; }
    }
}