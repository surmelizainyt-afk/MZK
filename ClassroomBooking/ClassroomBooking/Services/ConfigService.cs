using System;
using System.IO;
using System.Web.Script.Serialization;
using ClassroomBooking.Configuration;

namespace ClassroomBooking.Services
{
    public class ConfigService
    {
        public AppSettings Load()
        {
            string path = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "appsettings.json");

            if (!File.Exists(path))
                return new AppSettings();

            string json = File.ReadAllText(path);

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            AppSettings settings = serializer.Deserialize<AppSettings>(json);

            return settings ?? new AppSettings();
        }
    }
}