using System;
using System.Collections.Generic;

namespace TimeCafeWinUI3.Core.Models;

public partial class BillingType
{
    public int BillingTypeId { get; set; }

    public string BillingTypeName { get; set; }

    public virtual ICollection<Tariff> Tariffs { get; set; } = new List<Tariff>();

    public virtual ICollection<Visit> Visits { get; set; } = new List<Visit>();
}
