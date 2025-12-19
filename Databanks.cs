using CustomPDALogMod;
using Nautilus.Handlers;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UWE;

namespace PDALogs
{
    internal class Databanks
    {
        public static bool LogsLoaded = false;

        public static readonly List<JsonDef> LoadedJsons = new List<JsonDef>();
        private static readonly HashSet<string> RegisteredLogs = new HashSet<string>();

        protected static void RegisterDataBanks(JsonDef logsettings, Texture2D image)
        {
            if (string.IsNullOrEmpty(logsettings.id))
                throw new Exception("Log id is null or empty");
            else
                PDAHandler.AddEncyclopediaEntry
                    (
                    logsettings.id,
                    logsettings.category,
                    logsettings.title,
                    logsettings.description,
                    image,
                    unlockSound: PDAHandler.UnlockImportant
                    );


            StoryGoalHandler.RegisterCustomEvent(logsettings.id, () =>
            {

                PDAEncyclopedia.Add(logsettings.id, true);

                // Save Data Stuff || Verileri Kaydetme Şeyleri
                Plugin.Pdacache.CollectedPDAs.Add(logsettings.id);
                

            });
            
        }

        
        

        internal static void LoadLogs(string directory)
        {
            foreach (string dir in Directory.GetFiles(directory, "*.json"))
            {
                try
                {
                    string json = File.ReadAllText(dir);
                    JsonDef log = JsonConvert.DeserializeObject<JsonDef>(json);

                    if (log == null)
                    {
                    }

                    if (log.Imagepath == "null")
                    {
                        RegisterDataBanks(log, null);
                    }
                    else

                        

                        RegisterDataBanks(log, null);


                    LoadedJsons.Add(log);

                    
                }
                catch (System.Exception exception)
                {
                }

                LogsLoaded = true;
            }
        }

    }
}
