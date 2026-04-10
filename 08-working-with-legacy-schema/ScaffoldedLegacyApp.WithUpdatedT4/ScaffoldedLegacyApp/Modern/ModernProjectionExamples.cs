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
                    h => h.CustomerId,
                    c => c.CustomerId,
                    (h, c) => new { h, c })
                .Join(
                    db.OrderStatuses,
                    hc => hc.h.StatusCode,
                    s => s.StatusCode,
                    (hc, s) => new { hc.h, hc.c, s })
                .LeftJoin(
                    db.Employees,
                    hcs => hcs.h.SalesRepEmployeeId,
                    e => e.EmployeeId,
                    (hcs, e) => new { hcs.h, hcs.c, hcs.s, SalesRep = e })
                .Join(
                    lineTotals,
                    x => x.h.OrderId,
                    lt => lt.OrderId,
                    (x, lt) => new ModernOrderProjection
                    {
                        OrderId = x.h.OrderId,
                        OrderNo = x.h.OrderNumber,
                        CustomerName = x.c.Name,
                        StatusName = x.s.StatusName,
                        SalesRepName = x.SalesRep != null ? x.SalesRep.FullName : "[Unassigned]",
                        TotalQuantity = lt.TotalQuantity,
                        NetAmount = lt.GrossAmount - (decimal)(x.h.HeaderDiscountAmount ?? 0)
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
                    d => d.OrderId,
                    h => h.OrderId,
                    (d, h) => new { d, h })
                .RightJoin(
                    db.ProductCatalogs,
                    dh => dh.d.ProductId,
                    p => p.ProductId,
                    (dh, p) => new ProductOrderCoverageDto
                    {
                        SKU = p.Sku,
                        ProductName = p.Name,
                        OrderId = dh != null ? dh.h.OrderId : null,
                        OrderNo = dh != null ? dh.h.OrderNumber : null
                    });

        return await query
            .OrderBy(x => x.SKU)
            .ThenBy(x => x.OrderNo)
            .ToListAsync();
    }
}
