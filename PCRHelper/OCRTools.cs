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
using System.Text.RegularExpressions;

namespace PCRHelper
{
    class OCRTools
    {

        public static bool UsingTesseract
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

        public string OCRWithTesseract(Bitmap bitmap)
        {
            return OCRWithTesseract(bitmap, "chi_sim+eng");
        }

        public string OCRWithTesseract(Bitmap bitmap, string lans)
        {
            var configMgr = ConfigMgr.GetInstance();
            var randStr = DateTime.Now.ToString("yyMMddHHmmss") + new Random().Next(0, 99).ToString("D2");
            var tempImgStorePath = configMgr.GetCacheFileFullPath($"tesseract_{randStr}.png");
            var resName = $"tesseract_result_{randStr}";
            var tempResPathForTess = configMgr.GetCacheFileFullPath($"{resName}");
            var tempResStorePath = configMgr.GetCacheFileFullPath($"{resName}.txt");
            bitmap.Save(tempImgStorePath);
            var tesseratPath = ConfigMgr.GetInstance().TesseractPath;
            var startInfo = new ProcessStartInfo()
            {
                FileName = tesseratPath,
                Arguments = string.Format("{0} {1} -l {2} --psm 6", tempImgStorePath, tempResPathForTess, lans),
                WindowStyle = ProcessWindowStyle.Hidden,
            };
            var proc = Process.Start(startInfo);
            proc.WaitForExit();
            var s = File.ReadAllText(tempResStorePath);
            s = FilterTesseractResult(s);
            File.Delete(tempImgStorePath);
            File.Delete(tempResStorePath);
            return s;
        }

        private Regex filterTail = new Regex("[\\r\\n\\f]+$");
        private string FilterTesseractResult(string result)
        {
            result = result.Replace(" ", "");
            result = filterTail.Replace(result, "");
            return result;
        }

        public string OCR(Mat mat)
        {
            return OCR(mat.ToRawBitmap());
        }

        public string OCR(Bitmap bitmap)
        {
            if (UsingTesseract)
            {
                return OCRWithTesseract(bitmap);
            }
            return "";
        }

        public string ToGrayAndOCR(Mat mat)
        {
            var graphicsTools = GraphicsTools.GetInstance();
            var gray = graphicsTools.ToGray(mat);
            return OCR(mat.ToRawBitmap());
        }

        public string ToGrayAndOCR(Bitmap bitmap)
        {
            return ToGrayAndOCR(bitmap.ToOpenCvMat());
        }

        public string ToBinAndOCR(Mat mat, int threshold)
        {
            var graphicsTools = GraphicsTools.GetInstance();
            var gray = graphicsTools.ToGray(mat);
            var bin = graphicsTools.ToBinary(gray, threshold);
            return OCR(bin.ToRawBitmap());
        }

        public string ToBinAndOCR(Bitmap bitmap, int threshold)
        {
            return ToBinAndOCR(bitmap.ToOpenCvMat(), threshold);
        }

    }
}
