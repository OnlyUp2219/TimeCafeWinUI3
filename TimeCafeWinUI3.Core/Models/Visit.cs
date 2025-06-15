using System;
using System.Collections.Generic;

namespace TimeCafeWinUI3.Core.Models;

public partial class Visit
{
    public int VisitId { get; set; }

    public int? ClientId { get; set; }

    public int? TariffId { get; set; }

    public int? BillingTypeId { get; set; }

    public DateTime EntryTime { get; set; }

    public DateTime? ExitTime { get; set; }

    public decimal? VisitCost { get; set; }

    public virtual BillingType BillingType { get; set; }

    public virtual Client Client { get; set; }

    public virtual ICollection<FinancialTransaction> FinancialTransactions { get; set; } = new List<FinancialTransaction>();

    public virtual Tariff Tariff { get; set; }
}
