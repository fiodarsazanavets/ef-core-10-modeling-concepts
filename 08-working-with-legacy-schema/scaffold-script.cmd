dotnet ef dbcontext scaffold ^
  "Server=(localdb)\MSSQLLocalDB;Database=ContosoLegacySales;Trusted_Connection=True;TrustServerCertificate=True" ^
  Microsoft.EntityFrameworkCore.SqlServer ^
  --context LegacySalesDbContext ^
  --context-dir Data/Context ^
  --output-dir Data/Scaffolded/Entities ^
  --namespace ContosoLegacySales.Data.Scaffolded.Entities ^
  --context-namespace ContosoLegacySales.Data.Context ^
  --schema crm ^
  --schema sales ^
  --schema ref ^
  --table crm.tblCustomer ^
  --table crm.Customer_Address ^
  --table crm.Employee ^
  --table sales.ProductCatalog ^
  --table sales.ProductSupplier ^
  --table sales.OrderHdr ^
  --table sales.OrderDtl ^
  --table sales.vw_OrderSummary ^
  --table ref.lkp_OrderStatus ^
  --table ref.Region ^
  --use-database-names ^
  --no-onconfiguring