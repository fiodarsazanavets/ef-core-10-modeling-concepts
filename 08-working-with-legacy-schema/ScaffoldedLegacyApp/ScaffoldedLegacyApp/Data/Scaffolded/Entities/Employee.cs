using System;
using System.Collections.Generic;

namespace ContosoLegacySales.Data.Scaffolded.Entities;

public partial class Employee
{
    public int EmpID { get; set; }

    public string EmpNo { get; set; } = null!;

    public string FullNm { get; set; } = null!;

    public string? EmailAddr { get; set; }

    public int? ManagerEmpID { get; set; }

    public string IsSalesRepFlag { get; set; } = null!;

    public DateTime HireDt { get; set; }

    public DateTime? TerminatedDt { get; set; }

    public virtual ICollection<Employee> InverseManagerEmp { get; set; } = new List<Employee>();

    public virtual Employee? ManagerEmp { get; set; }

    public virtual ICollection<OrderHdr> OrderHdrs { get; set; } = new List<OrderHdr>();
}
