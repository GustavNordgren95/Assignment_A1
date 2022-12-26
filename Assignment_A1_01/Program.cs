using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.Json;

using Assignment_A1_01.Models;
using Assignment_A1_01.Services;

namespace Assignment_A1_01
{
    class Program
    {
        static async Task Main(string[] args)
        {
            double latitude = 59.85859649062523;
            double longitude = 17.63892985364318;

            Forecast forecast = await new OpenWeatherService().GetForecastAsync(latitude, longitude);

            //Your Code to present each forecast item in a grouped list
            Console.WriteLine($"Weather forecast for {forecast.City}");
            Console.WriteLine();

            var forecastDayGrp = forecast.Items.GroupBy(d => d.DateTime.Date, d => d);

            foreach (var forecastDay in forecastDayGrp)
            {
                Console.WriteLine();
                Console.WriteLine(forecastDay.Key);
                Console.WriteLine();
                foreach (var item in forecastDay)
                {
                    Console.WriteLine($" {item.DateTime} - Väder: {item.Description}, Temperatur: {item.Temperature}°C, Vind: {item.WindSpeed} m/s");
                    Console.WriteLine();
                }
            }
        }
    }
}