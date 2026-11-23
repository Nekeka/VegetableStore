using Microsoft.AspNetCore.Mvc;
using VegetableStore.Data;
using VegetableStore.Rendering;

namespace VegetableStore;

public class  Program
{
    // 2.1
    private static readonly InMemoryVegetableRepository Repository = new();

    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        
        // 2.2
        builder.Services.AddSingleton<PageRenderer>();

        // 2.4

        var app = builder.Build();

        // працюємо
        app.UseStaticFiles();

        

        app.MapGet("/", Index);                                                        
        app.MapPost("/vegetables", AddVegetable).DisableAntiforgery();                
        app.MapGet("/vegetables/{id:int}/edit", EditForm);                             // update форма
        app.MapPost("/vegetables/{id:int}/edit", UpdateVegetable).DisableAntiforgery(); //update збереження
        app.MapPost("/vegetables/{id:int}/delete", DeleteVegetable);                   //delete

        app.Run();
    }

    
    private static IResult Index(PageRenderer pages)
    {
        return pages.Index(Repository.GetAll());
    }


    private static IResult AddVegetable(
        [FromForm] string name,
        [FromForm] string type,
        [FromForm] decimal price)
    {
        //2.3
        Repository.Add(name, type, price);
        return Results.Redirect("/");
    }


    private static IResult EditForm(int id, PageRenderer pages)
    {
        var vegetable = Repository.GetById(id);
        if (vegetable is null)
            return pages.NotFound($"Овоч з id={id} не знайдено");

        return pages.Edit(vegetable);
    }


    private static IResult UpdateVegetable(
        int id,
        [FromForm] string name,
        [FromForm] string type,
        [FromForm] decimal price,
        PageRenderer pages)
    {
        // 2.3
        if (!Repository.Update(id, name, type, price))
            return pages.NotFound($"Овоч з id={id} не знайдено");

        return Results.Redirect("/");
    }


    private static IResult DeleteVegetable(int id, PageRenderer pages)
    {
        if (!Repository.Delete(id))
            return pages.NotFound($"Овоч з id={id} не знайдено");

        return Results.Redirect("/");
    }
}
