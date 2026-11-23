using VegetableStore.Models;

namespace VegetableStore.Data;

public class InMemoryVegetableRepository
{
    private int _nextId = 6;

    private readonly List<VegetableModel> _vegetables = new()
    {
        new(1, "Cucumber", "Vegetable", 3.5m),
        new(2, "Carrot",   "Crop",      2.50m),
        new(3, "Potato",   "Vegetable", 1.50m),
        new(4, "Pumpkin",  "Vegetable", 8m),
        new(5, "Pea",      "Beans",     1m)
    };

    public IReadOnlyList<VegetableModel> GetAll()
    {
        return _vegetables.ToList();
    }

    public VegetableModel? GetById(int id)
    {
        return _vegetables.FirstOrDefault(v => v.Id == id);
    }

    public VegetableModel Add(string name, string type, decimal price)
    {
        var vegetable = new VegetableModel(_nextId++, name, type, price);
        _vegetables.Add(vegetable);
        return vegetable;
    }

    public bool Update(int id, string name, string type, decimal price)
    {
        var vegetable = GetById(id);
        if (vegetable is null)
            return false;

        vegetable.Name = name;
        vegetable.Type = type;
        vegetable.Price = price;
        return true;
    }

    public bool Delete(int id)
    {
        return _vegetables.RemoveAll(v => v.Id == id) > 0;
    }
}
