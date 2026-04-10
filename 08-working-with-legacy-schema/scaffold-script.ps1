Scaffold-DbContext `
  "Server=(localdb)\MSSQLLocalDB;Database=ContosoLegacySales;Trusted_Connection=True;TrustServerCertificate=True" `
  Microsoft.EntityFrameworkCore.SqlServer `
  -Context LegacySalesDbContext `
  -ContextDir Data/Context `
  -OutputDir Data/Scaffolded/Entities `
  -Namespace ScaffoldedLegacyApp.Data.Scaffolded.Entities `
  -ContextNamespace ScaffoldedLegacyApp.Data.Context `
  -Schemas crm,sales,ref `
  -Tables crm.tblCustomer,crm.Customer_Address,crm.Employee,sales.ProductCatalog,sales.ProductSupplier,sales.OrderHdr,sales.OrderDtl,sales.vw_OrderSummary,ref.lkp_OrderStatus,ref.Region `
  -UseDatabaseNames `
  -NoOnConfiguring