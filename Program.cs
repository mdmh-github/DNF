using System;
using System.IO;
using System.Linq;
using System.Text;

namespace DotNetFile
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine(PROVIDE_CORRECT_DATA);
                return;
            }

            string type = args[0].ToLower();
            string name = CleanString(args[1]);
            string content = CreateContent(type, name);

            string file = string.Concat(
                Directory.GetCurrentDirectory(),
                Path.DirectorySeparatorChar,
                name,
                ".cs"
            );

            Console.WriteLine(FILE_CREATED, file);

            File.WriteAllText(file, content);
        }

        static string CreateContent(string type, string name)
        {
            var sb = new StringBuilder(TEMPLATE);
            sb.Replace("_NAME_", name);
            sb.Replace("_TYPE_", type);
            sb.Replace("_NAMESPACE_", CreateNameSpace());
            return sb.ToString();
        }

        static string CreateNameSpace()
        {
            var currentDirectory = new DirectoryInfo(".");
            var projectDirectory = GetProjectDirectory(currentDirectory);

            var projectPathLength = projectDirectory.FullName.Length;

            var relativeDirectory =
                currentDirectory.Equals(projectDirectory) ?
                    currentDirectory.Name :
                    currentDirectory.FullName
                        .Substring(projectPathLength+1);

            //Console.WriteLine($"projectDirectory {projectDirectory.FullName}: {IsRoot(currentDirectory)}");

            return CleanString(relativeDirectory).Replace(Path.DirectorySeparatorChar, '.');

            DirectoryInfo GetProjectDirectory(DirectoryInfo d)
                => IsRoot(d) ? d : GetProjectDirectory(d.Parent);

            bool IsRoot(DirectoryInfo d)
                => (d.Parent is null || d.GetFiles("*.csproj").Any() || d.FullName.Length < 3);
        }

        static string CleanString(string s)
        {
            var sb = new StringBuilder(s);

            foreach (var item in STRINGS_TO_REMOVE)
            {
                sb.Replace(item, string.Empty);
            }

            return sb.ToString();
        }

        static string[] STRINGS_TO_REMOVE = { " ", "." };

        static string
            FILE_CREATED = "{0} has been created",
            PROVIDE_CORRECT_DATA = "You need to provide the  type and the name of the file",
            TEMPLATE = @"using System;

namespace _NAMESPACE_
{
    _TYPE_ _NAME_
    {
    }
}";
    }
}