using BepInEx;
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

        protected static void RegisterDataBanks(JsonDef logsettings, Texture2D image,FMODAsset Audiolog)
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
                    null,
                    unlockSound: PDAHandler.UnlockImportant,
                    Audiolog
                    
                    );
            Plugin.Log.LogInfo($"Log {logsettings.title} has registered.");

            StoryGoalHandler.RegisterCustomEvent(logsettings.id, () =>
            {

                PDAEncyclopedia.Add(logsettings.id, true);

                // Save Data Stuff || Verileri Kaydetme Şeyleri
                Plugin.Pdacache.CollectedPDAs.Add(logsettings.id);
                

            });
            
        }

        internal static IEnumerator registerdatabankswithaudio(string audio, JsonDef log)
        {
            Plugin.Log.LogInfo($"starting to convert audiofile to fmod path: {log.Audiofile}");
            FMODAsset VoiceLogFMODAsset = AudiotoFMODAsset.ConverttoFMOD(log.Audiofile,log);

            RegisterDataBanks(log , null , VoiceLogFMODAsset);

            yield return VoiceLogFMODAsset;

        }

        internal static IEnumerator registerdatabankswithaudioandimage(string audio,Texture2D image, JsonDef log)
        {
            Plugin.Log.LogInfo($"starting to convert audiofile to fmod path: {log.Audiofile}");
            FMODAsset VoiceLogFMODAsset = AudiotoFMODAsset.ConverttoFMOD(log.Audiofile, log);

            RegisterDataBanks(log, image, VoiceLogFMODAsset);

            yield return VoiceLogFMODAsset && image;

        }


        internal static IEnumerator RegisterDatabankwithimage(string imagepath, JsonDef log)
        {
            string pathofmodfolder = Path.Combine(Paths.PluginPath, "CustomPDALogMod");
            string pathoflogfolder = Path.Combine(pathofmodfolder, "logs");
            string pathofimagefolder = Path.Combine(pathoflogfolder, "Image");
            string pathofimage = Path.Combine(pathofimagefolder, imagepath);
            Plugin.Log.LogInfo($"attempting to set up pda with image {pathofimage}");

            Texture2D image = ImagetoTexture2d.convert(pathofimage);

            if (log.Audiofile == string.Empty)
            {

                RegisterDataBanks(log, image, null);
                yield return image;

            }
            else
            {

                CoroutineHost.StartCoroutine(registerdatabankswithaudioandimage(log.Audiofile, image, log));
                yield return image;
            }




                yield return image;
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

                    if (log.Imagepath == string.Empty)
                    {
                        if (log.Audiofile == string.Empty)
                        {
                            RegisterDataBanks(log, null, null);
                            LoadedJsons.Add(log);
                        }
                        else
                        {
                            CoroutineHost.StartCoroutine(registerdatabankswithaudio(log.Audiofile, log));
                            LoadedJsons.Add(log);
                        }
                    }
                    else
                    {




                        CoroutineHost.StartCoroutine(RegisterDatabankwithimage(log.Imagepath, log));


                        LoadedJsons.Add(log);
                    }

                    
                }
                catch (System.Exception exception)
                {
                }

                LogsLoaded = true;
            }
        }

    }
}
