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

        public void DisplayImage(string key, Bitmap img)
        {
            DisplayImage(key, img.ToOpenCvMat());
        }

        public void DisplayImage(string key, Mat mat)
        {
            var cacheDir = ConfigMgr.GetInstance().CacheDir;
            var storePath = Tools.GetInstance().JoinPath(cacheDir, $"{key}.png");
            mat.SaveImage(storePath);
        }

        private Mat ReadImageFromFile(string filePath)
        {
            return Cv2.ImRead(filePath);
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
            var channels = source.Channels();
            var code = channels == 4 ? ColorConversionCodes.BGRA2GRAY : ColorConversionCodes.BGR2GRAY;
            Cv2.CvtColor(source, gray, code);
            return gray;
        }

        public Mat ToGray(Bitmap bitmap)
        {
            return ToGray(bitmap.ToOpenCvMat());
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
            return bin;
        }

        public Mat ToBinaryPlus(Mat gray, int threshold)
        {
            return ToBinaryPlus(gray, threshold, 0, 255);
        }

        public Mat ToBinaryPlus(Mat gray, int threshold, int lower, int higher)
        {
            var r = new Mat();
            gray.CopyTo(r);
            for (int i = 0; i < gray.Rows; i++)
            {
                for (int j = 0; j < gray.Cols; j++)
                {
                    var clr = gray.GetPixel(i, j);
                    var v = clr.R > threshold ? higher : lower;
                    r.SetPixel(i, j, v);
                }
            }
            return r;
        }

        private int[] dx = { -1, 0, 1, 0 };
        private int[] dy = { 0, 1, 0, -1 };

        private bool InBounds(Bitmap bitmap, int x, int y)
        {
            if (x < 0 || x >= bitmap.Width) return false;
            if (y < 0 || y >= bitmap.Height) return false;
            return true;
        }

        private void CleanBinCornerDFS(Bitmap bitmap, bool[,] vis, int x, int y)
        {
            bitmap.SetPixel(x, y, Color.White);
            vis[x, y] = true;
            for (int k = 0; k < 4; k++)
            {
                var nx = x + dx[k];
                var ny = y + dy[k];
                if (InBounds(bitmap, nx, ny) && !vis[nx, ny])
                {
                    var pix = bitmap.GetPixel(nx, ny);               
                    if (pix.G == 0)
                    {
                        CleanBinCornerDFS(bitmap, vis, nx, ny);
                    }
                }
            }
        }

        public Bitmap CleanBinCorner(Bitmap bitmap)
        {
            var vis = new bool[bitmap.Width, bitmap.Height];
            //CleanBinCornerDFS(bitmap, vis, 0, 0);
            //CleanBinCornerDFS(bitmap, vis, 0, bitmap.Height - 1);
            //CleanBinCornerDFS(bitmap, vis, bitmap.Width - 1, 0);
            //CleanBinCornerDFS(bitmap, vis, bitmap.Width - 1, bitmap.Height - 1);
            for (int x = 0; x < bitmap.Width; x++) CleanBinCornerDFS(bitmap, vis, x, 0);
            for (int x = 0; x < bitmap.Width; x++) CleanBinCornerDFS(bitmap, vis, x, bitmap.Height - 1);
            for (int y = 0; y < bitmap.Height; y++) CleanBinCornerDFS(bitmap, vis, 0, y);
            for (int y = 0; y < bitmap.Height; y++) CleanBinCornerDFS(bitmap, vis, bitmap.Width - 1, y);
            return bitmap;
        }

        public Mat ToReverse(Bitmap bitmap)
        {
            return ToReverse(bitmap.ToOpenCvMat());
        }

        public Mat ToReverse(Mat mat)
        {
            //var r = new Mat();
            //mat.CopyTo(r);
            //for (int i = 0; i < mat.Rows; i++)
            //{
            //    for (int j = 0; j < mat.Cols; j++)
            //    {
            //        var clr = mat.GetPixel(i, j);
            //        r.SetPixel(i, j, 255 - clr.R, 255 - clr.G, 255 - clr.B);
            //    }
            //}
            //return r;
            return (~mat).ToMat();
        }

        
    }
}
