using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json; 
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Text.Json;

using Assignment_A1_03.Models;

namespace Assignment_A1_03.Services
{
    public class OpenWeatherService
    {
        HttpClient httpClient = new HttpClient();

        //Cache declaration
        ConcurrentDictionary<(double, double, string), Forecast> cachedGeoForecasts = new ConcurrentDictionary<(double, double, string), Forecast>();
        ConcurrentDictionary<(string, string), Forecast> cachedCityForecasts = new ConcurrentDictionary<(string, string), Forecast>();

        string CacheKeyTimeStamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm");

        // Your API Key
        readonly string apiKey = "459a4733f190f0bda950abaa2f2d5b25";

        //Event declaration
        public event EventHandler<string> WeatherForecastAvailable;
        protected virtual void OnWeatherForecastAvailable(string message)
        {
            WeatherForecastAvailable?.Invoke(this, message);
        }

        public async Task<Forecast> GetForecastAsync(string City)
        {
            //part of cache code here to check if forecast is in Cache
            //generate an event that shows forecast was from cache
            //Your code

            Forecast fResponse = null;

            (string, string) cacheKey = (City, CacheKeyTimeStamp);

            if (cachedCityForecasts.ContainsKey(cacheKey))
            {
                OnWeatherForecastAvailable($"Cached weather forecast for {City} available");
                return cachedCityForecasts[cacheKey];
            }

            //https://openweathermap.org/current
            var language = System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
            var uri = $"https://api.openweathermap.org/data/2.5/forecast?q={City}&units=metric&lang={language}&appid={apiKey}";

            Forecast forecast = await ReadWebApiAsync(uri);

            //part of event and cache code here
            //generate an event with different message if cached data
            //Your code

            if (cachedCityForecasts != null)
            {
                WeatherForecastAvailable?.Invoke(cachedCityForecasts, $"Weather forecast for {City} available");
            }
            
            return forecast;

        }
        public async Task<Forecast> GetForecastAsync(double latitude, double longitude)
        {
            //part of cache code here to check if forecast in Cache
            //generate an event that shows forecast was from cache
            //Your code

            string CacheKeyTimeStamp = DateTime.Now.ToString("g");

            (double, double, string) cacheKey = (latitude, longitude, CacheKeyTimeStamp);

            if (cachedGeoForecasts.ContainsKey(cacheKey))
            {
                OnWeatherForecastAvailable($"Cached weather forecast for {latitude}, {longitude} available");
                return cachedGeoForecasts[cacheKey];
            }

            //https://openweathermap.org/current
            var language = System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
            var uri = $"https://api.openweathermap.org/data/2.5/forecast?lat={latitude}&lon={longitude}&units=metric&lang={language}&appid={apiKey}";

            Forecast forecast = await ReadWebApiAsync(uri);

            //part of event and cache code here
            //generate an event with different message if cached data
            //Your code


            return forecast;
        }
        private async Task<Forecast> ReadWebApiAsync(string uri)
        {
            //Read the response from the WebApi
            HttpResponseMessage response = await httpClient.GetAsync(uri);
            response.EnsureSuccessStatusCode();
            WeatherApiData wd = await response.Content.ReadFromJsonAsync<WeatherApiData>();


            //Convert WeatherApiData to Forecast using Linq.
            //Your code
            return new Forecast
            {
                City = wd.city.name,
                Items = wd.list.Select(x => new ForecastItem
                {
                    DateTime = UnixTimeStampToDateTime(x.dt),
                    Temperature = x.main.temp,
                    WindSpeed = x.wind.speed,
                    Description = x.weather.FirstOrDefault().description,
                    Icon = x.weather.FirstOrDefault().icon
                }).ToList(),

            };
        }
        private DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dateTime;
        }
    }
}

/*
Modify OpenWeatherService so a Forecast downloaded from the web-api is cached and
returned for an identical request made within 1 minute. The message from the fired event
should differ in case of cached or downloaded forecast.

Hint:
This can be done elegantly using the thread safe version of Dictionary<>. Why thread safe?
Tuples are excellent for Keys and to get a string of current date/time without seconds can be
done by DateTime.Now.ToString("yyyy-MM-dd HH:mm") .
Modify the caller to make multiple requests to show that cached data is received.
*/
