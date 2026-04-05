using System;
using System.Collections.Generic;

namespace ContosoLegacySales.Data.Scaffolded.Entities;

public partial class OrderDtl
{
    public int OrderID { get; set; }

    public int LineNum { get; set; }

    public int ProductID { get; set; }

    public int QtyOrdered { get; set; }

    public decimal UnitPriceAmt { get; set; }

    public decimal? LineDiscountAmt { get; set; }

    public int? CreatedByUserId { get; set; }

    public int? FulfilledQty { get; set; }

    public virtual OrderHdr Order { get; set; } = null!;

    public virtual ProductCatalog Product { get; set; } = null!;
}
