using OpenCvSharp;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Collections.Generic;
using RawPoint = System.Drawing.Point;

namespace PCRHelper
{

    /// <summary>
    /// 严格来说应该叫PCRState
    /// </summary>
    class MumuState
    {

        public static MumuState Create()
        {
            return new MumuState();
        }

        int width = 2160;
        int height = 1080;
        bool checkResolution = false;//分辨率
        bool checkTopBottomBounds = false;

        private Process process;
        private RECT rect;
        private RECT viewportRect;
        private ConfigMgr configMgr = ConfigMgr.GetInstance();
        private GraphicsTools graphicsTools = GraphicsTools.GetInstance();
        private OCRTools ocrTools = OCRTools.GetInstance();
        private LogTools logTools = LogTools.GetInstance();

        private MumuState()
        {

        }

        private bool UseCache
        {
            get
            {
                return false;
            }
        }

        public Process Process
        {
            get
            {
                if (!UseCache || process == null)
                {
                    process = Tools.GetInstance().GetMumuProcess();
                }
                return process;
            }
        }

        public RECT Rect
        {
            get
            {
                if (!UseCache || rect.Width == 0)
                {
                    rect = Tools.GetInstance().GetWindowRect(Process);
                }
                return rect;
            }
        }

        public RECT ViewportRect
        {
            get
            {
                //if (!UseCache || TopY == 0 || BottomY == 0)
                //{
                //    var capture = DoRealTimeCaptureAndAnalyze();
                //    if (checkResolution)
                //    {
                //        CheckResolution(capture);
                //    }
                //    var crect = Rect;
                //    viewportRect = new RECT() {
                //        x1 = rect.x1,
                //        y1 = rect.y1 + (int)TopY,
                //        x2 = rect.x2,
                //        y2 = rect.y1 + (int)BottomY,
                //    };
                //}
                if (!UseCache || viewportRect.Width == 0)
                {
                    var proc = Tools.GetInstance().GetMumuProcess();
                    var hWnd = Win32ApiHelper.FindWindowEx(proc.MainWindowHandle, IntPtr.Zero, null, null);
                    Win32ApiHelper.GetWindowRect(hWnd, out viewportRect);
                    if (viewportRect.Width <= 15 || viewportRect.Height <= 15)
                    {
                        throw new Exception("Mumu模拟器窗口尺寸不合法");
                    }
                    var title = Win32ApiHelper.GetWindowTitle(hWnd);
                    if (!title.Contains("NemuPlayer"))
                    {
                        throw new Exception("无法获取Mumu模拟器窗口"); 
                    }
                }
                return viewportRect;
            }
        }

        public void CheckResolution(Bitmap capture)
        {
            var wdivhCap = 1.0 * capture.Height / capture.Width;
            var wdivh = 0.5625;
            var validWdivhOff = 0.05;
            if (Math.Abs(wdivhCap - wdivh) > validWdivhOff)
            {
                throw new BreakException($"请使用{width}*{height}的分辨率");
            }
        }

        public Bitmap DoCapture(RECT rect)
        {
            var capture = Tools.GetInstance().CaptureWindow(rect);
            GraphicsTools.GetInstance().DisplayImage("DoCapture", capture);
            return capture;
        }

        /// <summary>
        /// 包括标题栏
        /// </summary>
        /// <returns></returns>
        public Bitmap DoRealTimeCapture()
        {
            var rect = Rect;
            var capture = DoCapture(rect);
            return capture;
        }

        public Bitmap DoRealTimeCaptureAndAnalyze()
        {
            var capture = DoRealTimeCapture();
            GetViewportTopBottomBounds(capture);
            return capture;
        }

        public Bitmap DoRealTimeViewportCapture()
        {
            var viewportRect = ViewportRect;
            var capture = DoCapture(viewportRect);
            return capture;
        }

        float topY;
        public float TopY
        {
            get { return topY; }
            private set
            {
                topY = value;
            }
        }

        float bottomY;
        public float BottomY
        {
            get { return bottomY; }
            private set
            {
                bottomY = value;
            }
        }

        public void GetViewportTopBottomBounds(Bitmap bitmap)
        {
            var graphicsTools = GraphicsTools.GetInstance();
            var mat = bitmap.ToOpenCvMat();
            var bin = graphicsTools.ToGrayBinary(mat, 50);

            var width = bitmap.Width;

            if (configMgr.FixedViewportTopBottomY)
            {
                TopY = configMgr.FixedViewportTopY;
                BottomY = configMgr.FixedViewportBottomY;
            }
            else
            {
                var threshold = (int)(255 * bin.Rows * 0.5);
                var preSum = 0;
                for (int r = 0; r < bin.Rows; r++)
                {
                    var sum = 0;
                    for (int c = 0; c < bin.Cols; c++)
                    {
                        var clr = bin.GetPixel(r, c);
                        sum += clr.R;
                    }

                    var diff = Math.Abs(sum - preSum);
                    if (diff > threshold)
                    {
                        TopY = r;
                        break;
                    }
                    preSum = sum;
                }
                preSum = 0;
                for (int r = bin.Rows - 1; r >= 0; r--)
                {
                    var sum = 0;
                    for (int c = 0; c < bin.Cols; c++)
                    {
                        var clr = bin.GetPixel(r, c);
                        sum += clr.R;
                    }

                    var diff = Math.Abs(sum - preSum);
                    if (diff > threshold)
                    {
                        BottomY = r;
                        break;
                    }
                    preSum = sum;
                }
            }

            if (checkTopBottomBounds)
            {
                var height = 1.0 * width * this.height / this.width;

                var validTopY = 36;
                var validTopYOff = 10;

                var validBottomY = height + 36;
                var validBottomYOff = 10;

                var topYOff = Math.Abs(TopY - validTopY);
                if (topYOff > validTopYOff)
                {
                    throw new NoTrackTraceException("无法检测到Mumu模拟器上边界");
                }
                var bottomYOff = Math.Abs(BottomY - validBottomY);
                if (bottomYOff > validBottomYOff)
                {
                    throw new NoTrackTraceException("无法检测到Mumu模拟器下边界");
                }
            }
        }

        public Bitmap GetCaptureRect(Bitmap capture, RECT captureRect, Vec4f rectRate)
        {
            var mat = capture.ToOpenCvMat();
            return GetCaptureRect(mat, captureRect, rectRate).ToRawBitmap();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="capture">完整的截图捕获</param>
        /// <param name="captureRect">完整的截图所对应的矩形 相对于屏幕</param>
        /// <param name="rectRate">子矩形的比率 相对于capture</param>
        /// <returns></returns>
        public Mat GetCaptureRect(Mat mat, RECT captureRect, Vec4f rectRate)
        {
            var relativeRect = captureRect.GetChildRectByRate(rectRate);
            var childMat = GraphicsTools.GetInstance().GetChildMatByRECT(mat, relativeRect);
            return childMat;
        }

        /// <summary>
        /// 获取到的是模拟器中的坐标
        /// </summary>
        /// <param name="rect">这个参数暂时无用 opencv坐标</param>
        /// <param name="pointRate"></param>
        /// <returns></returns>
        public RawPoint GetEmulatorPoint(RECT viewportRect, Vec2f pointRate)
        {
            var wid = width;
            var hei = height;
            return new RawPoint((int)(wid * pointRate.Item0), (int)(hei * pointRate.Item1));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="viewportRect">opencv坐标</param>
        /// <param name="point">opencv坐标 相对于viewport左上角</param>
        /// <returns></returns>
        public RawPoint GetEmulatorPoint(RECT viewportRect, RawPoint point)
        {
            var pointRate = new Vec2f(1.0f * point.X / viewportRect.Width, 1.0f * point.Y / viewportRect.Height);
            return GetEmulatorPoint(viewportRect, pointRate);
        }

        public void DoClick(RawPoint point)
        {
            AdbTools.GetInstance().DoTap(point);
        }

        public void DoClick(RECT viewportRect, Vec2f pointRate)
        {
            var point = GetEmulatorPoint(viewportRect, pointRate);
            AdbTools.GetInstance().DoTap(point);
        }

        public void DoDrag(RECT viewportRect, Vec2f startPointRate, Vec2f endPointRate, int dragDuration)
        {
            var startPoint = GetEmulatorPoint(viewportRect, startPointRate);
            var endPoint = GetEmulatorPoint(viewportRect, endPointRate);
            AdbTools.GetInstance().DoDrag(startPoint, endPoint, dragDuration);
        }

        Vec2f[] mainlandArenaPlayerPointRateArr = new Vec2f[]
        {
            new Vec2f(0.6669f, 0.3312f),
            new Vec2f(0.6653f, 0.5357f),
            new Vec2f(0.6448f, 0.7433f),
        };

        public Vec2f GetArenaPlayerPointRate(int index)
        {
            if (configMgr.PCRRegion == PCRRegion.Mainland)
            {
                return mainlandArenaPlayerPointRateArr[index];
            }
            else if (configMgr.PCRRegion == PCRRegion.Taiwan)
            {
                return mainlandArenaPlayerPointRateArr[index];
            }
            else if (configMgr.PCRRegion == PCRRegion.Japan)
            {
                return mainlandArenaPlayerPointRateArr[index];
            }
            throw new BreakException("GetArenaPlayerPointRateArr" + index);
        }

        public void ClickArenaPlayer(RECT viewportRect, int index)
        {
            var arenaPlayerPointRate = GetArenaPlayerPointRate(index);
            var point = GetEmulatorPoint(viewportRect, arenaPlayerPointRate);
            DoClick(point);
        }

        Vec2f arenaRefreshPointRate = new Vec2f(0.8663f, 0.1680f);

        public Vec2f GetArenaRefreshPointRate()
        {
            return arenaRefreshPointRate;
        }

        public void ClickArenaRefresh(RECT viewportRect)
        {
            var arenaRefreshPointRate = GetArenaRefreshPointRate();
            var point = GetEmulatorPoint(viewportRect, arenaRefreshPointRate);
            DoClick(point);
        }


        Vec4f[] mainlandArenaPlayerNameRectRateArr = new Vec4f[]
        {
            new Vec4f(0.5135f, 0.2511f, 0.6608f, 0.2823f),
            new Vec4f(0.5135f, 0.4638f, 0.6608f, 0.4980f),
            new Vec4f(0.5135f, 0.6766f, 0.6608f, 0.7116f),
        };

        Vec4f[] taiwanArenaPlayerNameRectRateArr = new Vec4f[]
        {
            new Vec4f(0.4934f, 0.2755f, 0.6594f, 0.3061f),
            new Vec4f(0.4934f, 0.4883f, 0.6594f, 0.5204f),
            new Vec4f(0.4934f, 0.7041f, 0.6594f, 0.7347f),
        };

        public Vec4f GetArenaPlayerNameRectRate(int index)
        {
            if (configMgr.PCRRegion == PCRRegion.Mainland)
            {
                return mainlandArenaPlayerNameRectRateArr[index];
            }
            else if (configMgr.PCRRegion == PCRRegion.Taiwan)
            {
                return taiwanArenaPlayerNameRectRateArr[index];
            }
            throw new BreakException("GetArenaPlayerNamePointRateArr" + index);
        }

        public Bitmap GetArenaPlayerNameRectCapture(Bitmap viewportCapture, RECT viewportRect, int index)
        {
            var arenaPlayerNameRectRate = GetArenaPlayerNameRectRate(index);
            return GetCaptureRect(viewportCapture, viewportRect, arenaPlayerNameRectRate);
        }

        public Mat GetArenaPlayerNameRectCapture(Mat viewportMat, RECT viewportRect, int index)
        {
            var arenaPlayerNameRectRate = GetArenaPlayerNameRectRate(index);
            return GetCaptureRect(viewportMat, viewportRect, arenaPlayerNameRectRate);
        }

        public string DoArenaPlayerNameOCR(Bitmap viewportCapture, RECT viewportRect, int index)
        {
            var capture = GetArenaPlayerNameRectCapture(viewportCapture, viewportRect, index);
            graphicsTools.DisplayImage("NameToOCR" + index, capture);
            var name = ocrTools.ToGrayAndOCR(capture);
            return name;
        }

        public string DoArenaPlayerNameOCR(Mat viewportMat, RECT viewportRect, int index)
        {
            var capture = GetArenaPlayerNameRectCapture(viewportMat, viewportRect, index);
            graphicsTools.DisplayImage("NameToOCR" + index, capture);
            var name = ocrTools.ToGrayAndOCR(capture);
            return name;
        }

        Vec4f[] mainlandArenaPlayerRankRectRateArr = new Vec4f[]
        {
            new Vec4f(0.7450f, 0.2227f, 0.8258f, 0.2582f),
            new Vec4f(0.7450f, 0.4369f, 0.8258f, 0.4723f),
            new Vec4f(0.7450f, 0.6496f, 0.8258f, 0.6879f),
        };

        public Vec4f GetArenaPlayerRankRectRate(int index)
        {
            if (configMgr.PCRRegion == PCRRegion.Mainland)
            {
                return mainlandArenaPlayerRankRectRateArr[index];
            }
            else if (configMgr.PCRRegion == PCRRegion.Taiwan)
            {
                return mainlandArenaPlayerRankRectRateArr[index];
            }
            throw new BreakException("GetArenaPlayerRankPointRateArr" + index);
        }

        public Bitmap GetArenaPlayerRankRectCapture(Bitmap viewportCapture, RECT viewportRect, int index)
        {
            var arenaPlayerRankRectRate = GetArenaPlayerRankRectRate(index);
            return GetCaptureRect(viewportCapture, viewportRect, arenaPlayerRankRectRate);
        }

        public Mat GetArenaPlayerRankRectCapture(Mat viewportMat, RECT viewportRect, int index)
        {
            var arenaPlayerRankRectRate = GetArenaPlayerRankRectRate(index);
            return GetCaptureRect(viewportMat, viewportRect, arenaPlayerRankRectRate);
        }

        public string DoArenaPlayerRankOCR(Bitmap viewportCapture, RECT viewportRect, int index)
        {
            var capture = GetArenaPlayerRankRectCapture(viewportCapture, viewportRect, index);
            var gray = graphicsTools.ToGray(capture);
            var reverse = graphicsTools.ToReverse(gray);
            graphicsTools.DisplayImage("RankReverse" + index, reverse);
            var bin = graphicsTools.ToBinaryPlus(reverse, 90);
            bin = graphicsTools.CleanBinCorner(bin);
            graphicsTools.DisplayImage("RankToOCR" + index, bin);
            var rank = ocrTools.OCR(bin);
            return rank;
        }

        public string DoArenaPlayerRankOCR(Mat viewportMat, RECT viewportRect, int index)
        {
            var capture = GetArenaPlayerRankRectCapture(viewportMat, viewportRect, index);
            var gray = graphicsTools.ToGray(capture);
            var reverse = graphicsTools.ToReverse(gray);
            graphicsTools.DisplayImage("RankReverse" + index, reverse);
            var bin = graphicsTools.ToBinaryPlus(reverse, 90);
            bin = graphicsTools.CleanBinCorner(bin);
            graphicsTools.DisplayImage("RankToOCR" + index, bin);
            var rank = ocrTools.OCR(bin);
            return rank;
        }

        Vec2f actStageExchangePointRate = new Vec2f(0.5288f, 0.7913f);

        public Vec2f GetActStageExchangePointRate()
        {
            return actStageExchangePointRate;
        }

        public void ClickActStageExchange(RECT viewportRect)
        {
            var actStageExchangePointRateI = GetActStageExchangePointRate();
            var point = GetEmulatorPoint(viewportRect, actStageExchangePointRateI);
            DoClick(point);
        }


        Vec2f backPointRate = new Vec2f(0.0317f, 0.0526f);

        public Vec2f GetBackPointRate()
        {
            return backPointRate;
        }

        public void ClickBack(RECT viewportRect)
        {
            var backPointRateI = GetBackPointRate();
            var point = GetEmulatorPoint(viewportRect, backPointRateI);
            DoClick(point);
        }


        Vec2f[] tabPointRateArr = new Vec2f[]
        {
            new Vec2f(0.1379f, 0.9465f),
            new Vec2f(0.2552f, 0.9516f),
            new Vec2f(0.3716f, 0.9585f),
            new Vec2f(0.5000f, 0.9482f),
            new Vec2f(0.6397f, 0.9534f),
            new Vec2f(0.7491f, 0.9430f),
            new Vec2f(0.8629f, 0.9534f),
        };

        public Vec2f GetTabPointRate(PCRTab pcrTab)
        {
            switch(pcrTab)
            {
                case PCRTab.Mainpage: return tabPointRateArr[0];
                case PCRTab.Character: return tabPointRateArr[1];
                case PCRTab.Story: return tabPointRateArr[2];
                case PCRTab.Battle: return tabPointRateArr[3];
                case PCRTab.Guildhouse: return tabPointRateArr[4];
                case PCRTab.Pickup: return tabPointRateArr[5];
                case PCRTab.Menu: return tabPointRateArr[6];
            }
            return tabPointRateArr[0];
        }

        public void ClickTab(RECT viewportRect, PCRTab pcrTab)
        {
            var tabPointRate = GetTabPointRate(pcrTab);
            var point = GetEmulatorPoint(viewportRect, tabPointRate);
            DoClick(point);
        }

        Vec2f[] battleEntrancePointRateArr = new Vec2f[]
        {
            new Vec2f(0.6328f, 0.3903f),
            new Vec2f(0.7974f, 0.2591f),
            new Vec2f(0.9207f, 0.2591f),
            new Vec2f(0.7931f, 0.5095f),
            new Vec2f(0.9233f, 0.5199f),
            new Vec2f(0.6500f, 0.7582f),
            new Vec2f(0.8767f, 0.7547f),
        };

        public Vec2f GetBattleEntrancePointRate(PCRBattleMode mode)
        {
            switch (mode)
            {
                case PCRBattleMode.Mainline: return battleEntrancePointRateArr[0];
                case PCRBattleMode.Explore: return battleEntrancePointRateArr[1];
                case PCRBattleMode.Underground: return battleEntrancePointRateArr[2];
                case PCRBattleMode.Survey: return battleEntrancePointRateArr[3];
                case PCRBattleMode.Team: return battleEntrancePointRateArr[4];
                case PCRBattleMode.Arena: return battleEntrancePointRateArr[5];
                case PCRBattleMode.PrincessArena: return battleEntrancePointRateArr[6];
            }
            return battleEntrancePointRateArr[0];
        }

        public void ClickBattleEntrance(RECT viewportRect, PCRBattleMode mode)
        {
            var battleEntracePointRate = GetBattleEntrancePointRate(mode);
            var point = GetEmulatorPoint(viewportRect, battleEntracePointRate);
            DoClick(point);
        }

        Vec2f[] storyEntrancePointRateArr = new Vec2f[]
        {
            new Vec2f(0.7686f, 0.2522f),
            new Vec2f(0.6128f, 0.6414f),
            new Vec2f(0.7627f, 0.6472f),
            new Vec2f(0.9178f, 0.6443f),
        };

        public Vec2f GetStoryEntrancePointRate(PCRStory story)
        {
            var arr = storyEntrancePointRateArr;
            switch (story)
            {
                case PCRStory.Mainline: return arr[0];
                case PCRStory.Character: return arr[1];
                case PCRStory.Guild: return arr[2];
                case PCRStory.Extra: return arr[3];
            }
            return arr[0];
        }

        public void ClickStoryEntrance(RECT viewportRect, PCRStory story)
        {
            var storyEntracePointRate = GetStoryEntrancePointRate(story);
            var point = GetEmulatorPoint(viewportRect, storyEntracePointRate);
            DoClick(point);
        }

        Vec2f[] dataDownloadPointRateArr = new Vec2f[]
        {
            new Vec2f(0.5058f, 0.6822f),
            new Vec2f(0.6390f, 0.6880f),
        };

        public Vec2f GetDataDownloadPointRate(bool hasVoice)
        {
            var arr = dataDownloadPointRateArr;
            return hasVoice ? arr[1] : arr[0];
        }

        public void ClickDataDownloadButton(RECT viewportRect, bool hasVoice)
        {
            var dataDownloadPointRate = GetDataDownloadPointRate(hasVoice);
            var point = GetEmulatorPoint(viewportRect, dataDownloadPointRate);
            DoClick(point);
        }

        public Vec2f GetMenuButtonRectRate()
        {
            return new Vec2f(0.9105f, 0.0816f);
        }

        public void ClickMenuButton(RECT viewportRect)
        {
            var menuButtonPointRate = GetMenuButtonRectRate();
            var point = GetEmulatorPoint(viewportRect, menuButtonPointRate);
            DoClick(point);
        }

        public Vec2f GetSkipButtonRectRate()
        {
            return new Vec2f(0.8057f, 0.0831f);
        }

        public void ClickSkipButton(RECT viewportRect)
        {
            var skipButtonPointRate = GetSkipButtonRectRate();
            var point = GetEmulatorPoint(viewportRect, skipButtonPointRate);
            DoClick(point);
        }

        public Vec2f GetSkipConfirmButtonRectRate()
        {
            return new Vec2f(0.5990f, 0.6924f);
        }

        /// <summary>
        /// 注意这个按钮位置是不确定的
        /// </summary>
        /// <param name="viewportRect"></param>
        public void ClickSkipConfirmButton(RECT viewportRect)
        {
            var skipConfirmButtonPointRate = GetSkipConfirmButtonRectRate();
            var point = GetEmulatorPoint(viewportRect, skipConfirmButtonPointRate);
            DoClick(point);
        }

        Dictionary<PCRMainpageButton, Vec2f> mainpageButtonRectRateMap = new Dictionary<PCRMainpageButton, Vec2f>()
        {
            { PCRMainpageButton.Present, new Vec2f(0.9543f, 0.8012f) },
        };


        public Vec2f GetMainpageButtonRectRate(PCRMainpageButton buttonType)
        {
            return mainpageButtonRectRateMap[buttonType];
        }

        public void ClickMainpageButton(RECT viewportRect, PCRMainpageButton buttonType)
        {
            var pointRate = GetMainpageButtonRectRate(buttonType);
            var point = GetEmulatorPoint(viewportRect, pointRate);
            DoClick(point);
        }

        public bool IsMatchConnecting(Mat viewportMat, RECT viewportRect, out RECT matchedRect)
        {
            var childMat = viewportMat.GetChildMatByRectRate(new Vec4f(0.7899f, 0.0036f, 0.9810f, 0.1289f));
            var exMat = configMgr.GetPCRExImg("connecting.png", viewportMat, viewportRect);
            var matchRes = graphicsTools.MatchImage(childMat, exMat);
            matchedRect = matchRes.MatchedRect;
            return matchRes.Success;
        }

        public PCRStateStruct GetPCRState(Bitmap viewportCapture, RECT viewportRect)
        {
            return GetPCRState(viewportCapture.ToOpenCvMat(), viewportRect);
        }

        public PCRStateStruct GetPCRState(Mat viewportMat, RECT viewportRect)
        {
            RECT matchedRect;
            if (IsMatchConnecting(viewportMat, viewportRect, out matchedRect))
            {
                return new PCRStateStruct() { State = PCRState.Connecting, Param0 = matchedRect };
            }
            return new PCRStateStruct()
            {
                State = PCRState.Unknown,
            };
        }
    }

    struct PCRStateStruct
    {
        public PCRState State;
        public object Param0;
    }

    enum PCRState
    {
        Unknown,
        Connecting,
        Loading,
    }

    enum PCRTab
    {
        Mainpage,
        Character,
        Story,
        Battle,
        Guildhouse,
        Pickup,
        Menu,
    }

    enum PCRBattleMode
    {
        Mainline,
        Explore,
        Underground,
        Survey,
        Team,
        Arena,
        PrincessArena,
    }

    enum PCRStory
    {
        Mainline,
        Character,
        Guild,
        Extra,
    }

    enum PCRMainpageButton
    {
        Present,
    }
}
