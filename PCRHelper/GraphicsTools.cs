using System;
using OpenCvSharp;
using System.Collections.Generic;
using System.Drawing;
using OpenCvSharp.Extensions;
using CvPoint = OpenCvSharp.Point;

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
            if (key == "DoCapture")
            {
                mat.SaveImage(storePath);
            }
            else
            {
                //Cv2.ImShow(key, mat);
                mat.SaveImage(storePath);
            }
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
                    r.SetPixel(i, j, v, v, v);
                }
            }
            return r;
        }

        private int[] dx = { -1, 0, 1, 0 };
        private int[] dy = { 0, 1, 0, -1 };
        private int[] dr = { -1, 0, 1, 0 };
        private int[] dc = { 0, 1, 0, -1 };

        private bool InBounds(Bitmap bitmap, int x, int y)
        {
            if (x < 0 || x >= bitmap.Width) return false;
            if (y < 0 || y >= bitmap.Height) return false;
            return true;
        }

        private bool InBounds(Mat mat, int r, int c)
        {
            if (r < 0 || r >= mat.Rows) return false;
            if (c < 0 || c >= mat.Cols) return false;
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

        private void CleanBinCornerDFS(Mat mat, bool[,] vis, int r, int c)
        {
            mat.SetPixel(r, c, 255, 255, 255);
            vis[r, c] = true;
            for (int k = 0; k < 4; k++)
            {
                var nr = r + dr[k];
                var nc = c + dc[k];
                if (InBounds(mat, nr, nc) && !vis[nr, nc])
                {
                    var pix = mat.GetPixel(nr, nc);
                    if (pix.R == 0)
                    {
                        CleanBinCornerDFS(mat, vis, nr, nc);
                    }
                }
            }
        }

        public Bitmap CleanBinCorner(Bitmap bitmap)
        {
            var res = new Bitmap(bitmap);
            var vis = new bool[res.Width, res.Height];
            for (int x = 0; x < res.Width; x++) CleanBinCornerDFS(res, vis, x, 0);
            for (int x = 0; x < res.Width; x++) CleanBinCornerDFS(res, vis, x, res.Height - 1);
            for (int y = 0; y < res.Height; y++) CleanBinCornerDFS(res, vis, 0, y);
            for (int y = 0; y < res.Height; y++) CleanBinCornerDFS(res, vis, res.Width - 1, y);
            return res;
        }

        public Mat CleanBinCorner(Mat mat)
        {
            var res = new Mat();
            mat.CopyTo(res);
            var vis = new bool[res.Rows, res.Cols];
            for (int r = 0; r < res.Rows; r++) CleanBinCornerDFS(res, vis, r, 0);
            for (int r = 0; r < res.Rows; r++) CleanBinCornerDFS(res, vis, r, res.Cols - 1);
            for (int c = 0; c < res.Cols; c++) CleanBinCornerDFS(res, vis, 0, c);
            for (int c = 0; c < res.Cols; c++) CleanBinCornerDFS(res, vis, res.Rows - 1, c);
            return res;
        }

        public Mat ToReverse(Bitmap bitmap)
        {
            return ToReverse(bitmap.ToOpenCvMat());
        }

        public Mat ToReverse(Mat mat)
        {
            return (~mat).ToMat();
        }

        public MatchImageResult MatchImage(Mat source, Mat search)
        {
            return MatchImage(source, search, 0.7);
        }

        public MatchImageResult MatchImage(Mat source, Mat search, double threshold)
        {
            var res = new Mat();
            //var c1 = source.Channels();
            //var c2 = search.Channels();
            DisplayImage("source", source);
            DisplayImage("search", search);
            Cv2.MatchTemplate(source, search, res, TemplateMatchModes.CCoeffNormed);
            double minVal, maxVal;
            CvPoint minLoc, maxLoc;
            Cv2.MinMaxLoc(res, out minVal, out maxVal, out minLoc, out maxLoc);
            //LogTools.GetInstance().Info($"maxVal = {maxVal}, threshold = {threshold}");
            if (maxVal < threshold)
            {
                return new MatchImageResult()
                {
                    Success = false,
                };
            }
            Cv2.Circle(source, maxLoc.X + search.Width / 2, maxLoc.Y + search.Height / 2, 25, Scalar.Red);
            DisplayImage("ImageMatch", source);
            return new MatchImageResult()
            {
                Success = true,
                MatchedRect = new RECT()
                {
                    x1 = maxLoc.X,
                    y1 = maxLoc.Y,
                    x2 = maxLoc.X + search.Width,
                    y2 = maxLoc.Y + search.Height,
                }
            };
        }


    }

    struct MatchImageResult
    {
        public bool Success;
        public RECT MatchedRect;
    }
}
