using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureStorage.Models
{
    public class Product : TableEntity
    {
        public string Name { get; set; }
        public string Color { get; set; }
        public double Price { get; set; }
        public int Stock { get; set; }
    }
}
