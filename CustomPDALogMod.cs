using BepInEx;
using Nautilus.Handlers;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using System;
using System.Collections.Generic;

namespace CustomPDALogMod
{
    [BepInPlugin("TDCDev.CustomPDALogMod", "Custom PDA Log Mod", "1.0.1")]
    public class CustomPDALogMod : BaseUnityPlugin
    {
        private string LogsFolder;

        private void Awake()
        {
            LogsFolder = Path.Combine(Path.GetDirectoryName(Info.Location), "logs");

            if (!Directory.Exists(LogsFolder))
                Directory.CreateDirectory(LogsFolder);

            LoadLogs();
        }

        private void LoadLogs()
        {
            var allLogs = new Dictionary<string, List<PDALogData>>();

            foreach (string file in Directory.GetFiles(LogsFolder, "*.json"))
            {
                try
                {
                    string json = File.ReadAllText(file);
                    PDALogData data = JsonConvert.DeserializeObject<PDALogData>(json);

                    if (data == null) continue;

                    string cat = string.IsNullOrEmpty(data.category) ? "Custom Logs" : data.category;
                    string sub = string.IsNullOrEmpty(data.subcategory) ? "General" : data.subcategory;
                    string key = cat + "|" + sub;

                    if (!allLogs.ContainsKey(key))
                        allLogs[key] = new List<PDALogData>();

                    allLogs[key].Add(data);

                    RegisterLog(data);
                    SpawnLog(data);
                }
                catch (Exception e)
                {
                    Logger.LogError($"Log yüklenemedi: {file} | {e}");
                }
            }

            Logger.LogInfo($"Toplam {allLogs.Count} kategori yüklendi.");
        }

        private void RegisterLog(PDALogData data)
        {
            if (data == null || string.IsNullOrEmpty(data.id))
                return;

            string category = string.IsNullOrEmpty(data.category) ? "Custom Logs" : data.category;
            string subCategory = string.IsNullOrEmpty(data.subcategory) ? "General" : data.subcategory;

            // Placeholder icon ve sprite
            Texture2D dummyIcon = Texture2D.whiteTexture;
            Sprite dummySprite = Sprite.Create(dummyIcon, new Rect(0, 0, 1, 1), Vector2.zero);
            FMODAsset dummyAudio = null;

            PDAHandler.AddEncyclopediaEntry(
                data.id,
                category,
                subCategory,
                data.title ?? data.id,
                dummyIcon,
                dummySprite,
                dummyAudio,
                dummyAudio
            );

            Logger.LogInfo($"Log kaydedildi: {data.id} ({category} | {subCategory})");
        }

        private void SpawnLog(PDALogData data)
        {
            if (data == null || string.IsNullOrEmpty(data.id))
                return;

            TechType techType;
            if (!Enum.TryParse(data.techType, out techType))
                techType = TechType.PDA;

            // Prefab fallback
            GameObject prefab = Resources.Load<GameObject>("Submarine/Prefabs/PDA");

            if (prefab == null)
            {
                Logger.LogError($"Prefab bulunamadı: {techType}");
                return;
            }

            GameObject clone = GameObject.Instantiate(prefab);
            clone.name = data.id;

            Vector3 pos = data.position != null
                ? new Vector3(data.position.x, data.position.y, data.position.z)
                : Vector3.zero;

            Vector3 rot = data.rotation != null
                ? new Vector3(data.rotation.x, data.rotation.y, data.rotation.z)
                : Vector3.zero;

            clone.transform.position = pos;
            clone.transform.eulerAngles = rot;

            Logger.LogInfo($"Log spawn edildi: {data.id}");
        }
    }

    public class PDALogData
    {
        public string id;
        public string techType;       // PDA, PDA_geo vb.
        public string title;
        public string description;
        public string category;       // opsiyonel
        public string subcategory;    // opsiyonel
        public PositionData position; // opsiyonel
        public PositionData rotation; // opsiyonel
    }

    public class PositionData
    {
        public float x;
        public float y;
        public float z;
    }
}
