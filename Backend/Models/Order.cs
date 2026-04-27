using System;
using System.Collections.Generic;

namespace Backend.Models;

public class Order
{
    public int Id { get; set; }
    
    // Customer Info
    public int CustomerId { get; set; }
    public User? Customer { get; set; }
    
    public List<OrderProduct> OrderProducts { get; set; } = new();
    
    public decimal TotalPrice { get; set; }
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
}
