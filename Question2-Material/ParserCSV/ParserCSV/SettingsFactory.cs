using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace ParserCSV
{
    public class SettingsFactory
    {
        public Settings Settings { get; set; }

        /// <summary>
        /// Initialises settings from App.config
        /// </summary>
        public SettingsFactory()
        {
            Settings = new Settings();
        }

        public void Read()
        {
            Settings.Connection = ConfigurationManager.ConnectionStrings["DB"].ConnectionString;
            //Settings.Source = ConfigurationManager.AppSettings["Src1"].ToString(); //not used to cater for: Cannot read file using hard-coded file names
        }
    }
}