using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using static Project_IGI_Texture_Editor.RES;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;
using System.Collections;

namespace Project_IGI_Texture_Editor
{
    internal class RES
    {

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ILFF_HEADER
        {
            public char[] signature; // "ILFF"
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 4)]
            public uint datasize;
            public uint alignement;
            public uint chunksize;

            public ILFF_HEADER(bool initialize = true)
            {
                signature = new char[4];
                datasize = 0;
                alignement = 0;
                chunksize = 0;

                if (initialize)
                {
                    signature = new char[] { 'I', 'L', 'F', 'F' };
                }
            }

            public ILFF_HEADER(BinaryReader reader)
            {
                signature = reader.ReadChars(4);
                datasize = reader.ReadUInt32();
                alignement = reader.ReadUInt32();
                chunksize = reader.ReadUInt32();
                Console.WriteLine("ILFF_HEADER signature: " + new string(signature));
                Console.WriteLine("ILFF_HEADER datasize: " + datasize);
                Console.WriteLine("ILFF_HEADER alignement: " + alignement);
                Console.WriteLine("ILFF_HEADER chunksize: " + chunksize);
            }
        }


        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct BLOB
        {
            public ILFF_HEADER header;
            public byte[] data;
            public BLOB(BinaryReader reader)
            {
                header = new ILFF_HEADER(reader);
                data = reader.ReadBytes((int)header.datasize);
                // Calculate the amount of padding required
                int paddingSize = (int)(header.chunksize - 16 - header.datasize);

                // Add padding bytes to the end of the data array
                if (paddingSize > 0)
                {
                    Array.Resize(ref data, data.Length + paddingSize);
                    reader.ReadBytes(paddingSize).CopyTo(data, data.Length - paddingSize);
                }
            }
        }


        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ILFF
        {
            public ILFF_HEADER header;
            public char[] formatid; // "IRES"
            public List<BLOB> res;

            public ILFF(bool initialize = true)
            {
                header = new ILFF_HEADER(initialize);
                formatid = new char[4];
                res = new List<BLOB>();

                if (initialize)
                {
                    formatid = new char[] { 'I', 'R', 'E', 'S' };
                }
            }

            public ILFF(BinaryReader reader)
            {
                header = new ILFF_HEADER(reader);
                formatid = reader.ReadChars(4);
                res = new List<BLOB>();
                Console.WriteLine("ILFF header: " + header);
                Console.WriteLine("ILFF formatid: " + new string(formatid));
                while (reader.BaseStream.Position < reader.BaseStream.Length)
                {
                    Console.WriteLine("###############################################################");
                    BLOB blob = new BLOB(reader);
                    string blob_type = new string(blob.header.signature);
                    Console.WriteLine("\tILFF BLOB type: " + blob_type);

                    switch (blob_type)
                    {
                        case "NAME":
                            Console.WriteLine("ILFF + NAME");
                            break;
                        case "BODY":
                            Console.WriteLine("ILFF + BODY");
                            break;
                        case "CSTR":
                            Console.WriteLine("ILFF + CSTR");
                            break;
                        case "PATH":
                            Console.WriteLine("ILFF + PATH");
                            break;
                    }
                    res.Add(blob);
                    Console.WriteLine("\tILFF BLOB data: " + Encoding.UTF8.GetString(blob.data));
                }
            }
        }
    }
}
