using System;
using System.IO;
using System.Windows.Forms;

namespace PCRHelper
{
    class ConfigMgr
    {

        private static ConfigMgr instance;

        private static readonly string cacheDir = "./cache";

        public static ConfigMgr GetInstance()
        {
            if (instance == null)
            {
                instance = new ConfigMgr();
            }
            return instance;
        }

        private ConfigMgr()
        {
        }

        public void Init()
        {
            if (OCRTools.UsingTesseract)
            {
                InitOCRConfig();
            }
            InitAdbConfig();
        }

        private void InitOCRConfig()
        {
            var tesseractPath = TesseractPath;
            if (!File.Exists(tesseractPath))
            {
                var openDialog = new OpenFileDialog();
                openDialog.Title = "请选择Tesseract程序所在的目录";
                openDialog.FileName = "tesseract.exe";
                openDialog.Filter = "tesseract.exe|tesseract.exe";
                if (openDialog.ShowDialog() != DialogResult.OK)
                {
                    Application.Exit();
                }
                TesseractPath = openDialog.FileName;
            }
        }

        private void InitAdbConfig()
        {
            var adbServerPath = AdbServerExePath;
            if (!File.Exists(adbServerPath))
            {
                var openDialog = new OpenFileDialog();
                openDialog.Title = "请选择AdbServer程序所在的目录";
                openDialog.FileName = "adb_server.exe";
                openDialog.Filter = "adb_server.exe|adb_server.exe";
                if (openDialog.ShowDialog() != DialogResult.OK)
                {
                    Application.Exit();
                }
                AdbServerExePath = openDialog.FileName;
            }
        }

        public string TesseractPath
        {
            get
            {
                return PCRHelper.Properties.Settings.Default.TesseractPath;
            }
            set
            {
                PCRHelper.Properties.Settings.Default.TesseractPath = value;
                PCRHelper.Properties.Settings.Default.Save();
            }
        }

        public string AdbServerExePath
        {
            get
            {
                return PCRHelper.Properties.Settings.Default.AdbServerExePath;
            }
            set
            {
                PCRHelper.Properties.Settings.Default.AdbServerExePath = value;
                PCRHelper.Properties.Settings.Default.Save();
            }
        }


        public string CacheDir
        {
            get
            {
                if (!Directory.Exists(cacheDir))
                {
                    Directory.CreateDirectory(cacheDir);
                }
                return cacheDir;
            }
        }

        public string GetCacheFileFullPath(string relativePath)
        {
            var cacheDir = new DirectoryInfo(this.CacheDir).FullName;
            var path = Tools.GetInstance().JoinPath(cacheDir, relativePath);
            return path;
        }
    }
}
