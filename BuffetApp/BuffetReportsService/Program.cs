using Microsoft.Extensions.Configuration;
using Serilog;
using System.Configuration;

namespace BuffetReportsService
{
    public class Program
    {
        static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();

            string connection = ConfigurationManager.ConnectionStrings["Default"].ConnectionString;
            ReportsGenerator reportsGenerator = new ReportsGenerator(connection);
            reportsGenerator.MostFavourite();
            reportsGenerator.DailyReservesRating();
            reportsGenerator.MostPopular();
            reportsGenerator.LackOfFoods();
        }
    }
}
