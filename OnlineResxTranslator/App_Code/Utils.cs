using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Web;

/// <summary>
/// Zusammenfassungsbeschreibung für Utils
/// </summary>
public static class Utils {

    public static string HTMLDecode(string sText)
    {
        sText = sText.Replace("&quot;", "" + (char)(34));
        sText = sText.Replace("&lt;", "" + (char)(60));
        sText = sText.Replace("&gt;", "" + (char)(62));
        sText = sText.Replace("&amp;", "" + (char)(38));
        sText = sText.Replace("&nbsp;", "" + (char)(32));
        for (int I = 1; I <= 255; I++)
            sText = sText.Replace("&#" + I + ";", "" + (char)(I));

        return sText;
    }


    public static bool CreateBackup(System.Xml.XmlDocument xmlDoc, string fullFilename)
    {
        const string BackupDirectory = "Backup";

        if (xmlDoc == null || fullFilename.Length == 0)
        {
            // Nothing to do
            return false;
        }
        else
        {
            string Filename = fullFilename.Split("\\".ToCharArray())[fullFilename.Split("\\".ToCharArray()).GetUpperBound(0)];
            string FilePath = fullFilename.Left(fullFilename.Length - Filename.Length) + BackupDirectory + "\\";
            if (!Directory.Exists(FilePath))
            {
                // could not create directory
                if (!Directory.CreateDirectory(FilePath).Exists) return false;
            }

            if (Directory.Exists(FilePath))
            {
                Filename += "." + DateTime.Now.ToString("yyyyMMdd-HHmmssffff") + ".bak";
                xmlDoc.Save(FilePath + Filename);

                return File.Exists(FilePath + Filename);
            }
        }
        return false;
    }

}
public static class StringExtensions {
    public static string Left(this string value, int maxLength)
    {
        if (string.IsNullOrEmpty(value)) return value;
        maxLength = Math.Abs(maxLength);

        return (value.Length <= maxLength
               ? value
               : value.Substring(0, maxLength)
               );
    }
}
public static class UriExtensions {
    /// <summary>
    ///     Adds query string value to an existing url, both absolute and relative URI's are supported.
    /// </summary>
    /// <example>
    /// <code>
    ///     // returns "www.domain.com/test?param1=val1&amp;param2=val2&amp;param3=val3"
    ///     new Uri("www.domain.com/test?param1=val1").ExtendQuery(new Dictionary&lt;string, string&gt; { { "param2", "val2" }, { "param3", "val3" } }); 
    /// 
    ///     // returns "/test?param1=val1&amp;param2=val2&amp;param3=val3"
    ///     new Uri("/test?param1=val1").ExtendQuery(new Dictionary&lt;string, string&gt; { { "param2", "val2" }, { "param3", "val3" } }); 
    /// </code>
    /// </example>
    /// <param name="uri"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    public static Uri ExtendQuery(this Uri uri, IDictionary<string, string> values)
    {
        var baseUrl = uri.ToString();
        var queryString = string.Empty;
        if (baseUrl.Contains("?"))
        {
            var urlSplit = baseUrl.Split('?');
            baseUrl = urlSplit[0];
            queryString = urlSplit.Length > 1 ? urlSplit[1] : string.Empty;
        }

        NameValueCollection queryCollection = HttpUtility.ParseQueryString(queryString);
        foreach (var kvp in values ?? new Dictionary<string, string>())
        {
            queryCollection[kvp.Key] = kvp.Value;
        }
        var uriKind = uri.IsAbsoluteUri ? UriKind.Absolute : UriKind.Relative;
        return queryCollection.Count == 0
          ? new Uri(baseUrl, uriKind)
          : new Uri(string.Format("{0}?{1}", baseUrl, queryCollection), uriKind);
    }

    /// <summary>
    ///     Adds query string value to an existing url, both absolute and relative URI's are supported.
    /// </summary>
    /// <example>
    /// <code>
    ///     // returns "www.domain.com/test?param1=val1&amp;param2=val2&amp;param3=val3"
    ///     new Uri("www.domain.com/test?param1=val1").ExtendQuery(new { param2 = "val2", param3 = "val3" }); 
    /// 
    ///     // returns "/test?param1=val1&amp;param2=val2&amp;param3=val3"
    ///     new Uri("/test?param1=val1").ExtendQuery(new { param2 = "val2", param3 = "val3" }); 
    /// </code>
    /// </example>
    /// <param name="uri"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    public static Uri ExtendQuery(this Uri uri, object values)
    {
        return ExtendQuery(uri, values.GetType().GetProperties().ToDictionary
        (
            propInfo => propInfo.Name,
            propInfo => { var value = propInfo.GetValue(values, null); return value != null ? value.ToString() : ""; }
        ));
    }
}