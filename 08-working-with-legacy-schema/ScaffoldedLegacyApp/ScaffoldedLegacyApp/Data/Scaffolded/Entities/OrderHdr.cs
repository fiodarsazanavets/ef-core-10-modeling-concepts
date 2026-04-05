using System;
using System.Collections.Generic;

namespace ContosoLegacySales.Data.Scaffolded.Entities;

public partial class OrderHdr
{
    public int OrderID { get; set; }

    public string OrderNo { get; set; } = null!;

    public int CustId { get; set; }

    public int? SalesRepEmpID { get; set; }

    public string StatusCode { get; set; } = null!;

    public DateTime OrderedOn { get; set; }

    public DateTime? RequiredByDt { get; set; }

    public int? ShipToAddrId { get; set; }

    public int? BillToAddrId { get; set; }

    public string? LegacySourceSystem { get; set; }

    public string? ExternalRefNo { get; set; }

    public decimal? HeaderDiscountAmt { get; set; }

    public string? Comments { get; set; }

    public virtual Customer_Address? BillToAddr { get; set; }

    public virtual tblCustomer Cust { get; set; } = null!;

    public virtual ICollection<OrderDtl> OrderDtls { get; set; } = new List<OrderDtl>();

    public virtual Employee? SalesRepEmp { get; set; }

    public virtual Customer_Address? ShipToAddr { get; set; }

    public virtual lkp_OrderStatus StatusCodeNavigation { get; set; } = null!;
}
