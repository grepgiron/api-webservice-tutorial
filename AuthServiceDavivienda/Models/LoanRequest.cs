using System;
using System.Collections.Generic;

namespace AuthServiceDavivienda.Models
{
    public partial class LoanRequest
    {
        public string? Code { get; set; }
        public string? CustomerIdentification { get; set; }
        public int? Dues { get; set; }
        public decimal? Amount { get; set; }
        public int? State { get; set; }
        public decimal? Rate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? OfficialResolveIdentification { get; set; }
        public DateTime? ApproveDate { get; set; }
        public string? UserRequestIdentification { get; set; }
    }
}
