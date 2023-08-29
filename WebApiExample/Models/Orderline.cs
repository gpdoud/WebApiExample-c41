using System.Text.Json.Serialization;

namespace WebApiExample.Models;

public class Orderline {

    public int Id { get; set; }
    public int Quantity { get; set; } = 1;

    public int OrderId { get; set; }
    public virtual Order? Order { get; set; }

    public int ItemId { get; set; }
    [JsonIgnore]
    public virtual Item? Item { get; set; }

}
