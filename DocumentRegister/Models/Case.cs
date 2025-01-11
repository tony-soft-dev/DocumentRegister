using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DocumentRegister.Models
{
    public class ToProcess
    {
        public ToProcess(string p)
        {
            Path = p;
            Name = p.Substring(p.LastIndexOf("\\") + 1);
            PForm = new Form(); 
        }
        public string Path { get; }
        public string Name { get; }
        public Form PForm { get; set; }
    }
    public class Case : Form
    {
        private string caseName;
        private string caseNumber;
        private string[] parentDirectories;
        private string[] parentFiles;
        private string excelPath;
        private List<ToProcess> toProcessesList = new List<ToProcess>();
        public Case() {}
        public Case(string parentPath)
        {
            ParentPath = parentPath;
            FormVals = new Form();
            ProcessedCount = 1;
            SetValues();
            InitializeProcessPaths();
        }
        public string ParentPath { get; }  
        public string CaseName { get { return caseName; } }
        public string CaseNumber { get { return caseNumber; } }
        public string[] ParentDirectories {  get { return parentDirectories; } }
        public string[] ParentFiles { get { return parentFiles; } }
        public string ExcelPath {  get { return excelPath; } }
        public List<ToProcess> ToProcessList { get { return toProcessesList; } set { } }
        public Form FormVals { get; set; }
        public int ProcessedCount { get; set; }
        private void InitializeProcessPaths()
        {
            if (ParentPath == null)
            {
                return;
            }
            else
            {
                for (int i = 0; i < ParentDirectories.Length - 1; i++)
                {
                    StringComparison comp = StringComparison.Ordinal;
                    if (ParentDirectories[i] != null && ParentDirectories[i].Contains("to be processed", comp))
                    {
                        List<string> files = Directory.GetFiles(ParentDirectories[i], "*.*")
                            .Where(file => !file.EndsWith("db"))
                            .ToList();
                        foreach (string file in files)
                        {
                            toProcessesList.Add(new ToProcess(file));
                        }
                    }
                }
            }
        }
        private void SetValues()
        {
            caseName = ParentPath.Substring(ParentPath.LastIndexOf("\\") + 1);

            caseNumber = string.IsNullOrEmpty(ParentPath) ? null :
                    ParentPath.Substring(ParentPath.IndexOf("[")).Trim(new char[] { '[', ']' }).Replace("-", "");
            parentDirectories = string.IsNullOrEmpty(ParentPath) ? null :
                    Directory.GetDirectories(ParentPath);
            parentFiles = string.IsNullOrEmpty(ParentPath) ? null :
                    Directory.GetFiles(ParentPath);
            excelPath = string.IsNullOrEmpty(ParentPath) ? null :
                    Array.Find(ParentFiles, path => path.EndsWith(".xlsx", StringComparison.Ordinal));
        }
    }
}
