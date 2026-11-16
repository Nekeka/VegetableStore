namespace VegetableStore.Models;

public record VegetableModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public decimal Price { get; set; }

    public VegetableModel(int id, string name, string type, decimal price)
    {
        Id = id;
        Name = name;
        Type = type;
        Price = price;
    }
}
