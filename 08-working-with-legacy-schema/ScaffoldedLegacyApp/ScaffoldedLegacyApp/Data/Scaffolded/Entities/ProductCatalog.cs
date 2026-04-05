using System;
using System.Collections.Generic;

namespace ContosoLegacySales.Data.Scaffolded.Entities;

public partial class ProductCatalog
{
    public int ProductID { get; set; }

    public string SKU { get; set; } = null!;

    public string ProductNm { get; set; } = null!;

    public string ProductTypeCd { get; set; } = null!;

    public decimal UnitPriceAmt { get; set; }

    public string IsDiscontinuedFlag { get; set; } = null!;

    public decimal? WeightKg { get; set; }

    public string? DownloadUrl { get; set; }

    public string? NotesTxt { get; set; }

    public virtual ICollection<OrderDtl> OrderDtls { get; set; } = new List<OrderDtl>();

    public virtual ICollection<ProductSupplier> ProductSuppliers { get; set; } = new List<ProductSupplier>();
}
