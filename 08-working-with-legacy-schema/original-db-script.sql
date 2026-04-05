USE master;
GO

IF DB_ID('ContosoLegacySales') IS NOT NULL
BEGIN
    ALTER DATABASE ContosoLegacySales SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE ContosoLegacySales;
END
GO

CREATE DATABASE ContosoLegacySales;
GO

USE ContosoLegacySales;
GO

CREATE SCHEMA crm;
GO
CREATE SCHEMA sales;
GO
CREATE SCHEMA ref;
GO
CREATE SCHEMA audit;
GO

/* =========================================================
   Reference tables
   ========================================================= */

CREATE TABLE ref.lkp_OrderStatus
(
    StatusCode       char(1)        NOT NULL,
    StatusName       varchar(50)    NOT NULL,
    IsActiveFlag     char(1)        NOT NULL CONSTRAINT DF_lkp_OrderStatus_IsActiveFlag DEFAULT ('Y'),
    CONSTRAINT PK_lkp_OrderStatus PRIMARY KEY (StatusCode)
);
GO

CREATE TABLE ref.Region
(
    RegionCd         varchar(10)    NOT NULL,
    RegionName       varchar(100)   NOT NULL,
    CONSTRAINT PK_Region PRIMARY KEY (RegionCd)
);
GO

/* =========================================================
   CRM tables
   ========================================================= */

CREATE TABLE crm.tblCustomer
(
    CustId                int              IDENTITY(1,1) NOT NULL,
    CustCode              varchar(20)      NOT NULL,
    Cust_Nm               varchar(200)     NOT NULL,
    PrimaryEmailAddr      varchar(320)     NULL,
    PhoneNo               varchar(30)      NULL,
    IsPreferred           char(1)          NOT NULL CONSTRAINT DF_tblCustomer_IsPreferred DEFAULT ('N'),
    CreditLimitAmt        money            NULL,
    RegionCd              varchar(10)      NULL,
    LegacyScore           int              NULL,
    CreatedOn             datetime         NOT NULL CONSTRAINT DF_tblCustomer_CreatedOn DEFAULT (GETDATE()),
    ModifiedOn            datetime         NULL,
    rv                    rowversion       NOT NULL,
    CONSTRAINT PK_tblCustomer PRIMARY KEY (CustId),
    CONSTRAINT UQ_tblCustomer_CustCode UNIQUE (CustCode),
    CONSTRAINT FK_tblCustomer_Region FOREIGN KEY (RegionCd) REFERENCES ref.Region(RegionCd)
);
GO

CREATE TABLE crm.Customer_Address
(
    AddrId                int              IDENTITY(1,1) NOT NULL,
    CustId                int              NOT NULL,
    AddrTypeCd            char(1)          NOT NULL, -- B = billing, S = shipping
    AddrLine1             varchar(200)     NOT NULL,
    AddrLine2             varchar(200)     NULL,
    CityNm                varchar(100)     NOT NULL,
    StateProv             varchar(100)     NULL,
    PostalCd              varchar(20)      NULL,
    CountryNm             varchar(100)     NOT NULL,
    IsPrimaryFlag         char(1)          NOT NULL CONSTRAINT DF_Customer_Address_IsPrimaryFlag DEFAULT ('N'),
    CONSTRAINT PK_Customer_Address PRIMARY KEY (AddrId),
    CONSTRAINT FK_Customer_Address_tblCustomer FOREIGN KEY (CustId) REFERENCES crm.tblCustomer(CustId)
);
GO

CREATE TABLE crm.Employee
(
    EmpID                 int              IDENTITY(1,1) NOT NULL,
    EmpNo                 varchar(20)      NOT NULL,
    FullNm                varchar(200)     NOT NULL,
    EmailAddr             varchar(320)     NULL,
    ManagerEmpID          int              NULL,
    IsSalesRepFlag        char(1)          NOT NULL CONSTRAINT DF_Employee_IsSalesRepFlag DEFAULT ('N'),
    HireDt                datetime         NOT NULL,
    TerminatedDt          datetime         NULL,
    CONSTRAINT PK_Employee PRIMARY KEY (EmpID),
    CONSTRAINT UQ_Employee_EmpNo UNIQUE (EmpNo),
    CONSTRAINT FK_Employee_Manager FOREIGN KEY (ManagerEmpID) REFERENCES crm.Employee(EmpID)
);
GO

/* =========================================================
   Sales tables
   ========================================================= */

CREATE TABLE sales.ProductCatalog
(
    ProductID             int              IDENTITY(1,1) NOT NULL,
    SKU                   varchar(40)      NOT NULL,
    ProductNm             varchar(200)     NOT NULL,
    ProductTypeCd         char(1)          NOT NULL, -- P = physical, D = digital, S = service
    UnitPriceAmt          money            NOT NULL,
    IsDiscontinuedFlag    char(1)          NOT NULL CONSTRAINT DF_ProductCatalog_IsDiscontinuedFlag DEFAULT ('N'),
    WeightKg              decimal(10,2)    NULL,
    DownloadUrl           varchar(500)     NULL,
    NotesTxt              text             NULL,
    CONSTRAINT PK_ProductCatalog PRIMARY KEY (ProductID),
    CONSTRAINT UQ_ProductCatalog_SKU UNIQUE (SKU)
);
GO

CREATE TABLE sales.OrderHdr
(
    OrderID               int              IDENTITY(1000,1) NOT NULL,
    OrderNo               varchar(30)      NOT NULL,
    CustId                int              NOT NULL,
    SalesRepEmpID         int              NULL,
    StatusCode            char(1)          NOT NULL,
    OrderedOn             datetime         NOT NULL CONSTRAINT DF_OrderHdr_OrderedOn DEFAULT (GETDATE()),
    RequiredByDt          datetime         NULL,
    ShipToAddrId          int              NULL,
    BillToAddrId          int              NULL,
    LegacySourceSystem    varchar(30)      NULL,
    ExternalRefNo         varchar(50)      NULL,
    HeaderDiscountAmt     money            NULL CONSTRAINT DF_OrderHdr_HeaderDiscountAmt DEFAULT (0),
    Comments              text             NULL,
    CONSTRAINT PK_OrderHdr PRIMARY KEY (OrderID),
    CONSTRAINT UQ_OrderHdr_OrderNo UNIQUE (OrderNo),
    CONSTRAINT FK_OrderHdr_tblCustomer FOREIGN KEY (CustId) REFERENCES crm.tblCustomer(CustId),
    CONSTRAINT FK_OrderHdr_Employee FOREIGN KEY (SalesRepEmpID) REFERENCES crm.Employee(EmpID),
    CONSTRAINT FK_OrderHdr_Status FOREIGN KEY (StatusCode) REFERENCES ref.lkp_OrderStatus(StatusCode),
    CONSTRAINT FK_OrderHdr_ShipToAddr FOREIGN KEY (ShipToAddrId) REFERENCES crm.Customer_Address(AddrId),
    CONSTRAINT FK_OrderHdr_BillToAddr FOREIGN KEY (BillToAddrId) REFERENCES crm.Customer_Address(AddrId)
);
GO

CREATE TABLE sales.OrderDtl
(
    OrderID               int              NOT NULL,
    LineNum               int              NOT NULL,
    ProductID             int              NOT NULL,
    QtyOrdered            int              NOT NULL,
    UnitPriceAmt          money            NOT NULL,
    LineDiscountAmt       money            NULL CONSTRAINT DF_OrderDtl_LineDiscountAmt DEFAULT (0),
    CreatedByUserId       int              NULL, -- deliberate problem: should probably FK to Employee, but no FK exists
    FulfilledQty          int              NULL,
    CONSTRAINT PK_OrderDtl PRIMARY KEY (OrderID, LineNum),
    CONSTRAINT FK_OrderDtl_OrderHdr FOREIGN KEY (OrderID) REFERENCES sales.OrderHdr(OrderID),
    CONSTRAINT FK_OrderDtl_ProductCatalog FOREIGN KEY (ProductID) REFERENCES sales.ProductCatalog(ProductID)
);
GO

CREATE TABLE sales.ProductSupplier
(
    ProductID             int              NOT NULL,
    SupplierCode          varchar(20)      NOT NULL,
    SupplierNm            varchar(200)     NOT NULL,
    LeadTimeDays          int              NULL,
    PreferredFlag         char(1)          NOT NULL CONSTRAINT DF_ProductSupplier_PreferredFlag DEFAULT ('N'),
    CONSTRAINT PK_ProductSupplier PRIMARY KEY (ProductID, SupplierCode),
    CONSTRAINT FK_ProductSupplier_ProductCatalog FOREIGN KEY (ProductID) REFERENCES sales.ProductCatalog(ProductID)
);
GO

/* =========================================================
   Audit table WITHOUT a primary key on purpose
   This is common in legacy systems and useful for demoing
   scaffolding limitations.
   ========================================================= */

CREATE TABLE audit.ImportRunLog
(
    RunStartedOn          datetime         NOT NULL,
    SourceSystem          varchar(50)      NOT NULL,
    RowsRead              int              NOT NULL,
    RowsImported          int              NOT NULL,
    ResultMessage         varchar(4000)    NULL
);
GO

/* =========================================================
   Indexes
   ========================================================= */

CREATE INDEX IX_tblCustomer_RegionCd ON crm.tblCustomer(RegionCd);
CREATE INDEX IX_OrderHdr_CustId ON sales.OrderHdr(CustId);
CREATE INDEX IX_OrderHdr_SalesRepEmpID ON sales.OrderHdr(SalesRepEmpID);
CREATE INDEX IX_OrderHdr_StatusCode ON sales.OrderHdr(StatusCode);
CREATE INDEX IX_OrderDtl_ProductID ON sales.OrderDtl(ProductID);
GO

/* =========================================================
   View for reporting
   ========================================================= */

CREATE VIEW sales.vw_OrderSummary
AS
SELECT
    h.OrderID,
    h.OrderNo,
    h.CustId,
    c.Cust_Nm,
    h.StatusCode,
    s.StatusName,
    h.OrderedOn,
    h.SalesRepEmpID,
    e.FullNm AS SalesRepName,
    SUM(d.QtyOrdered) AS TotalQty,
    SUM((d.QtyOrdered * d.UnitPriceAmt) - ISNULL(d.LineDiscountAmt, 0)) - ISNULL(h.HeaderDiscountAmt, 0) AS NetAmount
FROM sales.OrderHdr h
INNER JOIN crm.tblCustomer c ON c.CustId = h.CustId
INNER JOIN ref.lkp_OrderStatus s ON s.StatusCode = h.StatusCode
LEFT JOIN crm.Employee e ON e.EmpID = h.SalesRepEmpID
INNER JOIN sales.OrderDtl d ON d.OrderID = h.OrderID
GROUP BY
    h.OrderID,
    h.OrderNo,
    h.CustId,
    c.Cust_Nm,
    h.StatusCode,
    s.StatusName,
    h.OrderedOn,
    h.SalesRepEmpID,
    e.FullNm,
    h.HeaderDiscountAmt;
GO

/* =========================================================
   Seed data
   ========================================================= */

INSERT INTO ref.Region (RegionCd, RegionName)
VALUES
('UK', 'United Kingdom'),
('US-EAST', 'US East'),
('US-WEST', 'US West'),
('DACH', 'Germany/Austria/Switzerland');
GO

INSERT INTO ref.lkp_OrderStatus (StatusCode, StatusName, IsActiveFlag)
VALUES
('N', 'New', 'Y'),
('P', 'Processing', 'Y'),
('S', 'Shipped', 'Y'),
('C', 'Cancelled', 'Y'),
('H', 'On Hold', 'Y');
GO

INSERT INTO crm.Employee (EmpNo, FullNm, EmailAddr, ManagerEmpID, IsSalesRepFlag, HireDt, TerminatedDt)
VALUES
('E100', 'Sophia Turner', 'sophia.turner@contoso.com', NULL, 'N', '2018-01-15', NULL),
('E110', 'Daniel Brooks', 'daniel.brooks@contoso.com', 1, 'Y', '2019-06-01', NULL),
('E120', 'Mia Patel', 'mia.patel@contoso.com', 1, 'Y', '2020-03-20', NULL),
('E130', 'Liam Carter', 'liam.carter@contoso.com', 1, 'Y', '2021-11-05', NULL);
GO

INSERT INTO crm.tblCustomer
(
    CustCode, Cust_Nm, PrimaryEmailAddr, PhoneNo, IsPreferred,
    CreditLimitAmt, RegionCd, LegacyScore, CreatedOn, ModifiedOn
)
VALUES
('CUST-001', 'Northwind Retail Ltd', 'procurement@northwind-retail.com', '+44-20-1000-1000', 'Y', 50000, 'UK', 82, '2023-01-10', '2025-11-12'),
('CUST-002', 'Alpine Industrial GmbH', 'orders@alpine-industrial.de', '+49-30-2000-2000', 'N', 120000, 'DACH', 91, '2023-02-05', '2025-12-01'),
('CUST-003', 'Blue Yonder Stores', 'sourcing@blueyonder.com', '+1-212-555-0100', 'Y', 75000, 'US-EAST', 77, '2023-03-19', NULL),
('CUST-004', 'Fabrikam Services', NULL, '+1-415-555-0200', 'N', NULL, 'US-WEST', 64, '2023-06-30', '2025-08-14');
GO

INSERT INTO crm.Customer_Address
(
    CustId, AddrTypeCd, AddrLine1, AddrLine2, CityNm, StateProv, PostalCd, CountryNm, IsPrimaryFlag
)
VALUES
(1, 'B', '10 Bishopsgate', NULL, 'London', NULL, 'EC2N 4BQ', 'UK', 'Y'),
(1, 'S', 'Warehouse 3, Docklands', NULL, 'London', NULL, 'E16 2GT', 'UK', 'Y'),
(2, 'B', 'Am Industriepark 12', NULL, 'Berlin', NULL, '10115', 'Germany', 'Y'),
(3, 'B', '550 Madison Ave', 'Floor 8', 'New York', 'NY', '10022', 'USA', 'Y'),
(4, 'B', '1 Market St', NULL, 'San Francisco', 'CA', '94105', 'USA', 'Y');
GO

INSERT INTO sales.ProductCatalog
(
    SKU, ProductNm, ProductTypeCd, UnitPriceAmt, IsDiscontinuedFlag, WeightKg, DownloadUrl, NotesTxt
)
VALUES
('LAP-15-PRO', 'Contoso Pro Laptop 15"', 'P', 1450.00, 'N', 1.80, NULL, 'High-volume corporate device'),
('MON-27-4K', 'Contoso 27" 4K Monitor', 'P', 420.00, 'N', 4.90, NULL, 'Frequently bundled with laptop orders'),
('LIC-ANALYTICS', 'Analytics Suite Annual License', 'D', 999.00, 'N', NULL, 'https://downloads.contoso.com/licenses/analytics', 'Electronic delivery only'),
('SUP-ONSITE', 'On-site Installation Service', 'S', 250.00, 'N', NULL, NULL, 'Scheduled with field team'),
('MON-24-OLD', 'Legacy 24" Monitor', 'P', 180.00, 'Y', 3.80, NULL, 'Kept for historical orders');
GO

INSERT INTO sales.ProductSupplier
(
    ProductID, SupplierCode, SupplierNm, LeadTimeDays, PreferredFlag
)
VALUES
(1, 'SUP-TECHDIST', 'Tech Distribution plc', 14, 'Y'),
(1, 'SUP-ALT-HW', 'Alt Hardware BV', 21, 'N'),
(2, 'SUP-DISPLAYCO', 'DisplayCo GmbH', 10, 'Y'),
(3, 'SUP-SOFTLIC', 'SoftLicensing Ltd', 1, 'Y'),
(4, 'SUP-FIELDSVC', 'Field Services Partners', 5, 'Y');
GO

INSERT INTO sales.OrderHdr
(
    OrderNo, CustId, SalesRepEmpID, StatusCode, OrderedOn, RequiredByDt,
    ShipToAddrId, BillToAddrId, LegacySourceSystem, ExternalRefNo, HeaderDiscountAmt, Comments
)
VALUES
('SO-2025-0001', 1, 2, 'S', '2025-11-01', '2025-11-15', 2, 1, 'CRM-V1', 'NW-PO-7781', 50.00, 'Priority customer'),
('SO-2025-0002', 2, 3, 'P', '2025-11-10', '2025-11-25', 3, 3, 'CRM-V1', 'ALP-45002', 0.00, NULL),
('SO-2025-0003', 3, NULL, 'N', '2025-11-20', '2025-12-05', 4, 4, 'EDI', 'BY-99881', 25.00, 'Awaiting sales rep assignment'),
('SO-2025-0004', 4, 4, 'C', '2025-11-22', NULL, 5, 5, 'PORTAL', 'FAB-1029', 0.00, 'Cancelled by customer'),
('SO-2025-0005', 1, 2, 'H', '2025-11-25', '2025-12-20', 2, 1, 'CRM-V1', 'NW-PO-7810', 100.00, 'Credit review');
GO

INSERT INTO sales.OrderDtl
(
    OrderID, LineNum, ProductID, QtyOrdered, UnitPriceAmt, LineDiscountAmt, CreatedByUserId, FulfilledQty
)
VALUES
(1000, 1, 1, 5, 1450.00, 100.00, 2, 5),
(1000, 2, 2, 5, 420.00, 0.00, 2, 5),
(1000, 3, 4, 1, 250.00, 0.00, 2, 1),

(1001, 1, 3, 25, 999.00, 500.00, 3, 0),
(1001, 2, 4, 2, 250.00, 0.00, 3, 0),

(1002, 1, 1, 10, 1450.00, 250.00, NULL, 0),
(1002, 2, 2, 10, 420.00, 100.00, NULL, 0),

(1003, 1, 5, 20, 180.00, 0.00, 4, 0),

(1004, 1, 1, 12, 1450.00, 500.00, 2, 0),
(1004, 2, 3, 12, 999.00, 1000.00, 2, 0);
GO

INSERT INTO audit.ImportRunLog
(
    RunStartedOn, SourceSystem, RowsRead, RowsImported, ResultMessage
)
VALUES
('2025-11-01T01:00:00', 'CRM-V1', 1200, 1194, 'Completed with 6 rejected rows'),
('2025-11-02T01:00:00', 'EDI', 800, 800, 'Completed'),
('2025-11-03T01:00:00', 'PORTAL', 450, 447, '3 rows skipped due to malformed postcode');
GO