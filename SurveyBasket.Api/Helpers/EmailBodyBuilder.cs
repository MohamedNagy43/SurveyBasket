using System.Text;

namespace SurveyBasket.Api.Helpers;

public static class EmailBodyBuilder
{
    public static async Task<string> BuildEmailBody(string templateName, Dictionary<string, string> templateModels)
    {
        string fullPath = $"{Directory.GetCurrentDirectory()}/Templates/{templateName}.html";
        string template = await File.ReadAllTextAsync(fullPath);

        var body = new StringBuilder(template);
        foreach (var model in templateModels)
        {
            body.Replace(model.Key, model.Value);
        }

        return body.ToString();
    }
}
