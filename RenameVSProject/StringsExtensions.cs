using System.Text;
using System.Text.RegularExpressions;

namespace RenameVSProject
{
    public static class StringsExtensions
    {
        private static Regex[]? s_fileRegexes;

        private static Regex[] FileRegexes
        {
            get
            {
                if (s_fileRegexes == null)
                {
                    string[] filters = [
                        "*.cs",
                        "*.xaml",
                        "*.json",
                        "*.sln",
                        "*.csproj",
                        "*.xml"];
                    s_fileRegexes = CreateRegexes(filters);
                }
                return s_fileRegexes;
            }
        }

        private static Regex[]? s_ignoreDirRegexes;

        private static Regex[] IgnoreDirRegexes
        {
            get
            {
                if (s_ignoreDirRegexes == null)
                {
                    string[] filters = [
                        ".*",
                        "build",
                        "docs",
                        "lib",
                        "obj"];
                    s_ignoreDirRegexes = CreateRegexes(filters);
                }
                return s_ignoreDirRegexes;
            }
        }

        private static Regex[] CreateRegexes(in string[] filters)
        {
            var length = filters.Length;
            var results = new Regex[length];

            for (var i = 0; i < length; i++)
            {
                var regular = CreateRegularString(filters[i]);

                results[i] = new Regex(regular, RegexOptions.Compiled);
            }

            return results;
        }

        private static string CreateRegularString(string filter)
        {
            var sb = new StringBuilder(filter.Length << 1);

            sb.Append('^');

            for (var i = 0; i < filter.Length; i++)
            {
                var ch = filter[i];
                if (ch == '.')
                {
                    sb.Append("\\.");
                }
                else if (ch == '*')
                {
                    sb.Append(".+");
                }
                else
                {
                    sb.Append(ch);
                }
            }
            sb.Append('$');
            return sb.ToString();
        }

        public static string[] FileFilter(this string[] files)
        {
            var results = new List<string>(files.Length);

            foreach (var filePath in files)
            {
                var fileName = Path.GetFileName(filePath);
                foreach (var regex in FileRegexes)
                {
                    if (regex.IsMatch(fileName))
                    {
                        results.Add(filePath);
                    }
                }
            }
            return [.. results];
        }

        public static string[] IgnoreDirFilter(this string[] dirs)
        {
            var results = new List<string>(dirs.Length);

            foreach (var dirPath in dirs)
            {
                var dirName = Path.GetFileName(dirPath);
                var isIgnore = false;
                foreach (var regex in IgnoreDirRegexes)
                {
                    if (regex.IsMatch(dirName))
                    {
                        isIgnore = true;
                        break;
                    }
                }
                if (!isIgnore)
                {
                    results.Add(dirPath);
                }
            }
            return [.. results];
        }
    }
}
