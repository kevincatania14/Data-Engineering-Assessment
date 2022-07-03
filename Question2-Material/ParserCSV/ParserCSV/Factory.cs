using ParserCSV.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.IO;

namespace ParserCSV
{
    public class Factory
    {
        public Settings Settings { get; set; }

        private List<string> files { get; set; } //catering for: Cannot read file using hard-coded file names

        private string insertSQL { get; set; }

        /// <summary>
        /// Constructor catering for: Cannot read file using hard-coded file names
        /// </summary>
        public Factory(List<string> files)
        {
            this.files = files;

            //setting default insert sql
            this.insertSQL = "insert into sale_record(Id,Region,Country,ItemType,SalesChannel," +
            "OrderPriority,OrderDate,OrderID,ShipDate,UnitsSold," +
            "UnitPrice,UnitCost,TotalRevenue,TotalCost,TotalProfit)" +
            "values(@Id,@Region,@Country,@ItemType,@SalesChannel," +
            "@OrderPriority,@OrderDate,@OrderID,@ShipDate,@UnitsSold," +
            "@UnitPrice,@UnitCost,@TotalRevenue,@TotalCost,@TotalProfit)";
        }

        /// <summary>
        /// Initialises SettingsFactory which sets up up DB connection from App.config
        /// </summary>
        public void Execute()
        {
            //checking if directory is empty
            if (this.files.Count > 0)
            {
                SettingsFactory sf = new SettingsFactory();
                sf.Read();
                Settings = sf.Settings;

                //ReadFile(Settings.Source); //not used to cater for: Cannot read file using hard-coded file names

                //Running ReadFile for each file passed as parameter to Factory constructor
                foreach (var file in files)
                {
                    ReadFile(file);
                }
            }
            else
            {
                Console.WriteLine("No CSV files found in directory");
                Logging.Log.Logger.Warn("No CSV files found in directory");
            }
        }

        /// <summary>
        /// Processes individual file and writes data to DB
        /// </summary>
        /// <param name="directoryToFile"></param>
        /// <returns>Exception and warning messages</returns>
        private bool ReadFile(string directoryToFile)
        {
            Logging.Log.Logger.Info("Reading file: {0} ", directoryToFile);

            int lineCounter = 0; //reseting to cater for: Extra PROCESSED/ERROR folders

            try
            {
                using (StreamReader sr = new StreamReader(directoryToFile))
                {
                    string currentLine = string.Empty;
                    SaleRecord record = new SaleRecord();
                    SqlCommand command = new SqlCommand();

                    try
                    {
                        using (SqlConnection connection = new SqlConnection(Settings.Connection))
                        {
                            while (!sr.EndOfStream)
                            {
                                if (connection.State != System.Data.ConnectionState.Open) connection.Open();

                                currentLine = sr.ReadLine();

                                if (lineCounter != 0) //catering for: First row of each file is the header
                                {
                                    try
                                    {
                                        record = SplitLine(currentLine);
                                        command = SQLCommandBuilder(connection, record);
                                        command.ExecuteNonQuery(); //adding processed record to database
                                    }
                                    catch (Exception e)
                                    {
                                        Console.WriteLine("Inner exception on Line: {0}; Error: {1}", lineCounter, e.Message);
                                        Logging.Log.Logger.Error("Inner exception on Line: {0}; Error: {1}", lineCounter, e.Message);
                                    }
                                }

                                lineCounter++;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Outer exception on Line: {0}; Error: {1}", lineCounter, e.Message);
                        Logging.Log.Logger.Error("Outer exception on Line: {0}; Error: {1}", lineCounter, e.Message);
                    }
                    finally
                    {
                        command.Dispose(); //finished reading file
                        //Console.WriteLine("Count: {0}", lineCounter);
                        Logging.Log.Logger.Info("Finished reading file");
                    }

                    /* Extra PROCESSED/ERROR folders */
                    string currentDir = ConfigurationManager.AppSettings["CurrentDir"].ToString() + Path.GetFileName(directoryToFile);
                    string processedDir = ConfigurationManager.AppSettings["ProcessedDir"].ToString() + Path.GetFileName(directoryToFile);
                    string errorDir = ConfigurationManager.AppSettings["ErrorDir"].ToString() + Path.GetFileName(directoryToFile);

                    if (lineCounter > 1)
                    {
                        try
                        {
                            MoveFile(currentDir, processedDir);
                            Logging.Log.Logger.Info("Total Lines: {0}; Moved file to PROCESSED folder", lineCounter);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                            Logging.Log.Logger.Error("Error when moving file {0} to PROCESSED folder: {1}", currentDir, e.Message);
                        }
                        //File.Move(currentDir, processedDir);
                        //File.Copy(currentDir, processedDir, true);
                    }
                    else
                    {                        
                        try
                        {
                            MoveFile(currentDir, errorDir);
                            Logging.Log.Logger.Info("Total Lines: {0}; Moved file to ERROR folder", lineCounter);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                            Logging.Log.Logger.Error("Error when moving file {0} to ERROR folder: {1}", currentDir, e.Message);
                        }
                        //File.Move(currentDir, processedDir);
                        //File.Copy(currentDir, errorDir, true);
                    }

                    return true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("General exception on Line: {0}; Error: {1}", lineCounter, e.Message);
                Logging.Log.Logger.Error("General exception on Line: {0}; Error: {1}", lineCounter, e.Message);
                return false;
            }
        }

        /// <summary>
        /// Splits comma delimited data from CSV
        /// </summary>
        /// <param name="line"></param>
        /// <returns>SaleRecord object ready to be sent to DB </returns>
        private SaleRecord SplitLine(string line)
        {
            SaleRecord sr = new SaleRecord();
            string[] columns = line.Split(','); //catering for: CSV Delimiter is comma (,)

            sr.Id = Guid.NewGuid();
            sr.Region = columns[0];
            sr.Country = columns[1];
            sr.ItemType = columns[2];
            sr.SalesChannel = columns[3];
            sr.OrderPriority = columns[4];
            sr.OrderDate = columns[5];
            sr.OrderID = columns[6];
            sr.ShipDate = Convert.ToDateTime(columns[7], CultureInfo.GetCultureInfo("en-Us").DateTimeFormat);
            sr.UnitsSold = Convert.ToInt32(columns[8]);
            sr.UnitPrice = Convert.ToDouble(columns[9]);
            sr.UnitCost = Convert.ToDouble(columns[10]);
            sr.TotalRevenue = Convert.ToDouble(columns[11]);
            sr.TotalCost = Convert.ToDouble(columns[12]);
            sr.TotalProfit = Convert.ToDouble(columns[13]);

            return sr;
        }

        /// <summary>
        /// Builds SqlCommand from SaleRecord object
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="sr"></param>
        /// <returns>SqlCommand to be sent to DB</returns>
        private SqlCommand SQLCommandBuilder(SqlConnection connection, SaleRecord sr)
        {
            SqlCommand command = connection.CreateCommand();
            command.CommandText = insertSQL;

            command.Parameters.AddWithValue("@Id", sr.Id);
            command.Parameters.AddWithValue("@Region", sr.Region);
            command.Parameters.AddWithValue("@Country", sr.Country);
            command.Parameters.AddWithValue("@ItemType", sr.ItemType);
            command.Parameters.AddWithValue("@SalesChannel", sr.SalesChannel);
            command.Parameters.AddWithValue("@OrderPriority", sr.OrderPriority);
            command.Parameters.AddWithValue("@OrderDate", sr.OrderDate);
            command.Parameters.AddWithValue("@OrderID", sr.OrderID);
            command.Parameters.AddWithValue("@ShipDate", sr.ShipDate);
            command.Parameters.AddWithValue("@UnitsSold", sr.UnitsSold);
            command.Parameters.AddWithValue("@UnitPrice", sr.UnitPrice);
            command.Parameters.AddWithValue("@UnitCost", sr.UnitCost);
            command.Parameters.AddWithValue("@TotalRevenue", sr.TotalRevenue);
            command.Parameters.AddWithValue("@TotalCost", sr.TotalCost);
            command.Parameters.AddWithValue("@TotalProfit", sr.TotalProfit);

            return command;
        }

        /// <summary>
        /// Helper method for moving files
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        private void MoveFile(string source, string target)
        {
            string tool = "move.bat";
            string arguments = source + " " + target; //arguments seperated by space

            try
            {
                Process.Start(ConfigurationManager.AppSettings["SupportingTools"].ToString() + tool, arguments);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}