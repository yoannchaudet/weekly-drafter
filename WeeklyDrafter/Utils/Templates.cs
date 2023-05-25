using System.Text;
using Scriban;
using Scriban.Runtime;

namespace weekly_drafter.Utils;

public static class Templates
{
  // Render a Liquid template provided from a string
  public static string RenderLiquidFromText(string text, object? context = null)
  {
    // Render
    var template = Template.ParseLiquid(text);
    return RenderTemplate(template, context);
  }

  public static string RenderLiquidFromFile(string path, object? context = null)
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

  private static string RenderTemplate(Template template, object? context)
  {
    // Don't render the template if it has errors
    if (template.HasErrors)
      Logger.Error($"Error parsing Liquid template{Environment.NewLine}{template.Messages}",
        new Logger.AnnotationProperties
        {
          File = template.SourceFilePath
        }, true);

    // Enrich the passed context
    var enrichedContext = new ScriptObject();
    if (context != null)
      enrichedContext.Import(context);
    enrichedContext.Add("dates", new DateUtils());
    enrichedContext.Add("markers", new MarkerUtils());

    // Return the rendered template
    return template.Render(enrichedContext).Trim();
  }

  // Date utils script object for templating
  private class DateUtils : ScriptObject
  {
    public static string Sortable(DateTime date)
    {
      return date.ToSortable();
    }

    public static string English(DateTime date)
    {
      return date.ToEnglish();
    }
  }

  // Marker utils script object for templating
  private class MarkerUtils : ScriptObject
  {
    public static Markers.Marker New(string name)
    {
      return new Markers.Marker(name);
    }

    public static Markers.Marker AddArgument(Markers.Marker marker, string key, string value)
    {
      return marker.AddArgument(key, value);
    }

    public static Markers.Marker JoinArgument(Markers.Marker marker, string key, List<string> values, char separator = ',')
    {
      if (values.Any())
        return AddArgument(marker, key, string.Join(separator, values));
      return marker;
    }

    public static string ToString(Markers.Marker marker)
    {
      return Markers.ToText(marker);
    }
  }
}