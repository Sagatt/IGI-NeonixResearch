using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Compression;

namespace Project_IGI_Texture_Editor
{
    public class ZipFileReader
    {
        private static ZipArchive archive;
        private static string[] file_list;

        public static string[] ReadZipFile(string zipFilePath)
        {
            archive = ZipFile.OpenRead(zipFilePath);            
            file_list = new string[archive.Entries.Count];
            int index = 0;
            foreach (ZipArchiveEntry entry in archive.Entries)
            {
                file_list[index] = entry.FullName;
                index++;
            }
            return file_list;
        }
        public static Stream GetZipFileItem(string itemPath)
        {
            foreach (ZipArchiveEntry entry in archive.Entries)
            {
                if (entry.FullName.Equals(itemPath))
                {
                    return entry.Open();
                }
            }
            return null;
        }
    }
}
