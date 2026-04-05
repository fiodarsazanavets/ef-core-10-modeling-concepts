using System;
using System.Collections.Generic;

namespace ContosoLegacySales.Data.Scaffolded.Entities;

public partial class lkp_OrderStatus
{
    public string StatusCode { get; set; } = null!;

    public string StatusName { get; set; } = null!;

    public string IsActiveFlag { get; set; } = null!;

    public virtual ICollection<OrderHdr> OrderHdrs { get; set; } = new List<OrderHdr>();
}
