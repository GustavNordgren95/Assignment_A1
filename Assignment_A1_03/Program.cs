using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Net.Http;
using System.Net.Http.Json; 
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.Json;

using Assignment_A1_03.Models;
using Assignment_A1_03.Services;

namespace Assignment_A1_03
{
    class Program
    {
        static void Main(string[] args)
        {
            OpenWeatherService service = new OpenWeatherService();

            //Register the event
            //Your Code

            service.WeatherForecastAvailable += ReportWeatherForecastAvailable; //Subscribed to the broadcaster


            Task<Forecast>[] tasks = { null, null, null, null };
            Exception exception = null;
            try
            {
                double latitude = 59.5086798659495;
                double longitude = 18.2654625932976;

                //Create the two tasks and wait for completion
                tasks[0] = service.GetForecastAsync(latitude, longitude);
                tasks[1] = service.GetForecastAsync("Miami");

                Task.WaitAll(tasks[0], tasks[1]);

                tasks[2] = service.GetForecastAsync(latitude, longitude);
                tasks[3] = service.GetForecastAsync("Miami");

                //Wait and confirm we get an event showing cached data avaialable
                Task.WaitAll(tasks[2], tasks[3]);
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
