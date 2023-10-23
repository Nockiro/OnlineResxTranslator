using System;
using System.IO;
using System.Net;

/// <summary>
/// Files for Transfer over the FTP Protocol
/// Licensed under CPOL // taken from: https://www.codeproject.com/Tips/443588/Simple-Csharp-FTP-Class
/// Modified for SSL use
/// </summary>
public class FTP
{
    private string host = null;
    private string user = null;
    private string pass = null;
    private Boolean ssl = true;
    private FtpWebRequest ftpRequest = null;
    private FtpWebResponse ftpResponse = null;
    private Stream ftpStream = null;
    private int bufferSize = 2048;

    /* Construct Object */

    public FTP(string hostIP, string userName, string password, bool ssl = true)
    { host = hostIP; user = userName; pass = password; this.ssl = ssl; }

    /* Download File */

    public void download(string remoteFile, string localFile)
    {
        /* Create an FTP Request */
        ftpRequest = (FtpWebRequest)FtpWebRequest.Create(host + "/" + remoteFile);
        /* Log in to the FTP Server with the User Name and Password Provided */
        ftpRequest.Credentials = new NetworkCredential(user, pass);
        /* When in doubt, use these options */
        ftpRequest.UseBinary = true;
        ftpRequest.UsePassive = true;
        ftpRequest.KeepAlive = true;
        ftpRequest.EnableSsl = ssl;
        /* Specify the Type of FTP Request */
        ftpRequest.Method = WebRequestMethods.Ftp.DownloadFile;
        /* Establish Return Communication with the FTP Server */
        ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
        /* Get the FTP Server's Response Stream */
        ftpStream = ftpResponse.GetResponseStream();
        /* Open a File Stream to Write the Downloaded File */
        //FileStream localFileStream = new FileStream(localFile, FileMode.Create);
        FileStream localFileStream = new FileStream(localFile, FileMode.Open, FileAccess.ReadWrite);

        /* Buffer for the Downloaded Data */
        byte[] byteBuffer = new byte[bufferSize];
        int bytesRead = ftpStream.Read(byteBuffer, 0, bufferSize);
        /* Download the File by Writing the Buffered Data Until the Transfer is Complete */
        while (bytesRead > 0)
        {
            localFileStream.Write(byteBuffer, 0, bytesRead);
            bytesRead = ftpStream.Read(byteBuffer, 0, bufferSize);
        }
        /* Resource Cleanup */
        localFileStream.Close();
        ftpStream.Close();
        ftpResponse.Close();
        ftpRequest = null;
        return;
    }

    /* Upload File */

    public void upload(string remoteFile, string localFile)
    {
        /* Create an FTP Request */
        ftpRequest = (FtpWebRequest)FtpWebRequest.Create(host + "/" + remoteFile);
        /* Log in to the FTP Server with the User Name and Password Provided */
        ftpRequest.Credentials = new NetworkCredential(user, pass);
        /* When in doubt, use these options */
        ftpRequest.UseBinary = true;
        ftpRequest.UsePassive = true;
        ftpRequest.KeepAlive = true;
        ftpRequest.EnableSsl = ssl;
        /* Specify the Type of FTP Request */
        ftpRequest.Method = WebRequestMethods.Ftp.UploadFile;
        /* Establish Return Communication with the FTP Server */
        ftpStream = ftpRequest.GetRequestStream();
        /* Open a File Stream to Read the File for Upload */
        FileStream localFileStream = new FileStream(localFile, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        /* Buffer for the Downloaded Data */
        byte[] byteBuffer = new byte[bufferSize];
        int bytesSent = localFileStream.Read(byteBuffer, 0, bufferSize);
        /* Upload the File by Sending the Buffered Data Until the Transfer is Complete */
        while (bytesSent != 0)
        {
            ftpStream.Write(byteBuffer, 0, bytesSent);
            bytesSent = localFileStream.Read(byteBuffer, 0, bufferSize);
        }
        /* Resource Cleanup */
        localFileStream.Close();
        ftpStream.Close();
        ftpRequest = null;
        return;
    }

    /* Delete File */

    public void delete(string deleteFile)
    {
        /* Create an FTP Request */
        ftpRequest = (FtpWebRequest)WebRequest.Create(host + "/" + deleteFile);
        /* Log in to the FTP Server with the User Name and Password Provided */
        ftpRequest.Credentials = new NetworkCredential(user, pass);
        /* When in doubt, use these options */
        ftpRequest.UseBinary = true;
        ftpRequest.UsePassive = true;
        ftpRequest.KeepAlive = true;
        ftpRequest.EnableSsl = ssl;
        /* Specify the Type of FTP Request */
        ftpRequest.Method = WebRequestMethods.Ftp.DeleteFile;
        /* Establish Return Communication with the FTP Server */
        ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
        /* Resource Cleanup */
        ftpResponse.Close();
        ftpRequest = null;
        return;
    }

    /* Rename File */

    public void rename(string currentFileNameAndPath, string newFileName)
    {
        try
        {
            /* Create an FTP Request */
            ftpRequest = (FtpWebRequest)WebRequest.Create(host + "/" + currentFileNameAndPath);
            /* Log in to the FTP Server with the User Name and Password Provided */
            ftpRequest.Credentials = new NetworkCredential(user, pass);
            /* When in doubt, use these options */
            ftpRequest.UseBinary = true;
            ftpRequest.UsePassive = true;
            ftpRequest.EnableSsl = ssl;
            ftpRequest.KeepAlive = true;
            /* Specify the Type of FTP Request */
            ftpRequest.Method = WebRequestMethods.Ftp.Rename;
            /* Rename the File */
            ftpRequest.RenameTo = newFileName;
            /* Establish Return Communication with the FTP Server */
            ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
            /* Resource Cleanup */
            ftpResponse.Close();
            ftpRequest = null;
        }
        catch (Exception ex) { Console.WriteLine(ex.ToString()); }
        return;
    }

    /* Create a New Directory on the FTP Server */

    public void createDirectory(string newDirectory)
    {
        try
        {
            /* Create an FTP Request */
            ftpRequest = (FtpWebRequest)WebRequest.Create(host + "/" + newDirectory);
            /* Log in to the FTP Server with the User Name and Password Provided */
            ftpRequest.Credentials = new NetworkCredential(user, pass);
            /* When in doubt, use these options */
            ftpRequest.UseBinary = true;
            ftpRequest.UsePassive = true;
            ftpRequest.KeepAlive = true;
            /* Specify the Type of FTP Request */
            ftpRequest.Method = WebRequestMethods.Ftp.MakeDirectory;
            /* Establish Return Communication with the FTP Server */
            ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
            /* Resource Cleanup */
            ftpResponse.Close();
            ftpRequest = null;
        }
        catch (Exception ex) { Console.WriteLine(ex.ToString()); }
        return;
    }

    /* Get the Size of a File */

    public string getFileSize(string fileName)
    {
        try
        {
            /* Create an FTP Request */
            ftpRequest = (FtpWebRequest)FtpWebRequest.Create(host + "/" + fileName);
            /* Log in to the FTP Server with the User Name and Password Provided */
            ftpRequest.Credentials = new NetworkCredential(user, pass);
            /* When in doubt, use these options */
            ftpRequest.UseBinary = true;
            ftpRequest.UsePassive = true;
            ftpRequest.KeepAlive = true;
            /* Specify the Type of FTP Request */
            ftpRequest.Method = WebRequestMethods.Ftp.GetFileSize;
            /* Establish Return Communication with the FTP Server */
            ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
            /* Establish Return Communication with the FTP Server */
            ftpStream = ftpResponse.GetResponseStream();
            /* Get the FTP Server's Response Stream */
            StreamReader ftpReader = new StreamReader(ftpStream);
            /* Store the Raw Response */
            string fileInfo = null;
            /* Read the Full Response Stream */
            try { while (ftpReader.Peek() != -1) { fileInfo = ftpReader.ReadToEnd(); } }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); }
            /* Resource Cleanup */
            ftpReader.Close();
            ftpStream.Close();
            ftpResponse.Close();
            ftpRequest = null;
            /* Return File Size */
            return fileInfo;
        }
        catch (Exception ex) { Console.WriteLine(ex.ToString()); }
        /* Return an Empty string Array if an Exception Occurs */
        return "";
    }

    /* List Directory Contents File/Folder Name Only */

    public string[] directoryListSimple(string directory)
    {
        /* Create an FTP Request */
        ftpRequest = (FtpWebRequest)FtpWebRequest.Create(host + "/" + directory);
        /* Log in to the FTP Server with the User Name and Password Provided */
        ftpRequest.Credentials = new NetworkCredential(user, pass);
        /* When in doubt, use these options */
        ftpRequest.UseBinary = true;
        ftpRequest.UsePassive = true;
        ftpRequest.KeepAlive = true;
        /* Specify the Type of FTP Request */
        ftpRequest.Method = WebRequestMethods.Ftp.ListDirectory;
        /* Establish Return Communication with the FTP Server */
        ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
        /* Establish Return Communication with the FTP Server */
        ftpStream = ftpResponse.GetResponseStream();
        /* Get the FTP Server's Response Stream */
        StreamReader ftpReader = new StreamReader(ftpStream);
        /* Store the Raw Response */
        string directoryRaw = null;
        /* Read Each Line of the Response and Append a Pipe to Each Line for Easy Parsing */
        try { while (ftpReader.Peek() != -1) { directoryRaw += ftpReader.ReadLine() + "|"; } }
        catch (Exception ex) { Console.WriteLine(ex.ToString()); }
        /* Resource Cleanup */
        ftpReader.Close();
        ftpStream.Close();
        ftpResponse.Close();
        ftpRequest = null;
        /* Return the Directory Listing as a string Array by Parsing 'directoryRaw' with the Delimiter you Append (I use | in This Example) */
        string[] directoryList = directoryRaw.Split("|".ToCharArray()); return directoryList;
    }

    /* List Directory Contents in Detail (Name, Size, Created, etc.) */

    public string[] directoryListDetailed(string directory)
    {
        /* Create an FTP Request */
        ftpRequest = (FtpWebRequest)FtpWebRequest.Create(host + "/" + directory);
        /* Log in to the FTP Server with the User Name and Password Provided */
        ftpRequest.Credentials = new NetworkCredential(user, pass);
        /* When in doubt, use these options */
        ftpRequest.UseBinary = true;
        ftpRequest.UsePassive = true;
        ftpRequest.KeepAlive = true;
        /* Specify the Type of FTP Request */
        ftpRequest.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
        /* Establish Return Communication with the FTP Server */
        ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
        /* Establish Return Communication with the FTP Server */
        ftpStream = ftpResponse.GetResponseStream();
        /* Get the FTP Server's Response Stream */
        StreamReader ftpReader = new StreamReader(ftpStream);
        /* Store the Raw Response */
        string directoryRaw = null;
        /* Read Each Line of the Response and Append a Pipe to Each Line for Easy Parsing */
        try { while (ftpReader.Peek() != -1) { directoryRaw += ftpReader.ReadLine() + "|"; } }
        catch (Exception ex) { Console.WriteLine(ex.ToString()); }
        /* Resource Cleanup */
        ftpReader.Close();
        ftpStream.Close();
        ftpResponse.Close();
        ftpRequest = null;
        /* Return the Directory Listing as a string Array by Parsing 'directoryRaw' with the Delimiter you Append (I use | in This Example) */
        try { string[] directoryList = directoryRaw.Split("|".ToCharArray()); return directoryList; }
        catch (Exception ex) { Console.WriteLine(ex.ToString()); }
        /* Return an Empty string Array if an Exception Occurs */
        return new string[] { "" };
    }
}