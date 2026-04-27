namespace Backend.Models;

public class OrderProduct
{
    public int OrderId { get; set; }
    public Order? Order { get; set; }

    public int ProductId { get; set; }
    public Product? Product { get; set; }

    public decimal Amount { get; set; }
    public int UnitId { get; set; }
    public Unit? Unit { get; set; }
}
