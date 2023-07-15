using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Linq;

namespace SDL_2_Test.engine
{
    public static class Level
    {
        private static readonly string levelNumberFile = "./levelNumber.txt";
        public static void SaveLevel()
        {
            int number = GetLevelNumber();
            string fileTitle = "./levels/level" + number + ".txt";

            string json = JsonConvert.SerializeObject(Variables.Entities, Formatting.Indented,
            new JsonSerializerSettings
            {
                ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore,
                TypeNameHandling = TypeNameHandling.Auto,
            });

            if (!System.IO.Directory.Exists("levels"))
                System.IO.Directory.CreateDirectory("levels");

            File.WriteAllText(fileTitle, json);

            string assetsTitle = "./levels/assetslevel" + number + ".txt";

            json = JsonConvert.SerializeObject(Assets.AssetsPaths, Formatting.Indented,
            new JsonSerializerSettings
            {
                ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore,
                TypeNameHandling = TypeNameHandling.Auto,
            });

            File.WriteAllText(assetsTitle, json);
        }

        public static void LoadLevel(string fileName)
        {
            if (File.Exists("./levels/" + fileName))
            {
                string json = File.ReadAllText("./levels/" + fileName);

                var settings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore,
                    TypeNameHandling = TypeNameHandling.Auto,
                };

                var entities = JsonConvert.DeserializeObject<List<Entity>>(json, settings);
                Variables.Entities = entities;
            }

            if (File.Exists("./levels/assets" + fileName))
            {
                string json = File.ReadAllText("./levels/assets" + fileName);

                var settings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore,
                    TypeNameHandling = TypeNameHandling.Auto,
                };

                var assets = JsonConvert.DeserializeObject<List<string>>(json, settings);
                foreach(string asset in assets)
                {
                    Assets.AddAsset(asset);
                }
            }
        }

        public static void DeleteLevel(string fileName)
        {
            File.Delete("./levels/" + fileName);
            File.Delete("./levels/assets" + fileName);
        }

        public static int GetLevelNumber()
        {
            int number = ReadNumberFromFile();
            SaveNumberToFile(number + 1);
            return number;
        }

        private static int ReadNumberFromFile()
        {
            int number = 0;
            if (File.Exists(levelNumberFile))
            {
                string fileContent = File.ReadAllText(levelNumberFile);
                int.TryParse(fileContent, out number);
            }
            return number;
        }

        private static void SaveNumberToFile(int number)
        {
            using (StreamWriter writer = File.CreateText(levelNumberFile))
            {
                writer.Write(number.ToString());
            }
        }
    }
}
