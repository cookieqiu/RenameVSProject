using RenameVSProject;
using System.Diagnostics;
using System.Text.RegularExpressions;

var path = ReadLine("请输入项目所在路径：", Directory.Exists);
var srcStr = ReadLine("请输入要替换的字符串：", null);
var tarStr = ReadLine("请输入新字符串：", null);

var sw = new Stopwatch();
sw.Start();

var srcRegex = new Regex(srcStr, RegexOptions.Compiled | RegexOptions.IgnoreCase);
ScanDirectory(path, srcRegex, tarStr);

sw.Stop();
Console.WriteLine(sw.Elapsed);

void ScanDirectory(string path, in Regex src, in string target)
{
    var files = Directory.GetFiles(path).FileFilter();
    foreach (var filePath in files)
    {
        var fileName = Path.GetFileName(filePath);
        var basePath = Path.GetDirectoryName(filePath)!;

        ReplaceFileContent(filePath, src, target);

        if (!src.IsMatch(fileName)) continue;

        var newFileName = src.Replace(fileName, target);
        var newFilePath = Path.Combine(basePath, newFileName);
        File.Move(filePath, newFilePath);
    }

    var dirs = Directory.GetDirectories(path).IgnoreDirFilter();
    foreach (var dirPath in dirs)
    {
        var dirName = Path.GetFileName(dirPath);

        ScanDirectory(dirPath, src, target);
    }

    var rootName = Path.GetFileName(path);
    var rootBasePath = Path.GetDirectoryName(path)!;

    if (src.IsMatch(rootName))
    {
        var newDirName = src.Replace(rootName, target);
        var newDirPath = Path.Combine(rootBasePath, newDirName);
        Directory.Move(path, newDirPath);
    }
}

void ReplaceFileContent(string filePath, in Regex src, in string target)
{
    var content = string.Empty;
    using (var reader = new StreamReader(filePath))
    {
        content = reader.ReadToEnd();
        if (content == null) return;
    }

    var newContent = src.Replace(content, target);
    using var writer = new StreamWriter(filePath);

    writer.Write(newContent);
}

string ReadLine(string msg, Predicate<string>? pred)
{
    var line = string.Empty;
    var isFirst = true;

    while (string.IsNullOrEmpty(line) || !(pred?.Invoke(line) ?? true))
    {
        if (!isFirst)
        {
            Console.Write("格式错误，");
        }
        else
        {
            isFirst = false;
        }

        Console.WriteLine(msg);
        line = Console.ReadLine()?.Trim();
    }
    return line;
}
