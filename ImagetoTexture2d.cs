using PDALogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CustomPDALogMod
{
    internal class ImagetoTexture2d
    {
        public static Texture2D texture;
        public static bool Imageisloaded = false;

        public static Texture2D convert(string ImagePath)
        {
            if (!File.Exists(ImagePath))
                Plugin.Log.LogError($"Image {ImagePath} is not valid.");

                return null;

            byte[] fileData = File.ReadAllBytes(ImagePath);

            Texture2D texture = new Texture2D(2, 2, TextureFormat.BC5, false);

            if (!ImageConversion.LoadImage(texture, fileData))
                return null;

            return texture;
        }
    }
}
