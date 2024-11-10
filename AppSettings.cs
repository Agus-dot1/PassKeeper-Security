using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Wpf.Ui.Appearance;

namespace PassKeeper
{
    public class AppSettings
    {
        private static readonly string SettigsFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.json");

        public ApplicationTheme Theme { get; set; } = ApplicationTheme.Unknown;

        public void Save()
        {
            string json = JsonSerializer.Serialize(this);
            File.WriteAllText(SettigsFilePath, json);
        }

        public static AppSettings Load()
        {
            if (File.Exists(SettigsFilePath))
            {
                string json = File.ReadAllText(SettigsFilePath);
                return JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings() ;
            }
            
            return new AppSettings();
            
        }


    }
}
