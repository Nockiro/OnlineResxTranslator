using System;
using System.Collections.Generic;
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