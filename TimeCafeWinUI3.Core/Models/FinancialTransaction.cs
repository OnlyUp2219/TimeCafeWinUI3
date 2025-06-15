using System;
using System.Collections.Generic;

namespace TimeCafeWinUI3.Core.Models;

public partial class FinancialTransaction
{
    public int TransactionId { get; set; }

    public int? ClientId { get; set; }

    public decimal Amount { get; set; }

    public DateTime TransactionDate { get; set; }

    public int? TransactionTypeId { get; set; }

    public int? VisitId { get; set; }

    public virtual Client Client { get; set; }

    public virtual TransactionType TransactionType { get; set; }

    public virtual Visit Visit { get; set; }
}
