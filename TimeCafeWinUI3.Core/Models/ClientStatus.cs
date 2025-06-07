namespace TimeCafeWinUI3.Core.Models;

public enum ClientStatusType
{
    Draft = 1,           // Черновик
    Active = 2,          // Активный
    Rejected = 3         // Отказ от услуг
}

public partial class ClientStatus
{
    public int StatusId { get; set; }

    public string StatusName { get; set; }

    public virtual ICollection<Client> Clients { get; set; } = new List<Client>();
}
