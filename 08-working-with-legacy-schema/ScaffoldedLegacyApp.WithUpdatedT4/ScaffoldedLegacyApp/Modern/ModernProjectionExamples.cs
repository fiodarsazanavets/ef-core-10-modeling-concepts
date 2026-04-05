using ContosoLegacySales.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace ScaffoldedLegacyApp.Modern;

public static class ModernProjectionExamples
{
    public static async Task<List<ModernOrderProjection>> GetOrderSummaries_ModernLeftJoin(
        LegacySalesDbContext db)
    {
        var lineTotals =
            db.OrderDtls
                .GroupBy(d => d.OrderId)
                .Select(g => new
                {
                    OrderId = g.Key,
                    TotalQuantity = g.Sum(x => x.QuantityOrdered),
                    GrossAmount = g.Sum(x =>
                        (decimal)x.QuantityOrdered * (decimal)x.UnitPriceAmount
                        - (decimal)(x.LineDiscountAmount ?? 0))
                });

        var query =
            db.OrderHdrs
                .Join(
                    db.Customers,
                    h => h.CustId,
                    c => c.CustId,
                    (h, c) => new { h, c })
                .Join(
                    db.lkp_OrderStatuses,
                    hc => hc.h.StatusCode,
                    s => s.StatusCode,
                    (hc, s) => new { hc.h, hc.c, s })
                .LeftJoin(
                    db.Employees,
                    hcs => hcs.h.SalesRepEmpID,
                    e => e.EmpID,
                    (hcs, e) => new { hcs.h, hcs.c, hcs.s, SalesRep = e })
                .Join(
                    lineTotals,
                    x => x.h.OrderID,
                    lt => lt.OrderId,
                    (x, lt) => new ModernOrderProjection
                    {
                        OrderId = x.h.OrderID,
                        OrderNo = x.h.OrderNo,
                        CustomerName = x.c.Cust_Nm,
                        StatusName = x.s.StatusName,
                        SalesRepName = x.SalesRep != null ? x.SalesRep.FullNm : "[Unassigned]",
                        TotalQuantity = lt.TotalQuantity,
                        NetAmount = lt.GrossAmount - (decimal)(x.h.HeaderDiscountAmt ?? 0)
                    });

        return await query
            .OrderBy(x => x.OrderNo)
            .ToListAsync();
    }

    public static async Task<List<ProductOrderCoverageDto>> GetAllProductsIncludingUnorderedOnes(
        LegacySalesDbContext db)
    {
        var query =
            db.OrderDtls
                .Join(
                    db.OrderHdrs,
                    d => d.OrderID,
                    h => h.OrderID,
                    (d, h) => new { d, h })
                .RightJoin(
                    db.ProductCatalogs,
                    dh => dh.d.ProductID,
                    p => p.ProductID,
                    (dh, p) => new ProductOrderCoverageDto
                    {
                        SKU = p.SKU,
                        ProductName = p.ProductNm,
                        OrderId = dh != null ? dh.h.OrderID : null,
                        OrderNo = dh != null ? dh.h.OrderNo : null
                    });

        return await query
            .OrderBy(x => x.SKU)
            .ThenBy(x => x.OrderNo)
            .ToListAsync();
    }
}
