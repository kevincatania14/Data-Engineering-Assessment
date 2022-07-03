using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ParserCSV;

namespace ParserCSV
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> csvFilesDirectory = Directory.GetFiles(@"C:\DataEngineeringAssessment\Question2-Material\TestFiles", "*.csv").ToList();
            Factory f = new Factory(csvFilesDirectory);
            f.Execute();
            Console.WriteLine("Processing complete. Refer to full log.");
            Console.ReadKey();
        }
    }
}