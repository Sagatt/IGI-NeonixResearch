using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using static Project_IGI_Texture_Editor.TEX;
using static Project_IGI_Texture_Editor.RES;
using System.Runtime.InteropServices.ComTypes;

namespace Project_IGI_Texture_Editor
{
    internal class TEX
    {

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct BITMAP
        {
            public long lod;
            public long width;
            public long height;
            public long mode;

            [MarshalAs(UnmanagedType.ByValArray)]
            public byte[] bitmap;

            public BITMAP(long lod, long width, long height, long mode)
            {
                this.lod = lod;
                this.width = width;
                this.height = height;
                this.mode = mode;
                this.bitmap = new byte[0];
            }

            public void Read(BinaryReader reader)
            {
                long depth = 0;
                switch (mode)
                {
                    case 2: depth = 2; break;
                    case 3: depth = 4; break;
                    case 67: depth = 4; break;
                }

                long w = width / ((long)1L << (int)lod);
                long h = height / ((long)1L << (int)lod);
                bitmap = reader.ReadBytes((int)(w * h * depth));
            }

            public void Write(BinaryWriter writer)
            {
                writer.Write(lod);
                writer.Write(width);
                writer.Write(height);
                writer.Write(mode);
                writer.Write(bitmap);
            }
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct LOOP
        {
            public char[] signature; // "LOOP"
            public uint version;

            public LOOP(bool initialize = true)
            {
                signature = new char[4];
                version = 0;

                if (initialize)
                {
                    signature = new char[] { 'L', 'O', 'O', 'P' };
                }
            }

            public LOOP(BinaryReader reader)
            {
                signature = reader.ReadChars(4);
                version = reader.ReadUInt32();
            }
            public static void WriteLOOP(BinaryWriter writer, byte version) {

                writer.Write(new byte[] { 76, 79, 79, 80 });
                writer.Write(new byte[] { version, 0, 0, 0 });
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct LOOP_6
        {
            public ushort _0;
            public ushort _1;
            public ushort _2;
            public ushort _3;
            public uint count_x;
            public uint count_y;

            public struct Item
            {
                public uint reserved_0;
                public uint reserved_1;
                public uint reserved_2;
                public uint reserved_3;
            }

            public Item[] items;

            public LOOP_6(BinaryReader reader)
            {
                _0 = reader.ReadUInt16();
                _1 = reader.ReadUInt16();
                _2 = reader.ReadUInt16();
                _3 = reader.ReadUInt16();
                count_x = reader.ReadUInt32();
                count_y = reader.ReadUInt32();

                items = new Item[count_x * count_y];

                for (int i = 0; i < (count_x * count_y); i++)
                {
                    items[i] = new Item();
                    items[i].reserved_0 = reader.ReadUInt32();
                    items[i].reserved_1 = reader.ReadUInt32();
                    items[i].reserved_2 = reader.ReadUInt32();
                    items[i].reserved_3 = reader.ReadUInt32();

                }
            }
        }


        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct LOOP_9
        {
            public uint _0;
            public uint _1;
            public uint _2;
            public uint _3;
            public uint _4;
            public uint offset;
            public uint count;
            public uint _5;
            public uint width;
            public uint height;
            public uint mode;

            public struct Item
            {
                public uint offset;
                public uint mode;
                public ushort width_line;
                public ushort width;
                public ushort height;
                public ushort _0;
                public uint reserved_0;
                public uint reserved_1;
                public uint reserved_2;
                public uint reserved_3;
            }

            public Item[] items;
            public BITMAP[] imgs;
            //public LOOP_6 tail;

            //public LOOP_9(bool initialize = true)
            //{
            //    if (initialize)
            //    {
            //        _0 = 0;
            //        _1 = 0;
            //        _2 = 0;
            //        _3 = 0;
            //        _4 = 0;
            //        offset = 0;
            //        count = 0;
            //        _5 = 0;
            //        width = 0;
            //        height = 0;
            //        mode = 0;
            //        items = new Item[0];
            //        imgs = new BITMAP[0];
            //    }
            //}

            public LOOP_9(BinaryReader reader)
            {
                _0 = reader.ReadUInt32();
                _1 = reader.ReadUInt32();
                _2 = reader.ReadUInt32();
                _3 = reader.ReadUInt32();
                _4 = reader.ReadUInt32();
                offset = reader.ReadUInt32();
                count = reader.ReadUInt32();
                _5 = reader.ReadUInt32();
                width = reader.ReadUInt32();
                height = reader.ReadUInt32();
                mode = reader.ReadUInt32();

                Console.WriteLine("TEX 9 _0: " + _0);
                Console.WriteLine("TEX 9 _1: " + _1);
                Console.WriteLine("TEX 9 _2: " + _2);
                Console.WriteLine("TEX 9 _3: " + _3);
                Console.WriteLine("TEX 9 _4: " + _4);
                Console.WriteLine("TEX 9 offset: " + offset);
                Console.WriteLine("TEX 9 count: " + count);
                Console.WriteLine("TEX 9 _5: " + _5);
                Console.WriteLine("TEX 9 width: " + width);
                Console.WriteLine("TEX 9 height: " + height);
                Console.WriteLine("TEX 9 mode: " + mode);

                items = new Item[count];

                for (int i = 0; i < count; i++)
                {
                    items[i] = new Item();
                    items[i].offset = reader.ReadUInt32();
                    items[i].mode = reader.ReadUInt32();
                    items[i].width_line = reader.ReadUInt16();
                    items[i].width = reader.ReadUInt16();
                    items[i].height = reader.ReadUInt16();
                    items[i]._0 = reader.ReadUInt16();
                    items[i].reserved_0 = reader.ReadUInt32();
                    items[i].reserved_1 = reader.ReadUInt32();
                    items[i].reserved_2 = reader.ReadUInt32();
                    items[i].reserved_3 = reader.ReadUInt32();
                }

                imgs = new BITMAP[count];

                for (int i = 0; i < count; i++)
                {
                    imgs[i] = new BITMAP(0, width, height, mode);
                    imgs[i].Read(reader);
                }

                //tail = new LOOP_6(reader);
            }
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct LOOP_11
        {
            public uint mode;
            public uint multi;
            public uint _0;
            public ushort _1;
            public ushort width1;
            public ushort height1;
            public ushort width;
            public ushort height;
            public ushort depth;

            private long lod;

            public BITMAP img;

            public LOOP_11(BinaryReader reader)
            {
                mode = reader.ReadUInt32();
                multi = reader.ReadUInt32();
                _0 = reader.ReadUInt32();
                _1 = reader.ReadUInt16();
                width1 = reader.ReadUInt16();
                height1 = reader.ReadUInt16();
                width = reader.ReadUInt16();
                height = reader.ReadUInt16();
                depth = reader.ReadUInt16();

                lod = 0;

                img = new BITMAP(0, width, height, mode);
                img.Read(reader);

                Console.WriteLine("TEX 11 mode: " + mode);
                Console.WriteLine("TEX 11 multi: " + multi);
                Console.WriteLine("TEX 11 _0: " + _0);
                Console.WriteLine("TEX 11 _1: " + _1);
                Console.WriteLine("TEX 11 width1: " + width1);
                Console.WriteLine("TEX 11 height1: " + height1);
                Console.WriteLine("TEX 11 width: " + width);
                Console.WriteLine("TEX 11 height: " + height);
                Console.WriteLine("TEX 11 depth: " + depth);
                Console.WriteLine("TEX 11 lod: " + lod);
                Console.WriteLine("TEX 11 img: " + img.bitmap);
                Console.WriteLine(Encoding.UTF8.GetString(img.bitmap));
            }

            public static void WriteLOOPToFile(BinaryWriter writer, LOOP_11 loop, byte[] img)
            {

                LOOP.WriteLOOP(writer, 11);

                writer.Write(loop.mode);
                writer.Write(loop.multi);
                writer.Write(loop._0);
                writer.Write(loop._1);
                writer.Write(loop.width1);
                writer.Write(loop.height1);
                writer.Write(loop.width);
                writer.Write(loop.height);
                writer.Write(loop.depth);

                writer.Write(img);
            }
        }
    }
}
