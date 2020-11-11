using BuffetDAL.Repos.ADO;
using Serilog;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;

namespace BuffetReportsService
{
    public class ReportsGenerator
    {
        private string _connectionString;
        public ReportsGenerator(string connectionString)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File("consoleapp.txt")
                .CreateLogger();
            this._connectionString = connectionString;
        }

        public void MostFavourite()
        {
            try
            {
                Log.Information("MostFavourite() method execution started.");
                using (ADOUnitOfWork uow = new ADOUnitOfWork(this._connectionString))
                {
                    DataTable reportResults = uow.SpecificReports.GetMostFavouriteFoods();
                    using (var sw = new StreamWriter(new FileStream(@$"C:\{DateTime.Now.Date.ToString("d")}-mostfavourite.csv", FileMode.Create, FileAccess.Write)))
                    {
                        string name = reportResults.Columns[0].ColumnName;
                        string rate = reportResults.Columns[1].ColumnName;

                        sw.WriteLine($"{name};{rate}");

                        for (int i = 0; i < reportResults.Rows.Count; i++)
                        {
                            sw.WriteLine($"{reportResults.Rows[i][name]};{reportResults.Rows[i][rate]}");
                        }
                    }
                }
            }
            catch
            {
                Log.Warning("Something bad happened in attempt to execute query to database in MostFavourite().");
            }
        }

        public void MostPopular()
        {
            try
            {
                Log.Information("MostPopular() method execution started.");
                using (ADOUnitOfWork uow = new ADOUnitOfWork(this._connectionString))
                {
                    DataTable reportResults = uow.SpecificReports.GetMostPopularFoods();
                    using (var sw = new StreamWriter(new FileStream(@$"C:\{DateTime.Now.Date.ToString("d")}-mostpopular.csv", FileMode.Create, FileAccess.Write)))
                    {
                        string name = reportResults.Columns[0].ColumnName;
                        string orders = reportResults.Columns[1].ColumnName;

                        sw.WriteLine($"{name};{orders}");

                        for (int i = 0; i < reportResults.Rows.Count; i++)
                        {
                            sw.WriteLine($"{reportResults.Rows[i][name]};{reportResults.Rows[i][orders]}");
                        }
                    }
                }
            }
            catch
            {
                Log.Warning("Something bad happened in attempt to execute query to database in MostPopular().");
            }
        }

        public void LackOfFoods()
        {
            try
            {
                Log.Information("LackOfFoods() method execution started.");
                using (ADOUnitOfWork uow = new ADOUnitOfWork(this._connectionString))
                {
                    DataTable reportResults = uow.SpecificReports.GetLackOfFoods();
                    using (var sw = new StreamWriter(new FileStream(@$"C:\{DateTime.Now.Date.ToString("d")}-lackoffoods.csv", FileMode.Create, FileAccess.Write)))
                    {
                        string name = reportResults.Columns[0].ColumnName;
                        string lack = reportResults.Columns[1].ColumnName;

                        sw.WriteLine($"{name};{lack}");

                        for (int i = 0; i < reportResults.Rows.Count; i++)
                        {
                            sw.WriteLine($"{reportResults.Rows[i][name]};{reportResults.Rows[i][lack]}");
                        }
                    }
                }
            }
            catch
            {
                Log.Warning("Something bad happened in attempt to execute query to database in LackOfFoods().");
            }
        }

        public void DailyReservesRating()
        {
            try
            {
                Log.Information("LackOfFoods() method execution started.");
                using (ADOUnitOfWork uow = new ADOUnitOfWork(this._connectionString))
                {
                    DataTable reportResults = uow.SpecificReports.GetDailyReservesRating();
                    using (var sw = new StreamWriter(new FileStream(@$"C:\{DateTime.Now.Date.ToString("d")}-dailyreserves.csv", FileMode.Create, FileAccess.Write)))
                    {
                        string dayOfWeek = "Day of week";
                        string date = reportResults.Columns[0].ColumnName;
                        string reserves = reportResults.Columns[1].ColumnName;

                        sw.WriteLine($"{date};{dayOfWeek};{reserves}");

                        for (int i = 0; i < reportResults.Rows.Count; i++)
                        {
                            DateTime dateTime = (DateTime)reportResults.Rows[i][date];
                            int reservesCount = (int)reportResults.Rows[i][reserves];
                            sw.WriteLine($"{dateTime.Date};{dateTime.DayOfWeek};{reservesCount}");
                        }
                    }
                }
            }
            catch
            {
                Log.Warning("Something bad happened in attempt to execute query to database in LackOfFoods().");
            }
        }
    }
}
