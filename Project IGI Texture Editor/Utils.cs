using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_IGI_Texture_Editor
{
    internal class Utils
    {

        public static void SaveImage(Image im, string destPath)
        {
            im.Save(destPath, System.Drawing.Imaging.ImageFormat.Png);
        }
    }
}
