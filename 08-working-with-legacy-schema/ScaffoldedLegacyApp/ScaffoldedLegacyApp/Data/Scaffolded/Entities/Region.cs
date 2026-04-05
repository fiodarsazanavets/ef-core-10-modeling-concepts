using System;
using System.Collections.Generic;

namespace ContosoLegacySales.Data.Scaffolded.Entities;

public partial class Region
{
    public string RegionCd { get; set; } = null!;

    public string RegionName { get; set; } = null!;

    public virtual ICollection<tblCustomer> tblCustomers { get; set; } = new List<tblCustomer>();
}
