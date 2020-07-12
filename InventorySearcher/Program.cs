using SavegameToolkit;
using SavegameToolkit.Arrays;
using SavegameToolkit.Structs;
using SavegameToolkit.Types;
using SavegameToolkitAdditions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using Newtonsoft.Json;

namespace InventorySearcher
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting conversion from Ark binary to json.  First argument is Ark save file path; second is json file path.");
            string readFromFilename = args[0];
            string writeJsonFilename = args[1];
            (GameObjectContainer gameObjectContainer, float gameTime) = ImportSavegame.ReadSavegameFile(readFromFilename, writeJsonFilename);
            Console.WriteLine("Finished reading save file and writing json. Got " + gameObjectContainer.LongCount() + " objects total.");
        }
    }

    public class ImportSavegame
    {
        private readonly float gameTime;

        private ImportSavegame(float gameTime)
        {
            this.gameTime = gameTime;
        }

        public static (GameObjectContainer, float) ReadSavegameFile(string fileName, string jsonFileName)
        {
            if (new FileInfo(fileName).Length > int.MaxValue)
            {
                throw new Exception("Input file is too large.");
            }

            Stream stream = new MemoryStream(File.ReadAllBytes(fileName));

            ArkSavegame arkSavegame = new ArkSavegame();

            // We're actually converting to json so we don't want to eliminate everything besides dinos.
            //bool PredicateCreatures(GameObject o) => !o.IsItem && (o.Parent != null || o.Components.Any());
            //bool PredicateCreaturesAndCryopods(GameObject o) => (!o.IsItem && (o.Parent != null || o.Components.Any())) || o.ClassString.Contains("Cryopod") || o.ClassString.Contains("SoulTrap_");

            using (ArkArchive archive = new ArkArchive(stream))
            {
                arkSavegame.ReadBinary(archive, ReadingOptions.Create()
                        .WithDataFiles(false)
                        .WithEmbeddedData(false)
                        .WithDataFilesObjectMap(false)
                        //.WithObjectFilter(new Predicate<GameObject>(PredicateCreaturesAndCryopods))
                        .WithBuildComponentTree(true));
            }
            
            // Jenny's addition - immediately write the json format for any binary file we read in.
            using (StreamWriter file = File.CreateText(jsonFileName))
            using (JsonTextWriter writer = new JsonTextWriter(file))
            {
                writer.Formatting = Formatting.Indented;
                arkSavegame.WriteJson(writer, WritingOptions.Create());
            }
            // End of Jenny's addition.

            if (!arkSavegame.HibernationEntries.Any())
            {
                return (arkSavegame, arkSavegame.GameTime);
            }

            List<GameObject> combinedObjects = arkSavegame.Objects;

            foreach (HibernationEntry entry in arkSavegame.HibernationEntries)
            {
                ObjectCollector collector = new ObjectCollector(entry, 1);
                combinedObjects.AddRange(collector.Remap(combinedObjects.Count));
            }

            return (new GameObjectContainer(combinedObjects), arkSavegame.GameTime);
        }

    }
}
