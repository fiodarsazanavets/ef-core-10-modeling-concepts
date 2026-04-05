using ContosoLegacySales.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace ScaffoldedLegacyApp.Legacy;

public static class LegacyProjectionExamples
{
    public static async Task<List<LegacyOrderProjection>> GetOrderSummaries_LegacyPattern(
        LegacySalesDbContext db)
    {
        var query =
            from h in db.OrderHdrs
            join c in db.Customers on h.CustomerId equals c.CustomerId
            join s in db.OrderStatuses on h.StatusCode equals s.StatusCode
            join e in db.Employees on h.SalesRepEmployeeId equals e.EmployeeId into salesRepGroup
            from salesRep in salesRepGroup.DefaultIfEmpty()
            join d in db.OrderDtls on h.OrderId equals d.OrderId
            group new { h, c, s, salesRep, d } by new
            {
                h.OrderId,
                h.OrderNumber,
                c.Name,
                s.StatusName,
                SalesRepName = salesRep != null ? salesRep.FullName : null,
                h.HeaderDiscountAmount
            }
            into g
            select new LegacyOrderProjection
            {
                OrderId = g.Key.OrderId,
                OrderNo = g.Key.OrderNumber,
                CustomerName = g.Key.Name,
                StatusName = g.Key.StatusName,
                SalesRepName = g.Key.SalesRepName,
                TotalQuantity = g.Sum(x => x.d.QuantityOrdered),
                NetAmount =
                    g.Sum(x => (decimal)x.d.QuantityOrdered * (decimal)x.d.UnitPriceAmount
                               - (decimal)(x.d.LineDiscountAmount ?? 0))
                    - (decimal)(g.Key.HeaderDiscountAmount ?? 0)
            };

        return await query
            .OrderBy(x => x.OrderNo)
            .ToListAsync();
    }
}
