using System;
using System.IO;
using System.Collections;
using System.Data;
using System.IO.Compression;
using System.Diagnostics;

public class FileHelper
{
    public static int UnzipDir(string strDir)
    {
        DirectoryInfo di = new DirectoryInfo(strDir);
        return UnzipDir(di);
    }

    public static int UnzipDir(DirectoryInfo di)
    {
        int n = 0;
        foreach (FileInfo fi in di.GetFiles())
        {
            if (fi.Name.ToLower().EndsWith(".7z"))
            {
                UnzipFile(di, fi);
                n++;
            }
        }
        foreach (DirectoryInfo subdi in di.GetDirectories())
        {
            n += UnzipDir(subdi);
        }
        return n;
    }

    public static void UnzipFile(DirectoryInfo di, FileInfo fi)
    {
        string strTmpDir = fi.Name + "_";
        di.CreateSubdirectory(strTmpDir);

        string strTmpFull = di.FullName + "\\" + strTmpDir;
        DirectoryInfo diTmp = new DirectoryInfo(strTmpFull);
        Directory.SetCurrentDirectory(diTmp.FullName);

        string tmp7zFile = di.FullName + "\\" + strTmpDir + "\\" + fi.Name;
        File.Copy(fi.FullName, tmp7zFile);
        Call7zip(tmp7zFile);
        File.Delete(tmp7zFile);

        foreach (FileInfo extracted in diTmp.GetFiles())
        {
            string dst = diTmp.FullName + "\\..\\" + extracted.Name + ".#.LOG";
            if (!File.Exists(dst))
                extracted.MoveTo(dst);
        }

        Directory.SetCurrentDirectory(di.FullName);
        diTmp.Delete();
    }

    public static void Call7zip(string str7z)
    {
        // 创建一个新的进程来运行7-Zip
        Process process = new Process();
        process.StartInfo.FileName = @"D:\Program Files\7-Zip\7z.exe";
        process.StartInfo.Arguments = string.Format($"x \"{str7z}\"");
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;
        process.StartInfo.CreateNoWindow = true;

        // 启动7-Zip进程并等待其完成
        process.Start();
        process.WaitForExit();
    }

    public static string ReadFile(string file)
    {
        var sr = new StreamReader(file);
        string content = sr.ReadToEnd();
        sr.Close();
        return content;
    }
}
