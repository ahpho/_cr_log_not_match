using System;
using System.IO;
using System.Collections;
using System.Data;
using System.IO.Compression;
using System.Diagnostics;
using System.Reflection;

public class SortByNetPacket
{
    public static int Do(string inDir, string outDir)
    {
        if (!Directory.Exists(outDir)) Directory.CreateDirectory(outDir);

        int movedCount = 0;
        DirectoryInfo inDi = new DirectoryInfo(inDir);
        foreach (DirectoryInfo di in inDi.GetDirectories())
        {
            foreach (FileInfo fi in di.GetFiles("*.#.LOG"))
            {
                string content = FileHelper.ReadFile(fi.FullName);
                if (content.Contains("[T="))
                {
                    string targetDir = Path.Combine(outDir, di.Name);
                    if (!Directory.Exists(targetDir))
                        Directory.CreateDirectory(targetDir);
                    File.Move(fi.FullName, targetDir + "\\" + fi.Name);
                    movedCount++;
                }
            }
        }
        return movedCount;
    }

    public static int DeleteFromRoot(string oldDir, string copiedDir)
    {
        int deletedCount = 0;
        Directory.SetCurrentDirectory(oldDir);
        
        DirectoryInfo exDi = new DirectoryInfo(copiedDir);
        foreach (DirectoryInfo di in exDi.GetDirectories())
        {
            try
            {
                Directory.Delete(di.Name, true);
                deletedCount++;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }
        return deletedCount;
    }
}
