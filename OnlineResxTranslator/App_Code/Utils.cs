using System;
using System.IO;
using Extensions;

/// <summary>
/// Zusammenfassungsbeschreibung für Utils
/// </summary>
public static class Utils
{
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