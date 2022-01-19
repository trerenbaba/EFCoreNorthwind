using System;
using System.Collections.Generic;

#nullable disable

namespace EFCoreNorthwind.Data
{
    public partial class VwNancy
    {
        public string FirstName { get; set; }
        public string ProductName { get; set; }
        public decimal? UnitPrice { get; set; }
        public short? UnitsInStock { get; set; }
    }
}
