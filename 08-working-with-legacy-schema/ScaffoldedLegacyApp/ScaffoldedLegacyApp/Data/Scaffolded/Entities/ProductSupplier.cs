using System;
using System.Collections.Generic;

namespace ContosoLegacySales.Data.Scaffolded.Entities;

public partial class ProductSupplier
{
    public int ProductID { get; set; }

    public string SupplierCode { get; set; } = null!;

    public string SupplierNm { get; set; } = null!;

    public int? LeadTimeDays { get; set; }

    public string PreferredFlag { get; set; } = null!;

    public virtual ProductCatalog Product { get; set; } = null!;
}
