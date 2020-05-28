using System;
using OpenCvSharp;
using System.Collections.Generic;
using System.Drawing;
using OpenCvSharp.Extensions;

namespace PCRHelper
{
    class GraphicsTools
    {
        private static GraphicsTools instance;

        public static GraphicsTools GetInstance()
        {
            if (instance == null)
            {
                instance = new GraphicsTools();
            }
            return instance;
        }

        private GraphicsTools()
        {
            
        }

        internal void ShowImage(string key, Image img)
        {
            var cacheDir = ConfigMgr.GetInstance().CacheDir;
            var storePath = Tools.GetInstance().JoinPath(cacheDir, $"{key}.png");
            img.Save(storePath);
        }

        internal void ShowImage(string key, Mat mat)
        {
            //Cv2.ImShow(key, img);
            var cacheDir = ConfigMgr.GetInstance().CacheDir;
            var storePath = Tools.GetInstance().JoinPath(cacheDir, $"{key}.png");
            mat.SaveImage(storePath);
        }

        private Mat ReadImageFromFile(string filePath)
        {
            return Cv2.ImRead(filePath, ImreadModes.Color);
        }

        public Mat ToMat(Image image)
        {
            var bitmap = image as Bitmap;
            return bitmap.ToMat();
        }

        public Image ToImage(Mat mat)
        {
            return mat.ToBitmap();
        }

        public Mat GetChildMatByRECT(Mat mat, RECT rect)
        {
            var xRange = new Range(rect.x1, rect.x2);
            var yRange = new Range(rect.y1, rect.y2);
            var child = mat[yRange, xRange];
            return child;
        }

        public Mat ToGray(string filePath)
        {
            var mat = ReadImageFromFile(filePath);
            return ToGray(mat);
        }

        public Mat ToGray(Mat source)
        {
            var gray = new Mat();
            Cv2.CvtColor(source, gray, ColorConversionCodes.BGR2GRAY);
            ShowImage("ToGray-Gray", gray);
            return gray;
        }

        public Mat ToGrayBinary(string filePath, int threshold)
        {
            var mat = ReadImageFromFile(filePath);
            return ToGrayBinary(mat, threshold);
        }

        public Mat ToGrayBinary(Mat source, int threshold)
        {
            var gray = ToGray(source);
            return ToBinary(gray, threshold);
        }

        public Mat ToBinary(Mat gray, int threshold)
        {
            var bin = new Mat();
            Cv2.Threshold(gray, bin, threshold, 255, ThresholdTypes.Binary);
            ShowImage("ToBinary-Bin", bin);
            return bin;
        }

        public List<LineSegmentPolar> GetHoughLines(string filePath)
        {
            var mat = ReadImageFromFile(filePath);
            return GetHoughLines(mat);
        }

        public List<LineSegmentPolar> GetHoughLines(Mat source)
        {
            var temp = new Mat();
            var gray = new Mat();
            //边缘检测
            Cv2.Canny(source, temp, 50, 200, 3);
            ShowImage("GetHoughLines-Canny", temp);
            //灰化
            Cv2.CvtColor(source, gray, ColorConversionCodes.BGR2GRAY);
            ShowImage("GetHoughLines-Gray", temp);
            var r = Cv2.HoughLines(gray, 1, Cv2.PI, 150, 0, 0);
            return new List<LineSegmentPolar>(r);
        }
    }
}
