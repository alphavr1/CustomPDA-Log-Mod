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
        private List<JsonDef> Json = Databanks.LoadedJsons;
        private static string Imagepath; 
        public static Texture2D texture;

        public static void convert(string ImagePath)
        {
            

            ImagePath = Imagepath;

            
            
                if (File.Exists(Imagepath))
                {
                    byte[] filedata = File.ReadAllBytes(Imagepath);

                    texture = new Texture2D(1, 1);

                    if (ImageConversion.LoadImage(texture,filedata))
                    {
                        
                    }
                
               

                }
        }
    }
}
