namespace TimeCafeWinUI3.Models;

public class CrossInfo
{
    public float X { get; set; }
    public float Y { get; set; }
    public double Size { get; set; }
    public double Angle { get; set; }
    public string TariffId { get; set; } // Уникальный идентификатор тарифа
    public double OriginalWidth { get; set; } // Исходная ширина Border
    public double OriginalHeight { get; set; } // Исходная высота Border
}