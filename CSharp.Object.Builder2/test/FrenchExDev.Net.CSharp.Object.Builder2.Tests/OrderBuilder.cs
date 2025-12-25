namespace FrenchExDev.Net.CSharp.Object.Builder2.Tests;

public class OrderBuilder : AbstractBuilder<Order>
{
    public string? CustomerId { get; set; }
    public int? Quantity { get; set; }
    public decimal? Price { get; set; }

    protected override Order Instantiate() => new()
    {
        CustomerId = Require(CustomerId),
        Quantity = Require(Quantity),
        Price = Require(Price)
    };

    public OrderBuilder WithCustomerId(string id) { CustomerId = id; return this; }
    public OrderBuilder WithQuantity(int qty) { Quantity = qty; return this; }
    public OrderBuilder WithPrice(decimal price) { Price = price; return this; }
}
