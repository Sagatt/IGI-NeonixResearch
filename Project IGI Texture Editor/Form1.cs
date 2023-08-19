using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.ComponentModel.Design;
using JWC;
using Microsoft.Win32;
using System.Runtime.Remoting.Messaging;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Cryptography;
using System.Xml.Linq;
using System.IO.Pipes;
using static Project_IGI_Texture_Editor.TEX;
using static Project_IGI_Texture_Editor.RES;
using static Project_IGI_Texture_Editor.ZipFileReader;
using System.Collections;
using static System.Net.Mime.MediaTypeNames;
using System.Runtime.ConstrainedExecution;
using Image = System.Drawing.Image;

namespace Project_IGI_Texture_Editor
{
    public partial class Form1 : Form
    {
        const string PROJECT_NAME = "Project I.G.I Texture Editor";

        ByteViewer bv = new ByteViewer();
        Panel hexViewPanel = new Panel();
        Panel imageViewerPanel = new Panel();
        Panel valueViewPanel = new Panel();
        PictureBox pictureBox = new PictureBox();
        ListBox lb = new ListBox();
        ILFF ilff = new ILFF(true);
        LOOP loop = new LOOP(true);
        LOOP_9 loop_9;
        LOOP_11 loop_11;
        public string working_file;
        public TreeNode using_node;

        protected MruStripMenu mruMenu;
        static string mruRegKey = "SOFTWARE\\Project IGI\\Files";

        Stream stream = null;
        enum FILE_TYPES
        {
            TEX,
            RES,
            ZIP,
            BMP,
        }

        FILE_TYPES loaded_file;
        FILE_TYPES selected_file;

        public Form1()
        {
            InitializeComponent();

            CreateByteViewerPanel();
            CreateImageViewerPanel();
            CreateValueViewerPanel();

            mruMenu = new MruStripMenuInline(menuFile, menuRecentFile, new MruStripMenu.ClickedHandler(OnMruFile), mruRegKey + "\\PROJECTIGI", 16);
            mruMenu.LoadFromRegistry();

            ToolStripMenuItem saveImageMenuItem = new ToolStripMenuItem("Save Image");
            ToolStripMenuItem importImageMenuItem = new ToolStripMenuItem("Import Image");
            saveImageMenuItem.Click += new EventHandler(saveImageMenuItem_Click);
            importImageMenuItem.Click += new EventHandler(importImageMenuItem_Click);
            contextMenuStrip1.Items.Add(saveImageMenuItem);
            contextMenuStrip1.Items.Add(importImageMenuItem);
            pictureBox.ContextMenuStrip = contextMenuStrip1;
            pictureBox.MouseWheel += new MouseEventHandler(pictureBox_MouseWheel);
        }

        public void CreateByteViewerPanel()
        {
            hexViewPanel.Dock = DockStyle.Fill;
            hexViewPanel.Show();
            bv.Dock = DockStyle.Fill;
            hexViewPanel.Controls.Add(bv);
            splitContainer1.Panel2.Controls.Add(hexViewPanel);
            bv.Size = new System.Drawing.Size(splitContainer1.Panel2.Width, splitContainer1.Panel2.Height);
            hexViewPanel.Hide();
        }

        public void CreateImageViewerPanel()
        {
            imageViewerPanel.Dock = DockStyle.Fill;
            pictureBox.Dock = DockStyle.Fill;
            pictureBox.Show();
            splitContainer1.Panel2.Controls.Add(imageViewerPanel);
            imageViewerPanel.Controls.Add(pictureBox);
            imageViewerPanel.Show();

            pictureBox.MouseClick += pictureBox1_MouseClick;
            pictureBox.DoubleClick += pictureBox1_DoubleClick;
        }

        public void CreateValueViewerPanel()
        {
            valueViewPanel.Dock = DockStyle.Fill;
            splitContainer1.Panel2.Controls.Add(valueViewPanel);
            valueViewPanel.Controls.Add(lb);
            valueViewPanel.Show();
            lb.Dock = DockStyle.Fill;
            lb.Show();
        }

        private void pictureBox_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta > 0) // Zoom in
            {
                pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            }
            else if (e.Delta < 0) // Zoom out
            {
                pictureBox.SizeMode = PictureBoxSizeMode.Normal;
            }
        }

        public void ResetViewers()
        {
            bv.SetBytes(new byte[] { 0 });

            pictureBox.Image = null;

            lb.Items.Clear();;
        }

        private void OnMruFile(int number, String filename)
        {
            DialogResult result = MessageBox.Show(
                            "Are you sure you want to open again the file: " + filename + "?"
                            , PROJECT_NAME
                            , MessageBoxButtons.YesNo);

            // You may want to use exception handlers in your code

            if (result == DialogResult.Yes)
            {
                mruMenu.SetFirstFile(number);
                HandleFile(filename);
            }
            else
            {
                mruMenu.RemoveFile(number);
            }
        }

        public void HandleFile(string filePath)
        {
            ResetViewers();
            working_file = filePath;
            Console.WriteLine("HandleFile: " + filePath);
            string fileExtension = Path.GetExtension(filePath);
            string fileName = Path.GetFileName(filePath);

            this.Text = PROJECT_NAME + ": " + filePath;

            //bv.SetFile(filePath); // or SetBytes
            mruMenu.AddFile(filePath);
            mruMenu.SaveToRegistry();
            treeView1.Nodes.Clear();
            treeView1.Nodes.Add(fileName);
            stream = File.OpenRead(filePath);

            if (fileExtension == ".res")
            {
                using_node = treeView1.Nodes[0];
                loaded_file = FILE_TYPES.RES;
                // handle .res file
                ProcessResourceFileStream(stream, filePath);
            }
            else if (fileExtension == ".tex")
            {
                using_node = treeView1.Nodes[0];
                loaded_file = FILE_TYPES.TEX;
                // handle .tex file
                ProcessTextureFileStream(stream, filePath);
            }
            else if (fileExtension == ".zip")
            {
                loaded_file = FILE_TYPES.ZIP;
                // handle .tex file
                ProcessZIPFileStream(stream, filePath);
            }
            else
            {
                MessageBox.Show("Invalid file type. Please select a .res or .tex file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            string filePath = openFileDialog1.FileName;
            HandleFile(filePath);
        }

        private void pictureBox1_DoubleClick(object sender, EventArgs e)
        {

            pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                // Show the context menu strip
                pictureBox.ContextMenuStrip.Show(pictureBox, e.Location);
            }
        }

        private void saveImageMenuItem_Click(object sender, EventArgs e)
        {
            SavePictureBox();
        }

        private void importImageMenuItem_Click(object sender, EventArgs e)
        {
            if (loop.version == 11)
                importImageDialog.ShowDialog();
            else
                MessageBox.Show("Not supported on this version.");
        }

        public void SavePictureBox()
        {
            string fileName = Path.GetFileNameWithoutExtension(working_file);
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "PNG Image|*.png";
            saveFileDialog.Title = "Save an Image File";
            saveFileDialog.FileName = fileName;
            saveFileDialog.ShowDialog();
            if (saveFileDialog.FileName != "")
            {
                Image imageToSave = pictureBox.Image;
                imageToSave.Save(saveFileDialog.FileName);
            }
        }

        private void ToggleView() {
            if(imageToolStripMenuItem.Checked)
            {
                imageViewerPanel.Show();
                hexViewPanel.Hide();
                valueViewPanel.Hide();
            }
            if (hexViewToolStripMenuItem.Checked)
            {
                imageViewerPanel.Hide();
                hexViewPanel.Show();
                valueViewPanel.Hide();
            }
            if (valueViewToolStripMenuItem.Checked)
            {
                imageViewerPanel.Hide();
                hexViewPanel.Hide();
                valueViewPanel.Show();
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void imageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            imageToolStripMenuItem.Checked = true;
            hexViewToolStripMenuItem.Checked = false;
            valueViewToolStripMenuItem.Checked = false;
            ToggleView();
        }

        private void hexViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            imageToolStripMenuItem.Checked = false;
            hexViewToolStripMenuItem.Checked = true;
            valueViewToolStripMenuItem.Checked = false;
            ToggleView();
        }

        private void valueViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            imageToolStripMenuItem.Checked = false;
            hexViewToolStripMenuItem.Checked = false;
            valueViewToolStripMenuItem.Checked = true;
            ToggleView();
        }

        public void ProcessTextureFile(Stream stream)
        {
            if (stream == null) return;
            using (BinaryReader reader = new BinaryReader(stream))
            {
                loop = new LOOP(reader);
                switch (loop.version)
                {
                    case 2:
                        Console.WriteLine("TEX 2");
                        break;
                    case 7:
                        Console.WriteLine("TEX 7");
                        break;
                    case 9:
                        Console.WriteLine("TEX 9");
                        loop_9 = new LOOP_9(reader);
                        using_node.Nodes.Clear();
                        for (int i = 0; i < loop_9.imgs.Length; i++)
                        {
                            TreeNode node = new TreeNode();
                            node.Name = using_node.Name + "#" + i;
                            node.Text = using_node.Name + "#" + i;
                            node.Tag = i+".bmp";
                            using_node.Nodes.Add(node);
                        }

                        break;
                    case 11:
                        Console.WriteLine("TEX 11");
                        loop_11 = new LOOP_11(reader);

                        // Set the image data in the PictureBox control
                        Bitmap bitmap = null;
                        if (loop_11.mode == 67 || loop_11.mode == 3)
                            bitmap = new Bitmap((int)loop_11.width, (int)loop_11.height, PixelFormat.Format32bppArgb);
                        else
                            bitmap = new Bitmap((int)loop_11.width, (int)loop_11.height, PixelFormat.Format16bppArgb1555);

                        BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.WriteOnly, bitmap.PixelFormat);

                        Marshal.Copy(loop_11.img.bitmap, 0, bitmapData.Scan0, loop_11.img.bitmap.Length);

                        bitmap.UnlockBits(bitmapData);

                        pictureBox.Image = bitmap;
                        pictureBox.Show();

                        bv.SetBytes(loop_11.img.bitmap); // or SetBytes

                        lb.Items.Clear();
                        lb.Items.Add("File name: " + working_file);
                        lb.Items.Add("Texture Width: " + loop_11.width);
                        lb.Items.Add("Texture Height: " + loop_11.height);

                        string size = loop_11.img.bitmap.Length.ToString();
                        if (loop_11.img.bitmap.Length >= 1024 * 1024)
                        {
                            size = String.Format("{0:##.##} MB", (double)loop_11.img.bitmap.Length / (1024 * 1024));
                        }
                        else if (loop_11.img.bitmap.Length >= 1024)
                        {
                            size = String.Format("{0:##.##} KB", (double)loop_11.img.bitmap.Length / 1024);
                        }
                        lb.Items.Add("Texture Size: " + size);

                        break;
                }
            }
        }

        public void ProcessResourceFile(Stream stream)
        {
            MemoryStream outputStream = new MemoryStream();
            stream.CopyTo(outputStream);
            using (BinaryReader reader = new BinaryReader(outputStream))
            {
                outputStream.Seek(0, SeekOrigin.Begin);
                ilff = new ILFF(reader);
                int counter = 0;
                foreach (var res in ilff.res)
                {
                    if (new string(res.header.signature) == "NAME")
                    {
                        counter++;
                        TreeNode node = new TreeNode();
                        node.Text = Encoding.UTF8.GetString(res.data).Trim('\0');
                        node.Tag = Encoding.UTF8.GetString(res.data).Trim('\0');
                        using_node.Nodes.Add(node);
                    }
                }
                this.Text += " (" + counter + ")";

            }
        }

        public void ProcessTextureFileStream(Stream stream, string filePath)
        {
            // Check if the stream is a MemoryStream
            if (stream is MemoryStream memoryStream)
            {
                ProcessTextureFile(stream);
            }
            // Check if the stream is a FileStream
            else if (stream is FileStream fileStream)
            {
                ProcessTextureFile(stream);
            }
            // The stream is neither a MemoryStream nor a FileStream
            else
            {
                throw new ArgumentException("Unsupported stream type");
            }
        }

        public void ProcessZIPFileStream(Stream stream, string filePath)
        {
            string[] files = ReadZipFile(filePath);
            PopulateTreeNode(treeView1, files, "/");
        }

        public void ProcessResourceFileStream(Stream stream, string filePath)
        {
            // Check if the stream is a MemoryStream
            if (stream is MemoryStream memoryStream)
            {
                ProcessResourceFile(stream);
            }
            // Check if the stream is a FileStream
            else if (stream is FileStream fileStream)
            {
                ProcessResourceFile(stream);
            }
            // The stream is neither a MemoryStream nor a FileStream
            else
            {
                throw new ArgumentException("Unsupported stream type");
            }
        }

        public void LoadBMP(int idx) 
        {
            // Set the image data in the PictureBox control
            Bitmap bitmap = null;
            if (loop_9.imgs[idx].mode == 67 || loop_9.imgs[idx].mode == 3)
                bitmap = new Bitmap((int)loop_9.imgs[idx].width, (int)loop_9.imgs[idx].height, PixelFormat.Format32bppArgb);
            else
                bitmap = new Bitmap((int)loop_9.imgs[idx].width, (int)loop_9.imgs[idx].height, PixelFormat.Format16bppArgb1555);

            BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.WriteOnly, bitmap.PixelFormat);

            Marshal.Copy(loop_9.imgs[idx].bitmap, 0, bitmapData.Scan0, loop_9.imgs[idx].bitmap.Length);

            bitmap.UnlockBits(bitmapData);

            pictureBox.Image = bitmap;
            pictureBox.Show();

            bv.SetBytes(loop_9.imgs[idx].bitmap); // or SetBytes

            lb.Items.Clear();
            lb.Items.Add("File name: " + working_file);
            lb.Items.Add("Texture Width: " + loop_9.imgs[idx].width);
            lb.Items.Add("Texture height: " + loop_9.imgs[idx].height);

            string size = loop_9.imgs[idx].bitmap.Length.ToString();
            if (loop_9.imgs[idx].bitmap.Length >= 1024 * 1024)
            {
                size = String.Format("{0:##.##} MB", (double)loop_9.imgs[idx].bitmap.Length / (1024 * 1024));
            }
            else if (loop_9.imgs[idx].bitmap.Length >= 1024)
            {
                size = String.Format("{0:##.##} KB", (double)loop_9.imgs[idx].bitmap.Length / 1024);
            }
            lb.Items.Add("Texture Size: " + size);
        }

        public void LoadTexture(string textureName) 
        {
            int idx = 0;
            foreach (var res in ilff.res)
            {
                idx++;
                Console.WriteLine("textureName: " + textureName + " | - " + Encoding.UTF8.GetString(res.data).Trim('\0'));
                if (new string(res.header.signature) == "NAME" && textureName == Encoding.UTF8.GetString(res.data).Trim('\0'))
                {
                    //treeView1.Nodes.Add(Encoding.UTF8.GetString(res.data));
                    //MessageBox.Show(Encoding.UTF8.GetString(ilff.res[idx].data));
                    //
                    using (MemoryStream stream = new MemoryStream(ilff.res[idx].data))
                    {
                        ProcessTextureFileStream(stream, "");
                    }
                }
            }
        }

        public void HandleTreeViewItem(TreeViewEventArgs e) 
        {
            // Get the full path of the selected node
            using_node = e.Node;
            string fullPath = e.Node.FullPath;

            string additionalInfo = (string)e.Node.Tag;

            // Do something with the file path
            Console.WriteLine($"Working file: {working_file}");
            Console.WriteLine($"Selected file: {fullPath}");
            Console.WriteLine($"Selected file additionalInfo: {additionalInfo}");

            string fileName = Path.GetFileNameWithoutExtension(additionalInfo);
            string fileExtension = Path.GetExtension(additionalInfo);

            if (fileExtension == null || fileExtension == "") return;

            Console.WriteLine($"Selected file fileName: {fileName}");
            Console.WriteLine($"Selected file fileExtension: {fileExtension}");
            switch (fileExtension)
            {
                case ".tex":
                    selected_file = FILE_TYPES.TEX;
                    break;
                case ".res":
                    selected_file = FILE_TYPES.RES;
                    break;
                case ".bmp":
                    selected_file = FILE_TYPES.BMP;
                    break;
            }

            switch (loaded_file)
            {
                case FILE_TYPES.ZIP:
                    {
                        // Find resource or texture
                        if (selected_file == FILE_TYPES.TEX)
                        {
                            if (additionalInfo.StartsWith("LOCAL:"))
                                LoadTexture(additionalInfo);
                            else
                                ProcessTextureFile(GetZipFileItem(additionalInfo));
                        }
                        else if (selected_file == FILE_TYPES.RES)
                        {
                            ProcessResourceFile(GetZipFileItem(additionalInfo));
                        }
                        else if (selected_file == FILE_TYPES.BMP)
                        {
                            int idx = Int32.Parse(fileName);
                            LoadBMP(idx);
                        }
                        break;
                    }
                case FILE_TYPES.RES:
                    // Find resource
                    if (selected_file == FILE_TYPES.TEX)
                    {
                        LoadTexture(e.Node.Tag.ToString());
                    }
                    break;
                case FILE_TYPES.TEX:
                    // Find resource
                    if (selected_file == FILE_TYPES.TEX)
                    {
                        LoadTexture(e.Node.Tag.ToString());
                    }
                    else if (selected_file == FILE_TYPES.BMP)
                    {
                        int idx = Int32.Parse(fileName);
                        LoadBMP(idx);
                    }
                    break;
            }
        }


        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            HandleTreeViewItem(e);
        }

        public void PopulateTreeNode(TreeView root, string[] list, string pathSeparator)
        {
            char[] cachedpathseparator = pathSeparator.ToCharArray();
            foreach (string path in list)
            {

                // Split the path into individual directories
                string[] directories = path.Split(cachedpathseparator);

                // Start at the root node of the TreeView
                TreeNode currentNode = root.Nodes[0];
                Console.WriteLine($"Processing path: {path}");

                // Loop through each directory in the path
                foreach (string directory in directories)
                {
                    if (!string.IsNullOrEmpty(directory))
                    {
                        // Check if the directory already exists
                        bool directoryExists = false;
                        foreach (TreeNode childNode in currentNode.Nodes)
                        {
                            if (childNode.Text == directory)
                            {
                                currentNode = childNode;
                                directoryExists = true;
                                break;
                            }
                        }

                        // If the directory does not exist, create a new node for it
                        if (!directoryExists)
                        {
                            TreeNode newNode = new TreeNode(directory);

                            newNode.Tag = path;
                            currentNode.Nodes.Add(newNode);
                            currentNode = newNode;
                            Console.WriteLine($"Added node for directory: {directory}");
                        }
                    }
                }
            }
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
        }

        private void exportAllTexturesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (working_file == null || working_file == "") return;

            // Create an instance of the FolderBrowserDialog class
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();

            // Set the initial directory to be displayed in the dialog
            folderDialog.SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            switch (loaded_file)
            {
            case FILE_TYPES.TEX:
                // Export only the loaded TEX.
            
                SavePictureBox();
                break;
            case FILE_TYPES.RES:
                {
                    // Export the loaded textures in the RES.
                    if (folderDialog.ShowDialog() == DialogResult.OK)
                    {

                            // Get the selected folder path
                            string folderPath = folderDialog.SelectedPath;
                            Console.WriteLine(folderPath);
                            // Do something with the folder path
                            int idx = 0;
                            string file_name = "";
                            foreach (var res in ilff.res)
                            {
                                if (new string(res.header.signature) == "BODY")
                                {
                                    //string final_path = folderPath + "\\" + Path.GetFileNameWithoutExtension(file_name)+".png";

                                    //Console.WriteLine(final_path);
                                    using (MemoryStream stream = new MemoryStream(ilff.res[idx].data))
                                    {
                                        using (BinaryReader reader = new BinaryReader(stream))
                                        {
                                            loop = new LOOP(reader);
                                            switch (loop.version)
                                            {
                                                case 2:
                                                    Console.WriteLine("TEX 2");
                                                    break;
                                                case 7:
                                                    Console.WriteLine("TEX 7");
                                                    break;
                                                case 9:
                                                    Console.WriteLine("TEX 9");
                                                    //string file = folderPath + "\\" + Path.GetFileNameWithoutExtension(file_name) + i + ".png";
                                                    break;
                                                case 11:
                                                    Console.WriteLine("TEX 11");
                                                    loop_11 = new LOOP_11(reader);
                                                    Bitmap bitmap = null;
                                                    if (loop_11.mode == 67 || loop_11.mode == 3)
                                                        bitmap = new Bitmap((int)loop_11.width, (int)loop_11.height, PixelFormat.Format32bppArgb);
                                                    else
                                                        bitmap = new Bitmap((int)loop_11.width, (int)loop_11.height, PixelFormat.Format16bppArgb1555);

                                                    BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.WriteOnly, bitmap.PixelFormat);

                                                    Marshal.Copy(loop_11.img.bitmap, 0, bitmapData.Scan0, loop_11.img.bitmap.Length);

                                                    bitmap.UnlockBits(bitmapData);
                                                    bitmap.Save(folderPath + "\\" + Path.GetFileNameWithoutExtension(file_name) + ".png", System.Drawing.Imaging.ImageFormat.Png);
                                                    break;
                                            }
                                        }
                                    }
                                }
                                else
                                    file_name = Encoding.UTF8.GetString(res.data).Trim('\0');
                                idx++;
                            }

                        }
                    break;
                }
            case FILE_TYPES.ZIP:
                // Export the loaded stuff inside ZIP.
                // TODO: ZIP Exportation all stuff
                MessageBox.Show("TODO: ZIP Exportation all stuff");
                break;
            }
        }

        public struct IMAGE_DATA
        {
            public int width;
            public int height;
            public byte[] img;

        }

        public void ImportFile(string filePath)
        {
            const string fileName = "Import.tex";
            IMAGE_DATA image_data = GetImageDataFromPNG(filePath);
            using (var stream = File.Open(fileName, FileMode.Create))
            {
                using (var writer = new BinaryWriter(stream, Encoding.UTF8, false))
                {

                    switch (loop.version)
                    {
                        case 11:
                            loop_11.width1 = (ushort)image_data.width;
                            loop_11.height1 = (ushort)image_data.height;
                            loop_11.width = (ushort)image_data.width;
                            loop_11.height = (ushort)image_data.height;
                            loop_11.img.bitmap = image_data.img;
                            LOOP_11.WriteLOOPToFile(writer, loop_11, image_data.img);

                            // Set the image data in the PictureBox control
                            Bitmap bitmap = null;
                            if (loop_11.mode == 67 || loop_11.mode == 3)
                                bitmap = new Bitmap((int)loop_11.width, (int)loop_11.height, PixelFormat.Format32bppArgb);
                            else
                                bitmap = new Bitmap((int)loop_11.width, (int)loop_11.height, PixelFormat.Format16bppArgb1555);

                            BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.WriteOnly, bitmap.PixelFormat);

                            Marshal.Copy(loop_11.img.bitmap, 0, bitmapData.Scan0, loop_11.img.bitmap.Length);

                            bitmap.UnlockBits(bitmapData);

                            pictureBox.Image = bitmap;

                            break;
                        case 9:
                            //LOOP_9.WriteLOOPToFile(filePath, loop_9);
                            break;
                    }
                }
            }

        }

        public IMAGE_DATA GetImageDataFromPNG(string imagePath)
        {
            IMAGE_DATA data;
            // Load the PNG image using Bitmap class
            using (Bitmap bitmap = new Bitmap(imagePath))
            {
                // Lock the bitmap's data to access its pixels
                Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
                data.width = bitmap.Width;
                data.height = bitmap.Height;
                BitmapData bitmapData = bitmap.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

                try
                {
                    // Calculate the total number of bytes required to store the bitmap data
                    int bytes = Math.Abs(bitmapData.Stride) * bitmapData.Height;

                    // Create a byte array to hold the bitmap data
                    byte[] rawData = new byte[bytes];

                    // Copy the bitmap data to the byte array
                    System.Runtime.InteropServices.Marshal.Copy(bitmapData.Scan0, rawData, 0, bytes);
                    data.img = rawData;
                    //return rawData;
                    return data;
                }
                finally
                {
                    // Unlock the bitmap's data
                    bitmap.UnlockBits(bitmapData);
                }
            }
        }


        private void importImageDialog_FileOk(object sender, CancelEventArgs e)
        {
            string filePath = importImageDialog.FileName;
            ImportFile(filePath);
        }
    }
}
