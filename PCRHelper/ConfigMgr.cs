using OpenCvSharp;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using CvSize = OpenCvSharp.Size;

namespace PCRHelper
{
    enum PCRRegion
    {
        Mainland,
        Taiwan,
        Japan,
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
            //InitTesseractConfig();
            InitAdbConfig();
            InitPCRConfig();
        }

        public void InitTesseractConfig()
        {
            if (!OCRTools.UsingTesseract) return;
            var tesseractPath = PCRHelper.Properties.Settings.Default.TesseractPath;
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

        public PCRRegion ParsePCRRegion(string s)
        {
            s = s.ToLower();
            switch(s)
            {
                case "mainland": return PCRRegion.Mainland;
                case "taiwan": return PCRRegion.Taiwan;
                case "japan": return PCRRegion.Japan;
            }
            return PCRRegion.Mainland;
        }

        public PCRRegion PCRRegion
        {
            get
            {
                return ParsePCRRegion(PCRHelper.Properties.Settings.Default.PCRRegionStr);
            }
            set
            {
                PCRHelper.Properties.Settings.Default.PCRRegionStr = value.ToString();
                PCRHelper.Properties.Settings.Default.Save();
            }
        }

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
                    //return "chi_tra+eng";
                    return "chi_sim+eng";
                }
                throw new BreakException("OCRLans");
            }
        }

        public bool FixedViewportTopBottomY { get; set; } = false;

        public int FixedViewportTopY { get; set; } = 36;

        public int FixedViewportBottomY { get; set; } = 722;

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

        public Mat GetRawPCRExImg(string name)
        {
            var fullPath = GetPCRExImgFullPath(name);
            var mat = new Mat(fullPath, ImreadModes.Unchanged);
            return mat;
        }

        public Mat GetPCRExImg(string name, Bitmap viewportCapture, RECT viewportRect)
        {
            var viewportMat = viewportCapture.ToOpenCvMat();
            return GetPCRExImg(name, viewportMat, viewportRect);
        }

        public Mat GetPCRExImg(string name, Mat viewportMat, RECT viewportRect)
        {
            var viewportMatExPath = GetPCRExImgFullPath("capture.png");
            var viewportMatEx = new Mat(viewportMatExPath, ImreadModes.Unchanged);
            var widScale = 1.0 * viewportRect.Width / viewportMatEx.Width;
            var heiScale = 1.0 * viewportRect.Height / viewportMatEx.Height;
            var fullPath = GetPCRExImgFullPath(name);
            var mat = new Mat(fullPath, ImreadModes.Unchanged);
            mat = mat.Resize(new CvSize(mat.Width * widScale, mat.Height * heiScale));
            return mat;
        }
    }
}
