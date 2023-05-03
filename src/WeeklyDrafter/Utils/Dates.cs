using System.Globalization;

public static class Dates
{
  // Return today's date or the last Monday (which is the unversal start of the week, sorry 🇺🇸)
  public static DateTime GetMonday()
  {
    var now = DateTime.UtcNow;

    // Let's go back in time (this cannot happen for too long right?)
    while (now.DayOfWeek != DayOfWeek.Monday)
    {
      now = now.AddDays(-1);
    }

    return now;
  }

  // Format a date time to an English string (e.g. "April 17, 2023").
  public static string ToEnglish(this DateTime dateTime)
  {
    return dateTime.ToString("MMMM d, yyyy",
                   CultureInfo.CreateSpecificCulture("en-us"));
  }

  // Format a date time to a sortable format (e.g. "2023-04-17").
  public static string ToSortable(this DateTime dateTime)
  {
    return dateTime.ToString("yyyy-MM-dd",
                   CultureInfo.CreateSpecificCulture("en-us"));
  }
}