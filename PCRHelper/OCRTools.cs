using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Tesseract;
using OpenCvSharp;

namespace PCRHelper
{
    class OCRTools
    {

        public static bool UsingTesseractExe
        {
            get { return true; }
        }

        private static OCRTools instance;

        public static OCRTools GetInstance()
        {
            if (instance == null)
            {
                instance = new OCRTools();
            }
            return instance;
        }

        //private TesseractEngine engine;
        //private string tessdataDir = "./tessdata";

        private OCRTools()
        {
            //var tessdataDir = new DirectoryInfo(this.tessdataDir).FullName + "/";
            //engine = new TesseractEngine(tessdataDir, "eng", EngineMode.Default);
        }

        private string OCRUsingTesseractExe(Image img)
        {
            var configMgr = ConfigMgr.GetInstance();
            var tempImgStorePath = configMgr.GetCacheFileFullPath("tesseract-temp.png");
            var resName = "tesseract-result";
            var tempResPathForTess = configMgr.GetCacheFileFullPath($"{resName}");
            var tempResStorePath = configMgr.GetCacheFileFullPath($"{resName}.txt");
            img.Save(tempImgStorePath);
            var tesseratPath = ConfigMgr.GetInstance().TesseractPath;
            var startInfo = new ProcessStartInfo()
            {
                FileName = tesseratPath,
                Arguments = string.Format("{0} {1} -l chi_sim+eng --psm 6", tempImgStorePath, tempResPathForTess),
                WindowStyle = ProcessWindowStyle.Hidden,
            };
            var proc = Process.Start(startInfo);
            proc.WaitForExit();
            var s = File.ReadAllText(tempResStorePath);
            s = FilterTesseractResult(s);
            File.Delete(tempImgStorePath);
            File.WriteAllText(tempResStorePath, "");
            return s;
        }

        private string FilterTesseractResult(string result)
        {
            result = result.Replace(" ", "");
            return result;
        }

        public string OCR(Mat mat)
        {
            return OCR(GraphicsTools.GetInstance().ToImage(mat));
        }

        public string OCR(Image img)
        {
            if (UsingTesseractExe)
            {
                return OCRUsingTesseractExe(img);
            }
            return "";
        }

        public string ToGrayAndOCR(Mat mat)
        {
            var graphicsTools = GraphicsTools.GetInstance();
            var gray = graphicsTools.ToGray(mat);
            //graphicsTools.ShowImage("OCR-Gray", gray);
            return OCR(graphicsTools.ToImage(gray));
        }

        public string ToGrayAndOCR(Image img)
        {
            var graphicsTools = GraphicsTools.GetInstance();
            return ToGrayAndOCR(graphicsTools.ToMat(img));
        }

        public string ToBinAndOCR(Mat mat, int threshold)
        {
            var graphicsTools = GraphicsTools.GetInstance();
            var gray = graphicsTools.ToGray(mat);
            //graphicsTools.ShowImage("OCR-Gray", gray);
            var bin = graphicsTools.ToBinary(gray, threshold);
            //graphicsTools.ShowImage("OCR-Bin", bin);
            return OCR(graphicsTools.ToImage(bin));
        }

        public string ToBinAndOCR(Image img, int threshold)
        {
            var graphicsTools = GraphicsTools.GetInstance();
            return ToBinAndOCR(graphicsTools.ToMat(img), threshold);
        }

    }
}
