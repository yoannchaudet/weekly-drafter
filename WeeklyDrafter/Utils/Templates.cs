using System.Text;
using Scriban;

namespace weekly_drafter.Utils;

public static class Templates
{
  // Render a Liquid template provided from a string
  public static string RenderLiquidFromText(string text, object context)
  {
    // Render
    var template = Template.ParseLiquid(text);
    return RenderTemplate(template, context);
  }

  public static string RenderLiquidFromFile(string path, object context)
  {
    // Validate the file exists
    if (!File.Exists(path))
      Logger.Error($"File {path} does not exist, unable to parse Liquid template", new Logger.AnnotationProperties
      {
        File = path
      }, true);

    // Render
    var template = Template.ParseLiquid(File.ReadAllText(path, Encoding.UTF8), path);
    return RenderTemplate(template, context);
  }

  private static string RenderTemplate(Template template, object context)
  {
    // Don't render the template if it has errors
    if (template.HasErrors)
      Logger.Error($"Error parsing Liquid template{Environment.NewLine}{template.Messages}",
        new Logger.AnnotationProperties
        {
          File = template.SourceFilePath
        }, true);

    // Return the rendered template
    return template.Render(context).Trim();
  }
}