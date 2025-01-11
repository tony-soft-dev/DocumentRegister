using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentRegister.Helpers
{
    static class CleanFolders
    {

        public static List<string> GetFileNames(string[] parentList)
        {
            List<string> nameList = new List<string>();
            foreach (var path in parentList)
            {
                nameList.Add(new DirectoryInfo(path).Name);
            }

            return nameList;
        }



        //private static string
    }
}
