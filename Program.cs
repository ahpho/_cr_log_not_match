// See https://aka.ms/new-console-template for more information

const string rootDir = @"D:\Users\JT\Desktop\不同步4月10\2024-04-10";
int n;



//先解压
n = FileHelper.UnzipDir(rootDir);
Console.WriteLine($"UnzipDir done={n}");

//把有Exception的拷贝别处
string rootDirException = rootDir + "-exception";
n = SortByException.Do(rootDir, rootDirException);
Console.WriteLine($"SortByException done={n}");
n = SortByException.DeleteFromRoot(rootDir, rootDirException);
Console.WriteLine($"SortByException.DeleteFromRoot done={n}");

//把有Packet的拷贝别处
string rootDirPacket = rootDir + "-packet";
n = SortByNetPacket.Do(rootDir, rootDirPacket);
Console.WriteLine($"SortByNetPacket done={n}");
n = SortByNetPacket.DeleteFromRoot(rootDir, rootDirPacket);
Console.WriteLine($"SortByException.DeleteFromRoot done={n}");


//把有子弹差异的拷贝别处
string rootDirBullet = rootDir + "-bullet";
n = SortByBullet.Do(rootDir, rootDirBullet);
Console.WriteLine($"SortByBullet done={n}");
n = SortByBullet.DeleteFromRoot(rootDir, rootDirBullet);
Console.WriteLine($"SortByBullet.DeleteFromRoot done={n}");

