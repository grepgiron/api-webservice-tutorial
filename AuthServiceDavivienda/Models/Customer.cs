using System;
using System.Collections.Generic;

namespace AuthServiceDavivienda.Models
{
    public partial class Customer
    {
        public string Identification { get; set; } = null!;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Addres { get; set; }
        public string? Email { get; set; }
        public string? Telephone { get; set; }
    }
}
