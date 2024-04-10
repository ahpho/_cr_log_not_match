using System;
using System.IO;
using System.Collections;
using System.Data;
using System.IO.Compression;
using System.Diagnostics;
using System.Reflection;
using System.Linq;

public class SortByBullet
{
    public static int Do(string inDir, string outDir)
    {
        if (!Directory.Exists(outDir)) Directory.CreateDirectory(outDir);

        int movedCount = 0;
        DirectoryInfo inDi = new DirectoryInfo(inDir);
        foreach (DirectoryInfo di in inDi.GetDirectories())
        {
            string errorPlayerID = string.Empty;
            string errorBulletStr = string.Empty;
            string normalBulletStr = string.Empty;

            FileInfo[] fullLogs = di.GetFiles("*.#.LOG");
            if (fullLogs.Length < 2)
            {
                Console.WriteLine("fullLogs less than 2!");
                return 0;
            }

            // 随便找一个#.LOG文件，找到错误的那位Player
            foreach (FileInfo fi in fullLogs)
            {
                string content = FileHelper.ReadFile(fi.FullName);
                if (!content.Contains("[FATAL ERROR]")) continue;
                string[] lines = content.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string line in lines)
                {
                    if (line.Contains("[FATAL ERROR]"))
                    {
                        int lastComma = line.LastIndexOf(',');
                        int secLastComma = line.LastIndexOf(',', lastComma - 2);
                        errorPlayerID = line.Substring(secLastComma + 1, lastComma - secLastComma - 1);
                        break;
                    }
                }
                if (string.IsNullOrEmpty(errorPlayerID))
                {
                    Console.WriteLine("Can't find errorPlayer Log!");
                    return 0;
                }
                break;
            }

            // 收集两种BulletStr
            foreach (FileInfo fi in fullLogs)
            {
                if (fi.Name.Contains(errorPlayerID + "_"))
                {
                    if (string.IsNullOrEmpty(errorBulletStr))
                    {
                        string content = FileHelper.ReadFile(fi.FullName);
                        if (!content.Contains("[FATAL ERROR]")) continue;
                        string[] lines = content.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string line in lines)
                        {
                            if (line.Contains(" Bullet:"))
                            {
                                int startIndex = line.IndexOf(" Bullet:");
                                int endIndex = line.IndexOf(";", startIndex + 1);
                                errorBulletStr += line.Substring(startIndex + 1, endIndex - startIndex);
                            }
                        }
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(normalBulletStr))
                    {
                        string content = FileHelper.ReadFile(fi.FullName);
                        if (!content.Contains("[FATAL ERROR]")) continue;
                        string[] lines = content.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string line in lines)
                        {
                            if (line.Contains(" Bullet:"))
                            {
                                int startIndex = line.IndexOf(" Bullet:");
                                int endIndex = line.IndexOf(";", startIndex + 1);
                                normalBulletStr += line.Substring(startIndex + 1, endIndex - startIndex);
                            }
                        }
                    }
                }
            }

            // 比较两个BulletStr，不相等则全部#.LOG都拷贝过去
            if (errorBulletStr != normalBulletStr)
            {
                foreach (FileInfo fi in fullLogs)
                {
                    string targetDir = Path.Combine(outDir, di.Name);
                    if (!Directory.Exists(targetDir))
                        Directory.CreateDirectory(targetDir);
                    File.Move(fi.FullName, targetDir + "\\" + fi.Name);
                }
                movedCount++;
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
