using System;
using System.Collections.Generic;

namespace ContosoLegacySales.Data.Scaffolded.Entities;

public partial class Customer_Address
{
    public int AddrId { get; set; }

    public int CustId { get; set; }

    public string AddrTypeCd { get; set; } = null!;

    public string AddrLine1 { get; set; } = null!;

    public string? AddrLine2 { get; set; }

    public string CityNm { get; set; } = null!;

    public string? StateProv { get; set; }

    public string? PostalCd { get; set; }

    public string CountryNm { get; set; } = null!;

    public string IsPrimaryFlag { get; set; } = null!;

    public virtual tblCustomer Cust { get; set; } = null!;

    public virtual ICollection<OrderHdr> OrderHdrBillToAddrs { get; set; } = new List<OrderHdr>();

    public virtual ICollection<OrderHdr> OrderHdrShipToAddrs { get; set; } = new List<OrderHdr>();
}
