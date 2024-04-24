using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentRegister.Models
{
    public class Form
    {
        public Form() 
        {
            Date = DateTimeOffset.Now;
        }
        public (string, string) Link { get; set; }
        public string Description { get; set; }
        public DateTimeOffset Date { get; set; }
        public string To { get; set; }
        public string From { get; set; }
        public string Type { get; set; }
        public string Privilaged { get; set; }
    }
}
