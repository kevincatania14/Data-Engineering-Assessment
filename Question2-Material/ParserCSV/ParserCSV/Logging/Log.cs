using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParserCSV.Logging
{
    public static class Log
    {
        public static Logger Logger = LogManager.GetLogger("kevin-log");
        //public static Logger EventLog = LogManager.GetLogger("ApplicationEventLog");
    }
}