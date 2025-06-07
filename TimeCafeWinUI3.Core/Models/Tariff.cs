namespace TimeCafeWinUI3.Core.Models;

public partial class Tariff
{
    public int TariffId { get; set; }

    public string TariffName { get; set; }

    public decimal HourlyRate { get; set; }

    public decimal MinutelyRate { get; set; }

    public byte[] Icon { get; set; }

    public int? ThemeId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime LastModified { get; set; }

    public virtual Theme Theme { get; set; }

    public virtual ICollection<Visit> Visits { get; set; } = new List<Visit>();
}
