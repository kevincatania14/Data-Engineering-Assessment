using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParserCSV
{
    public class Settings
    {
        public string Connection { get; set; }
        //public string Source { get; set; } //not used to cater for: Cannot read file using hard-coded file names
    }
}