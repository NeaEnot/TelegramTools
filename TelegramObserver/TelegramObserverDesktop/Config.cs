using Newtonsoft.Json;
using System.IO;

namespace TelegramObserverDesktop
{
    public class Config
    {
        private const string path = "config.json";

        public int ApiId { get; set; }
        public string ApiHash { get; set; }
        public string PhoneNumber { get; set; }
        public string PathToMusic { get; set; }

        public void Save()
        {
            using (StreamWriter writer = new StreamWriter(path))
            {
                string json = JsonConvert.SerializeObject(this);
                writer.WriteAsync(json);
            }
        }

        public static Config Load()
        {
            if (!File.Exists(path))
            {
                throw new System.Exception("Config file in not found!");
            }

            using (StreamReader reader = new StreamReader(path))
            {
                string json = reader.ReadToEnd();
                Config restored = JsonConvert.DeserializeObject<Config>(json);

                return restored;
            }
        }
    }
}
