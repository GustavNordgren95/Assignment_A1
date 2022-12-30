using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Net.Http;
using System.Net.Http.Json; 
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.Json;

using Assignment_A1_02.Models;
using Assignment_A1_02.Services;

namespace Assignment_A1_02
{
    class Program
    {
        static async Task Main(string[] args)
        {
            OpenWeatherService service = new OpenWeatherService();

            //Register the event
            //Your Code

            service.WeatherForecastAvailable += ReportWeatherForecastAvailable; //Subscribed to the broadcaster

            Task<Forecast>[] tasks = { null, null };
            Exception exception = null;
            try
            {
                double latitude = 59.85859649062523;
                double longitude = 17.63892985364318;

                //Create the two tasks and wait for completion
                tasks[0] = service.GetForecastAsync(latitude, longitude); //Weather data is stored in task.Result.Items
                tasks[1] = service.GetForecastAsync("Miami");

                Task.WaitAll(tasks[0], tasks[1]);
            }
            catch (Exception ex)
            {
                exception = ex;
                //How to handle an exception
                //Your Code

                Console.WriteLine("-----------");
                Console.WriteLine("City weather service error");
                Console.WriteLine(ex.Message);

            }

            foreach (var task in tasks)
            {
                //How to deal with successful and fault tasks
                //Your Code

                var forecastDayGrp = task.Result.Items.GroupBy(d => d.DateTime.Date, d => d);
                Console.WriteLine();
                Console.WriteLine($"Weather forecast for: {task.Result.City}");
                Console.WriteLine();
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

                if (task == null) throw new Exception();
            }
        }

        //Event handler declaration
        //Your Code

        public static void ReportWeatherForecastAvailable(object sender, string message) //Subscriber taking in the message
        {
            Console.WriteLine(message);
        }
        
    }
}