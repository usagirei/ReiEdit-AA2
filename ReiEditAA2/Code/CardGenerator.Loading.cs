// --------------------------------------------------
// ReiEditAA2 - CardGenerator.Loading.cs
// --------------------------------------------------

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using ReiFX;

namespace ReiEditAA2.Code
{
    internal static partial class CardGenerator
    {
        internal static IEnumerable<Layer> LoadAndMirror(string dir, byte id, bool mirror)
        {
            var loaded = LoadLayers(dir, id);
            if (mirror)
                loaded.ForEach(MirrorEffect.ApplyEffect);
            return loaded;
        }

        internal static Layer LoadLayer(string file, byte zIndex = 0)
        {
            string dir = Path.GetDirectoryName(file);
            string nam = Path.GetFileNameWithoutExtension(file);

            string file2 = Directory.EnumerateFiles(dir, nam + "*.png")
                .FirstOrDefault();
            if (!File.Exists(file2))
                return null;

            string flags = file2.Remove(file.Length);
            bool xFlag = flags.IndexOf('X') != -1;
            bool yFlag = flags.IndexOf('Y') != -1;
            bool zFlag = flags.IndexOf('Z') != -1;

            Stream s = File.OpenRead(file2);
            Bitmap bmp = (Bitmap) Image.FromStream(s);
            s.Close();

            Layer l = (Layer) bmp;
            l.ZIndex = zIndex;
            l.XFlag = xFlag;
            l.YFlag = yFlag;
            l.ZFlag = zFlag;

            return l;
        }

        internal static List<Layer> LoadLayers(string dir, int id)
        {
            var layers = new List<Layer>();

            if (!Directory.Exists(dir))
                return layers;

            string fileFormat = String.Format("{0:D3}_*.png", id);

            var files = Directory.EnumerateFiles(dir, fileFormat);

            foreach (string file in files)
            {
                string fileName = Path.GetFileNameWithoutExtension(file)
                    .ToUpper();
                byte zIndex = Convert.ToByte(fileName.Substring(4, 1), 16);
                //bool xFlag = fileName.Length > 5 && fileName[5] == 'X';
                bool hasFlags = fileName.Length >= 5;
                string flags = hasFlags
                    ? fileName.Substring(4)
                    : "";
                bool xFlag = flags.IndexOf('X') != -1;
                bool yFlag = flags.IndexOf('Y') != -1;
                bool zFlag = flags.IndexOf('Z') != -1;

                Stream s = File.OpenRead(file);
                Bitmap bmp = (Bitmap) Image.FromStream(s);
                s.Close();

                Layer l = (Layer) bmp;
                l.ZIndex = zIndex;
                l.XFlag = xFlag;
                l.YFlag = yFlag;
                l.ZFlag = zFlag;

                layers.Add(l);
            }

            return layers;
        }

        internal static List<Layer> LoadLayers(string dir, string name)
        {
            var layers = new List<Layer>();

            if (!Directory.Exists(dir))
                return layers;

            string fileFormat = String.Format("{0}_*.png", name);

            var files = Directory.EnumerateFiles(dir, fileFormat);

            foreach (string file in files)
            {
                string fileName = Path.GetFileNameWithoutExtension(file)
                    .ToUpper();
                byte zIndex = Convert.ToByte(fileName.Substring(name.Length + 1, 1), 16);
                //bool xFlag = fileName.Length > 5 && fileName[5] == 'X';
                bool hasFlags = fileName.Length >= 5;
                string flags = hasFlags
                    ? fileName.Substring(4)
                    : "";
                bool xFlag = flags.IndexOf('X') != -1;
                bool yFlag = flags.IndexOf('Y') != -1;
                bool zFlag = flags.IndexOf('Z') != -1;

                Stream s = File.OpenRead(file);
                Bitmap bmp = (Bitmap) Image.FromStream(s);
                s.Close();

                Layer l = (Layer) bmp;
                l.ZIndex = zIndex;
                l.XFlag = xFlag;
                l.YFlag = yFlag;
                l.ZFlag = zFlag;

                layers.Add(l);
            }

            return layers;
        }
    }
}