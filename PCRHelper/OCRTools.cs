﻿using System;
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
            return OCRWithTesseract(bitmap.ToOpenCvMat(), ConfigMgr.GetInstance().OCRLans);
        }

        public string OCRWithTesseract(Mat mat)
        {
            return OCRWithTesseract(mat, ConfigMgr.GetInstance().OCRLans);
        }

        public string OCRWithTesseract(Mat mat, string lans)
        {
            var configMgr = ConfigMgr.GetInstance();
            var randStr = DateTime.Now.ToString("yyMMddHHmmssffff") + mat.GetHashCode().ToString();
            var tempImgStorePath = configMgr.GetCacheFileFullPath($"tesseract_{randStr}.png");
            var resName = $"tesseract_result_{randStr}";
            var tempResPathForTess = configMgr.GetCacheFileFullPath($"{resName}");
            var tempResStorePath = configMgr.GetCacheFileFullPath($"{resName}.txt");
            mat.SaveImage(tempImgStorePath);
            var tesseratPath = ConfigMgr.GetInstance().TesseractPath;
            var startInfo = new ProcessStartInfo()
            {
                FileName = tesseratPath,
                Arguments = string.Format("{0} {1} -l {2} --psm 6", tempImgStorePath, tempResPathForTess, lans),
                WindowStyle = ProcessWindowStyle.Hidden,
                //RedirectStandardOutput = true,
                //RedirectStandardError = true,
                //UseShellExecute = false,
            };
            var s = "";
            try
            {
                var proc = Process.Start(startInfo);
                proc.WaitForExit();
                //var output = proc.StandardOutput.ReadToEnd();
                //var error = proc.StandardError.ReadToEnd();
                s = File.ReadAllText(tempResStorePath);
                s = FilterTesseractResult(s);
                LogTools.GetInstance().Info("OCR:" + s);
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                File.Delete(tempImgStorePath);
                File.Delete(tempResStorePath);
            }
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
            if (UsingTesseract)
            {
                return OCRWithTesseract(mat);
            }
            return "";
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
            return OCR(mat);
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
