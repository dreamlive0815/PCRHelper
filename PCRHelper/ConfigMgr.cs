using System;
using System.IO;
using System.Windows.Forms;

namespace PCRHelper
{
    enum PCRRegion
    {
        Mainland = 0,
        Taiwan = 1,
    }

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
                InitTesseractConfig();
            }
            InitAdbConfig();
            InitPCRConfig();
        }

        private void InitTesseractConfig()
        {
            var tesseractPath = TesseractPath;
            if (!File.Exists(tesseractPath))
            {
                SetTesseractPath();
            }
        }

        public void SetTesseractPath()
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

        private void InitAdbConfig()
        {
            var adbServerPath = AdbServerExePath;
            if (!File.Exists(adbServerPath))
            {
                SetAdbServerExePath();
            }
        }

        public void SetAdbServerExePath()
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

        private void InitPCRConfig()
        {
            var pcrExImgsDir = PCRExImgsDir;
            if (!Directory.Exists(pcrExImgsDir))
            {
                SetPCRExImgsDir();
            }
        }

        public void SetPCRExImgsDir()
        {
            var folderDialog = new FolderBrowserDialog();
            folderDialog.Description = "请选择保存PCR样图的目录";
            if (folderDialog.ShowDialog() != DialogResult.OK)
            {
                Application.Exit();
            }
            PCRExImgsDir = folderDialog.SelectedPath;
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

        public string PCRExImgsDir
        {
            get
            {
                return PCRHelper.Properties.Settings.Default.PCRExImgsDir;
            }
            set
            {
                PCRHelper.Properties.Settings.Default.PCRExImgsDir = value;
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

        public PCRRegion PCRRegion { get; set; } = PCRRegion.Mainland;

        public string OCRLans
        {
            get
            {
                if (PCRRegion == PCRRegion.Mainland)
                {
                    return "chi_sim+eng";
                }
                else if (PCRRegion == PCRRegion.Taiwan)
                {
                    return "chi_tra+eng";
                }
                throw new Exception("OCRLans");
            }
        }

        public string GetCacheFileFullPath(string relativePath)
        {
            var cacheDir = new DirectoryInfo(this.CacheDir).FullName;
            var path = Tools.GetInstance().JoinPath(cacheDir, relativePath);
            return path;
        }

        public string GetPCRExImgFullPath(string imgName)
        {
            var path = Tools.GetInstance().JoinPath(PCRExImgsDir, PCRRegion.ToString(), imgName);
            return path;
        }
    }
}
