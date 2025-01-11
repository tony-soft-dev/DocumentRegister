using System;

namespace DocumentRegister.Models
{
    public class Form
    {
        public Form() 
        {
            Saved = false;
            Description = "";
            Date = DateTimeOffset.Now;
            To = "";
            From = "";
            DocType = "";
            Privileged = false;
        }
        public bool Saved { get; set; }
        public string Description { get; set; }
        public DateTimeOffset Date { get; set; }
        public string To { get; set; }
        public string From { get; set; }
        public string DocType { get; set; }
        public bool Privileged { get; set; }
    }
}
