using System;
using System.Collections.Generic;

namespace ContosoLegacySales.Data.Scaffolded.Entities;

public partial class tblCustomer
{
    public int CustId { get; set; }

    public string CustCode { get; set; } = null!;

    public string Cust_Nm { get; set; } = null!;

    public string? PrimaryEmailAddr { get; set; }

    public string? PhoneNo { get; set; }

    public string IsPreferred { get; set; } = null!;

    public decimal? CreditLimitAmt { get; set; }

    public string? RegionCd { get; set; }

    public int? LegacyScore { get; set; }

    public DateTime CreatedOn { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public byte[] rv { get; set; } = null!;

    public virtual ICollection<Customer_Address> Customer_Addresses { get; set; } = new List<Customer_Address>();

    public virtual ICollection<OrderHdr> OrderHdrs { get; set; } = new List<OrderHdr>();

    public virtual Region? RegionCdNavigation { get; set; }
}
