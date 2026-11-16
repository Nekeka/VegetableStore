using System.Globalization;
using System.Net;
using System.Text;
using VegetableStore.Models;

namespace VegetableStore.Rendering;

/// <summary>
/// Збирає HTML-сторінки з шаблонів у папці Templates.
/// ✅ Цей клас уже готовий. У уроці 2 змінювати його НЕ потрібно —
/// але прочитай його: тут немає нічого, чого ти не бачив в уроці 1.
/// </summary>
public class PageRenderer
{
    private static readonly CultureInfo Ukrainian = CultureInfo.GetCultureInfo("uk-UA");

    private readonly string _templatesPath;

    // IWebHostEnvironment сюди передає DI-контейнер (див. Program.cs → AddSingleton<PageRenderer>)
    public PageRenderer(IWebHostEnvironment env)
    {
        // ContentRootPath — папка проєкту. Шаблони лежать НЕ в wwwroot,
        // бо все з wwwroot доступне будь-кому напряму через браузер.
        _templatesPath = Path.Combine(env.ContentRootPath, "Templates");
    }

    /// <summary>Головна сторінка: форма додавання + таблиця овочів.</summary>
    /// <param name="vegetables">Що показати в таблиці.</param>
    /// <param name="errors">Помилки валідації (урок 2, крок 3).</param>
    /// <param name="antiforgeryField">Прихований input з токеном (урок 2, крок 4).</param>
    /// <param name="name">Значення, яке треба повернути у форму після помилки.</param>
    /// <param name="type">Значення, яке треба повернути у форму після помилки.</param>
    /// <param name="price">Значення, яке треба повернути у форму після помилки.</param>
    /// <param name="statusCode">HTTP-статус відповіді (200, 400...).</param>
    public IResult Index(
        IEnumerable<VegetableModel> vegetables,
        IEnumerable<string>? errors = null,
        string antiforgeryField = "",
        string name = "",
        string type = "",
        decimal? price = null,
        int statusCode = StatusCodes.Status200OK)
    {
        var list = vegetables.ToList();
        var rows = new StringBuilder();

        if (list.Count == 0)
        {
            rows.Append("""<tr><td colspan="5" class="empty">Поки що немає жодного овоча. Додай перший 👆</td></tr>""");
        }

        foreach (var v in list)
        {
            rows.Append($"""
                <tr>
                    <td class="muted">#{v.Id}</td>
                    <td><strong>{Encode(v.Name)}</strong></td>
                    <td><span class="badge">{Encode(v.Type)}</span></td>
                    <td class="num">{v.Price.ToString("0.00", Ukrainian)} грн</td>
                    <td class="actions">
                        <a class="btn btn-secondary btn-sm" href="/vegetables/{v.Id}/edit">Редагувати</a>
                        <form method="post" action="/vegetables/{v.Id}/delete" class="inline"
                              onsubmit="return confirm('Видалити цей овоч?');">
                            {antiforgeryField}
                            <button type="submit" class="btn btn-danger btn-sm">Видалити</button>
                        </form>
                    </td>
                </tr>
                """);
        }

        var html = Render("index.html", new()
        {
            ["errors"] = RenderErrors(errors),
            ["antiforgery"] = antiforgeryField,
            ["name"] = Encode(name),
            ["type"] = Encode(type),
            ["price"] = FormatForInput(price),
            ["count"] = list.Count.ToString(),
            ["rows"] = rows.ToString()
        });

        return Results.Content(html, "text/html", Encoding.UTF8, statusCode);
    }

    /// <summary>Сторінка редагування одного овоча.</summary>
    public IResult Edit(
        VegetableModel vegetable,
        IEnumerable<string>? errors = null,
        string antiforgeryField = "",
        int statusCode = StatusCodes.Status200OK)
    {
        var html = Render("edit.html", new()
        {
            ["errors"] = RenderErrors(errors),
            ["antiforgery"] = antiforgeryField,
            ["id"] = vegetable.Id.ToString(),
            ["name"] = Encode(vegetable.Name),
            ["type"] = Encode(vegetable.Type),
            ["price"] = FormatForInput(vegetable.Price)
        });

        return Results.Content(html, "text/html", Encoding.UTF8, statusCode);
    }

    /// <summary>Гарна сторінка 404.</summary>
    public IResult NotFound(string message)
    {
        var html = Render("message.html", new()
        {
            ["title"] = "Не знайдено",
            ["message"] = Encode(message)
        });

        return Results.Content(html, "text/html", Encoding.UTF8, StatusCodes.Status404NotFound);
    }

    // ---------- допоміжні методи ----------

    private string Render(string templateName, Dictionary<string, string> values)
    {
        var html = File.ReadAllText(Path.Combine(_templatesPath, templateName));

        foreach (var (key, value) in values)
        {
            html = html.Replace("{{" + key + "}}", value);
        }

        return html;
    }

    private static string RenderErrors(IEnumerable<string>? errors)
    {
        var list = errors?.ToList() ?? new List<string>();
        if (list.Count == 0)
            return string.Empty;

        var sb = new StringBuilder("""<div class="alert alert-error"><strong>Перевір форму:</strong><ul>""");
        foreach (var error in list)
        {
            sb.Append($"<li>{Encode(error)}</li>");
        }
        sb.Append("</ul></div>");
        return sb.ToString();
    }

    // <input type="number"> розуміє лише крапку, тому InvariantCulture
    private static string FormatForInput(decimal? price) =>
        price?.ToString(CultureInfo.InvariantCulture) ?? string.Empty;

    private static string Encode(string text) => WebUtility.HtmlEncode(text);
}
