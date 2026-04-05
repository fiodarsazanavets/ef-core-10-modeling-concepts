using System;
using System.Collections.Generic;

namespace ContosoLegacySales.Data.Scaffolded.Entities;

public partial class vw_OrderSummary
{
    public int OrderID { get; set; }

    public string OrderNo { get; set; } = null!;

    public int CustId { get; set; }

    public string Cust_Nm { get; set; } = null!;

    public string StatusCode { get; set; } = null!;

    public string StatusName { get; set; } = null!;

    public DateTime OrderedOn { get; set; }

    public int? SalesRepEmpID { get; set; }

    public string? SalesRepName { get; set; }

    public int? TotalQty { get; set; }

    public decimal? NetAmount { get; set; }
}
