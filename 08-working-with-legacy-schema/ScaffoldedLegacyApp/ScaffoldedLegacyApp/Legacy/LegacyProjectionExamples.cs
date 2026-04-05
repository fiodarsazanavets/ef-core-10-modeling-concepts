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
            join c in db.tblCustomers on h.CustId equals c.CustId
            join s in db.lkp_OrderStatuses on h.StatusCode equals s.StatusCode
            join e in db.Employees on h.SalesRepEmpID equals e.EmpID into salesRepGroup
            from salesRep in salesRepGroup.DefaultIfEmpty()
            join d in db.OrderDtls on h.OrderID equals d.OrderID
            group new { h, c, s, salesRep, d } by new
            {
                h.OrderID,
                h.OrderNo,
                c.Cust_Nm,
                s.StatusName,
                SalesRepName = salesRep != null ? salesRep.FullNm : null,
                h.HeaderDiscountAmt
            }
            into g
            select new LegacyOrderProjection
            {
                OrderId = g.Key.OrderID,
                OrderNo = g.Key.OrderNo,
                CustomerName = g.Key.Cust_Nm,
                StatusName = g.Key.StatusName,
                SalesRepName = g.Key.SalesRepName,
                TotalQuantity = g.Sum(x => x.d.QtyOrdered),
                NetAmount =
                    g.Sum(x => (decimal)x.d.QtyOrdered * (decimal)x.d.UnitPriceAmt
                               - (decimal)(x.d.LineDiscountAmt ?? 0))
                    - (decimal)(g.Key.HeaderDiscountAmt ?? 0)
            };

        return await query
            .OrderBy(x => x.OrderNo)
            .ToListAsync();
    }
}
