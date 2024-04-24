using DocumentRegister.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Input;

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
        private void Setup()
        {

        }
    }
    public class Case : Form
    {
        private string caseName = "huuh";
        private string caseNumber;
        private string[] parentDirectories;
        private string[] parentFiles;
        private string excelPath;
        private List<string> toBeProcessedPaths;
        private List<ToProcess> toProcessesList = new List<ToProcess>();
        private List<string> toBeProcessedFiles;
        private string[] toBeProcessedNames;
        public Case() {}
        public Case(string parentPath)
        {
            ParentPath = parentPath;
            SetValues();
            FormVals = new Form();

            InitializeProcessPaths();
        }
        public string ParentPath { get; }  
        public string CaseName { get { return caseName; } }
        public string CaseNumber { get { return caseNumber; } }
        public string[] ParentDirectories {  get { return parentDirectories; } }
        public string[] ParentFiles { get { return parentFiles; } }
        public string ExcelPath {  get { return excelPath; } }
        public List<string> ToBeProcessedPaths { get { return toBeProcessedPaths; } }
        public List<ToProcess> ToProcessList { get { return toProcessesList; } set { } }
        public string[] ToBeProcessedNames { get { return toBeProcessedNames; } }
        public Form FormVals { get; set; }
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
        private List<string> getToBeProcessedPaths()
        {
            if (ParentPath == null)
            {
                return null;
            }
            else
            {
                for (int i = 0; i < ParentDirectories.Length-1; i++)
                {
                    if (ParentDirectories[i] == null) return null;

                    StringComparison comp = StringComparison.Ordinal;
                    if (ParentDirectories[i].Contains("to be processed", comp))
                    {
                        return Directory.GetFiles(ParentDirectories[i], "*.*")
                            .Where(file => !file.EndsWith("db"))
                            .ToList();
                    }
                }
                return null;
            }
        }        
        public string[] getToBeProcessedNames()
        {
            if (!string.IsNullOrEmpty(ParentPath))
            {
                return null;
            }
            else
            {
                List<string> files = new List<string>();
                List<string> names = new List<string>();
                string url;

                for (int i = 0; i < ParentDirectories.Length; i++)
                {
                    StringComparison comp = StringComparison.Ordinal;
                    if (ParentDirectories[i].Contains("to be processed", comp))
                    {
                        url = ParentDirectories[i];
                        files.AddRange(Directory.GetFiles(url));
                    }
                }
                for (int i = 0; i < files.Count; i++)
                {
                    names.Add(files[i].Substring(files[i].LastIndexOf("\\") + 1));
                }
                return names.ToArray();
            }
        }
        private void SetValues()
        {
            caseName = ParentPath.Substring(ParentPath.LastIndexOf("\\") + 1);
            caseNumber = string.IsNullOrEmpty(ParentPath) ? null :
                    ParentPath.Substring(ParentPath.IndexOf("["));
            parentDirectories = string.IsNullOrEmpty(ParentPath) ? null :
                    Directory.GetDirectories(ParentPath);
            parentFiles = string.IsNullOrEmpty(ParentPath) ? null :
                    Directory.GetFiles(ParentPath);
            excelPath = string.IsNullOrEmpty(ParentPath) ? null :
                    Array.Find(ParentFiles, path => path.EndsWith(".xlsx", StringComparison.Ordinal));
            toBeProcessedPaths = getToBeProcessedPaths();
            toBeProcessedNames = getToBeProcessedNames();
        }
    }

    //public class CaseViewModel
    //{
    //    public CaseViewModel() { }
    //    public CaseViewModel(string parentPath)
    //    {
    //        currentCase.ParentPath = parentPath;
    //    }
    //    private Case currentCase = new Case();
    //    public Case CurrentCase { get { return currentCase; } }
    //}
}
