using HarbourControl.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace HarbourControl.Services
{
    public class WindSpeedLookups
    {
        public decimal GetWeatherInfo()
        {
          const string API_KEY = "a47d8c728e4d177b2968fff2c8f7067d";
          const string city = "Durban";
            string url = string.Format("http://api.openweathermap.org/data/2.5/weather?q={0}&units=metric&cnt=1&APPID={1}", city, API_KEY);
  
            decimal windSpeed = 0;

            using (WebClient client = new WebClient())
            {
                string json = client.DownloadString(url);

                WeatherInfo weatherInfo = (new JavaScriptSerializer()).Deserialize<WeatherInfo>(json);
                windSpeed = weatherInfo.Wind.Speed;

            }
           
            return windSpeed;
 
        }
    }
}
