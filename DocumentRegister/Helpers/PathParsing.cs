using System;
using System.IO;
using System.Linq;

namespace DocumentRegister.Helpers
{
    static class PathParsing
    {
        public static bool HasFilesToProcess(string[] employeeParentFolders)
        {
            for (int i = 0; i < employeeParentFolders.Length; i++)
            {
                StringComparison comp = StringComparison.Ordinal;
                if (employeeParentFolders[i].Contains("to be processed", comp) && Directory.GetFiles(employeeParentFolders[i]).Where(file => !file.EndsWith("db")).ToList().Count() > 0)
                {
                    return true;
                }
            }
            return false;
        }
        public static string[] GetDocsToBeProcessed(string[] employeeParentFolders)
        {
            for (int i = 0; i < employeeParentFolders.Length; i++)
            {
                if (employeeParentFolders[i] == null) return null;


                StringComparison comp = StringComparison.Ordinal;
                if (employeeParentFolders[i].Contains("to be processed", comp))
                {
                    return Directory.GetFiles(employeeParentFolders[i]);
                }
            }
            return null;
        }

        private static bool Contains(this String str, String substring,
                            StringComparison comp)
        {
            if (substring == null)
                throw new ArgumentNullException("substring",
                                             "substring cannot be null.");
            else if (!Enum.IsDefined(typeof(StringComparison), comp))
                throw new ArgumentException("comp is not a member of StringComparison",
                                         "comp");

            return str.IndexOf(substring, comp) >= 0;
        }

        private static string GetExcel(string[] employeeParentFiles)
        {
            return Array.Find(employeeParentFiles, path => path.EndsWith(".xlsx", StringComparison.Ordinal));
        }
    }
}
