using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace TimeCafeWinUI3.Core.Models;

public partial class Visit : INotifyPropertyChanged
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

    // UI Properties
    private string _durationText;
    public string DurationText
    {
        get => _durationText;
        set
        {
            _durationText = value;
            OnPropertyChanged(nameof(DurationText));
        }
    }

    private decimal _currentCost;
    public decimal CurrentCost
    {
        get => _currentCost;
        set
        {
            _currentCost = value;
            OnPropertyChanged(nameof(CurrentCost));
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
