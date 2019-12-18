using Lmi3d.GoSdk;
using Lmi3d.GoSdk.Messages;
using Lmi3d.Zen;
using Lmi3d.Zen.Io;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Collections;
using ABB.Robotics.Math;
using LmiColorMap;
using System.Drawing.Imaging;

namespace LmiScanner
{
    public struct Point
    {
        public double X;
        public double Y;
        public double Z;
    }
    public class DataContext
    {
        public double xResolution;
        public double yResolution;
        public double zResolution;
        public double xOffset;
        public double yOffset;
        public double zOffset;
    }
    public struct ProfilePoint
    {
        public double x;
        public double y;
        public double z;
    }
    public struct GoPoints
    {
        public Int16 x;
        public Int16 y;
    }
    public struct point
    {
        public double z;
    }
    public class LmiScanner
    {
        private static LmiScanner obj = null;
        private GoSystem system = null;
        private GoSensor sensor = null;
        private const string strFileSave = "model.txt";
        public const double pixelPitch = 0.8;
        private double length = 0;
        Point mPoint = new Point();
        public List<Vector4> lst_ImagSur = new List<Vector4>();          // for left beam
        /// <summary>
        /// 获取了数据
        /// </summary>
        public Action<Bitmap> ActGetBit = null;
        LmiScanner() {
            KApiLib.Construct();
            GoSdkLib.Construct();
        }
        ~LmiScanner() {
            Dispose();
        }
        /// <summary>
        /// 单实例
        /// </summary>
        /// <returns></returns>
        public static LmiScanner intance() {
            if (obj == null) {
                obj = new LmiScanner();
            }
            return obj;
        }
        /// <summary>
        /// 使用IP地址初始化相机
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public bool InitCamera(string ip) {
            try
            {
                Dispose();
                system = new GoSystem();
                KIpAddress ipAddress = KIpAddress.Parse(ip);
                //Console.WriteLine(ipAddress.ToString());
                sensor = system.FindSensorByIpAddress(ipAddress);
                sensor.Connect();
                Console.WriteLine("InitCamera :" + ip);
                return true;
            }
            catch (Exception ex) {
                Console.WriteLine("InitCamera Error:" + ex.Message);
                return false;
            }
        }
        /// <summary>
        /// 使用序列号初始化相机
        /// </summary>
        /// <param name="serial"></param>
        /// <returns></returns>
        public bool InitCamera(uint serial)
        {
            try
            {
                Dispose();
                system = new GoSystem();
                sensor = system.FindSensorById(serial);
                sensor.Connect();
                Console.WriteLine("InitCamera :" + serial);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("InitCamera Error:" + ex.Message);
                return false;
            }
        }
        /// <summary>
        /// 析构相机
        /// </summary>
        public void Dispose() {
            try
            {
                if (system != null) {
                    StopListen();
                    system.Dispose();
                    system = null;
                    sensor = null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("DisposeCamera Error:" + ex.Message);
            }
        }
        /// <summary>
        /// 开始监听拍照数据
        /// </summary>
        public void StartListen() {
            if (sensor.IsConnected()) {
                system.EnableData(true);
                system.SetDataHandler(onData);
                system.Start();
            }
        }
        /// <summary>
        /// 停止监听
        /// </summary>
        public void StopListen() {
            try
            {
                system.Stop();
            }
            catch (Exception ex) {
                Console.WriteLine("StopListen Error:" + ex.Message);
            }
        }
        /// <summary>
        /// 使用指定Job
        /// </summary>
        /// <param name="job"></param>
        public void UseJob(string job) {
            try {
                sensor.CopyFile(job, "_live.job");
            } catch (Exception ex) {
                Console.WriteLine("UseJob Error:" + ex.Message);
            }
        }
        /// <summary>
        /// 软触发拍照
        /// </summary>
        public void SoftTrigger() {
            try
            {
                sensor.Trigger();
            }
            catch (Exception ex) {
                Console.WriteLine("SoftTrigger Error:" + ex.Message);
            }
        }
        /// <summary>
        /// 监听回调函数
        /// </summary>
        /// <param name="data"></param>
        public void onData(KObject data)
        {
            if (File.Exists(strFileSave)) File.Delete(strFileSave);
            bool length_incr = true;
            lst_ImagSur.Clear();
            GoDataSet dataSet = (GoDataSet)data;
            DataContext context = new DataContext();
            for (UInt32 i = 0; i < dataSet.Count; ++i)
            {
                GoDataMsg dataObj = (GoDataMsg)dataSet.Get(i);
                switch (dataObj.MessageType)
                {
                    case GoDataMessageType.Stamp:
                        {
                            Console.WriteLine("GoDataMessageType.Stamp");
                            //GoStampMsg stampMsg = (GoStampMsg)dataObj;
                            //for (UInt32 j = 0; j < stampMsg.Count; j++)
                            //{
                            //    GoStamp stamp = stampMsg.Get(j);
                            //    Console.WriteLine("Frame Index = {0}", stamp.FrameIndex);
                            //    Console.WriteLine("Time Stamp = {0}", stamp.Timestamp);
                            //    Console.WriteLine("Encoder Value = {0}", stamp.Encoder);
                            //}
                        }
                        break;
                    case GoDataMessageType.UniformSurface://case GoDataMessageType.Surface:
                        {
                            Console.WriteLine("GoDataMessageType.UniformSurface");
                            GoUniformSurfaceMsg surfaceMsg = (GoUniformSurfaceMsg)dataObj;
                            long width = surfaceMsg.Width;
                            long height = surfaceMsg.Length;
                            long bufferSize = width * height;
                            IntPtr bufferPointer = surfaceMsg.Data;

                            short[] ranges = new short[bufferSize];
                            Marshal.Copy(bufferPointer, ranges, 0, ranges.Length);

                            context.xResolution = (double)surfaceMsg.XResolution / 1000000.0;
                            context.zResolution = (double)surfaceMsg.ZResolution / 1000000.0;
                            context.yResolution = (double)surfaceMsg.YResolution / 1000000.0;
                            context.yOffset = (double)surfaceMsg.YOffset / 1000.0;
                            context.xOffset = (double)surfaceMsg.XOffset / 1000.0;
                            context.zOffset = (double)surfaceMsg.ZOffset / 1000.0;

                            double phy_x;
                            double phy_y;
                            double phy_z;

                            FileStream fs = new FileStream(strFileSave, FileMode.Create);
                            StreamWriter sw = new StreamWriter(fs);
                            
                            for (int m = 0; m < height; m++)
                            {
                                for (int j = 0; j < width; j++)
                                {
                                    phy_z = ranges[m * width + j] * context.zResolution + context.zOffset;
                                    if (true)     //   /*phy_z > 20*/   这个过滤阈值根据实际情况选取
                                    {
                                        phy_x = j * context.xResolution + context.xOffset;
                                        phy_y = m * context.yResolution + context.yOffset;

                                        string strWrite = string.Format("{0} {1} {2}", phy_x, phy_y, phy_z);
                                        sw.WriteLine(strWrite);
                                        lst_ImagSur.Add(new Vector4((float)phy_x, (float)phy_y, (float)phy_z, 1.0f));
                                    }
                                }
                            }
                            sw.Flush();
                            //关闭流
                            sw.Close();
                            fs.Close();
                        }
                        break;
                    case GoDataMessageType.Measurement:
                        {
                            Console.WriteLine("GoDataMessageType.Measurement");
                            //GoMeasurementMsg measurementMsg = (GoMeasurementMsg)dataObj;
                            //for (UInt32 k = 0; k < measurementMsg.Count; ++k)
                            //{
                            //    GoMeasurementData measurementData = measurementMsg.Get(k);
                            //    mPoint.X = measurementData.Value;
                            //    mPoint.Y = measurementData.Value;
                            //    mPoint.Z = measurementData.Value;
                            //}
                        }
                        break;
                    case GoDataMessageType.ProfilePointCloud://case GoDataMessageType.Profile:
                        {
                            Console.WriteLine("GoDataMessageType.Profile");
                            //StreamWriter write = new StreamWriter(strFileSave, true);
                            //GoProfilePointCloudMsg profileMsg = (GoProfilePointCloudMsg)dataObj;
                            //Console.WriteLine("  Profile Message batch count: {0}", profileMsg.Count);
                            //for (UInt32 k = 0; k < profileMsg.Count; ++k)
                            //{
                            //    //int validPointCount = 0;
                            //    long profilePointCount = profileMsg.Width;
                            //    Console.WriteLine("  Item[{0}]: Profile data ({1} points)", i, profileMsg.Width);
                            //    context.xResolution = (profileMsg.XResolution / 1000000.0);
                            //    context.zResolution = profileMsg.ZResolution / 1000000.0;
                            //    context.xOffset = profileMsg.XOffset / 1000.0;
                            //    context.zOffset = profileMsg.ZOffset / 1000.0;
                            //    GoPoints[] points = new GoPoints[profilePointCount];
                            //    //point[] point111 = new point[profilePointCount];
                            //    ProfilePoint[] profileBuffer = new ProfilePoint[profilePointCount];
                            //    int structSize = Marshal.SizeOf(typeof(GoPoints));
                            //    IntPtr pointsPtr = profileMsg.Data;
                            //    for (UInt32 array = 0; array < profilePointCount; ++array)
                            //    {
                            //        IntPtr incPtr = new IntPtr(pointsPtr.ToInt64() + array * structSize);
                            //        points[array] = (GoPoints)Marshal.PtrToStructure(incPtr, typeof(GoPoints));

                            //        double real_x = (context.xOffset + context.xResolution * points[array].x);
                            //        double real_z = (context.zOffset + context.zResolution * points[array].y);

                            //        if (length_incr == true)
                            //        {
                            //            length += 1;//ReadIniSettings.ReadIni.objIniValue.iniScanner.step;
                            //            length_incr = false;
                            //        }

                            //        if (/*(real_z > 15 && real_x > -500.0 && real_z < 400*/true)
                            //        //if (real_z > 5.0  && real_z < 25 && length >100 && length < 2000)
                            //        {
                            //            write.WriteLine(real_x + " " + length + " " + real_z);
                            //        }
                            //    }
                            //    write.Flush();
                            //}
                            //write.Close();
                        }
                        break;
                    case GoDataMessageType.ProfileIntensity:
                        {
                            Console.WriteLine("GoDataMessageType.ProfileIntensity");
                            //GoProfileIntensityMsg profileMsg = (GoProfileIntensityMsg)dataObj;
                            //Console.WriteLine("  Profile Intensity Message batch count: {0}", profileMsg.Count);
                            //for (UInt32 k = 0; k < profileMsg.Count; ++k)
                            //{
                            //    byte[] intensity = new byte[profileMsg.Width];
                            //    IntPtr intensityPtr = profileMsg.Data;
                            //    Marshal.Copy(intensityPtr, intensity, 0, intensity.Length);
                            //}
                        }
                        break;
                    case GoDataMessageType.Alignment:
                        Console.WriteLine("GoDataMessageType.Alignment");
                        break;
                    case GoDataMessageType.BoundingBoxMatch:
                        Console.WriteLine("GoDataMessageType.BoundingBoxMatch");
                        break;
                    case GoDataMessageType.EdgeMatch:
                        Console.WriteLine("GoDataMessageType.EdgeMatch");
                        break;
                    case GoDataMessageType.EllipseMatch:
                        Console.WriteLine("GoDataMessageType.EllipseMatch");
                        break;
                    case GoDataMessageType.Event:
                        Console.WriteLine("GoDataMessageType.Event");
                        break;
                    case GoDataMessageType.ExposureCal:
                        Console.WriteLine("GoDataMessageType.ExposureCal");
                        break;
                    case GoDataMessageType.Generic:
                        Console.WriteLine("GoDataMessageType.Generic");
                        break;
                    case GoDataMessageType.Health:
                        Console.WriteLine("GoDataMessageType.Health");
                        break;
                    case GoDataMessageType.Range:
                        Console.WriteLine("GoDataMessageType.Range");
                        break;
                    case GoDataMessageType.RangeIntensity:
                        Console.WriteLine("GoDataMessageType.RangeIntensity");
                        break;
                    case GoDataMessageType.ResampledProfile://case GoDataMessageType.UniformProfile: 
                        Console.WriteLine("GoDataMessageType.ResampledProfile");
                        break;
                    case GoDataMessageType.Section:
                        Console.WriteLine("GoDataMessageType.Section");
                        break;
                    case GoDataMessageType.SectionIntensity:
                        Console.WriteLine("GoDataMessageType.SectionIntensity");
                        break;
                    case GoDataMessageType.SurfaceIntensity:
                        Console.WriteLine("GoDataMessageType.SurfaceIntensity");
                        break;
                    case GoDataMessageType.SurfacePointCloud:
                        Console.WriteLine("GoDataMessageType.SurfacePointCloud");
                        break;
                    case GoDataMessageType.Tracheid:
                        Console.WriteLine("GoDataMessageType.Tracheid");
                        break;
                    case GoDataMessageType.Unknown:
                        Console.WriteLine("GoDataMessageType.Unknown");
                        break;
                    case GoDataMessageType.Video:
                        Console.WriteLine("GoDataMessageType.Video");
                        break;
                }
            }
            dataSet.Dispose();
            if (ActGetBit != null && lst_ImagSur.Count > 0)
            {
                int imgWidth, imgHeight;
                int rowNum = lst_ImagSur.Count;
                double[] xVal = new double[rowNum];
                double[] yVal = new double[rowNum];
                ushort[] zVal = new ushort[rowNum];

                for (int m = 0; m < rowNum; m++)
                {
                    xVal[m] = Math.Round(lst_ImagSur[m].x / pixelPitch);
                    yVal[m] = Math.Round(lst_ImagSur[m].y / pixelPitch);
                    zVal[m] = (ushort)lst_ImagSur[m].z;
                }
                ArrayList list_x = new ArrayList(xVal);
                list_x.Sort();
                if (list_x.Count == 0)
                {
                    double minVal_x = 0;        // Convert.ToInt32(list[0]);
                    double maxVal_x = 0;
                    ActGetBit(null);
                }
                else
                {
                    double minVal_x = Convert.ToDouble(list_x[0]); // Convert.ToInt32(list[0]);
                    double maxVal_x = Convert.ToDouble(list_x[list_x.Count - 1]);

                    ArrayList list_y = new ArrayList(yVal);
                    list_y.Sort();
                    double minVal_y = Convert.ToDouble(list_y[0]); // Convert.ToInt32(list[0]);
                    double maxVal_y = Convert.ToDouble(list_y[list_y.Count - 1]);

                    ArrayList list_z = new ArrayList(zVal);
                    list_z.Sort();
                    double minVal_z = Convert.ToDouble(list_z[0]); // Convert.ToInt32(list[0]);
                    double maxVal_z = Convert.ToDouble(list_z[list_z.Count - 1]);

                    imgHeight = Convert.ToInt32(Math.Round(maxVal_x - minVal_x)) + 1;
                    imgWidth = Convert.ToInt32(Math.Round(maxVal_y - minVal_y)) + 1;

                    double scale = (double)imgHeight / imgWidth;
                    double[,] imgTemp = new double[imgHeight, imgWidth];

                    for (int m = 0; m < rowNum; m++)
                    {
                        xVal[m] = xVal[m] - minVal_x;
                        yVal[m] = yVal[m] - minVal_y;
                    }
                    for (int n = 0; n < rowNum; n++)
                    {
                        imgTemp[Convert.ToInt32(xVal[n]), Convert.ToInt32(yVal[n])] = zVal[n];
                    }

                    // 将imgTemp转成一维图像
                    ushort[] ZValues = new ushort[imgHeight * imgWidth];
                    for (int i = 0; i < ZValues.Length; i++)
                    {
                        ZValues[i] = (ushort)imgTemp[i / imgWidth, i % imgWidth];
                    }

                    ColorMaps MapColor = new ColorMaps();
                    ushort minValue = 0, maxValue = 0;
                    MapColor.FindMinMaxForColor(ZValues, (UInt32)(imgHeight * imgWidth), 0, ref minValue, ref maxValue);
                    Color[] colors = new Color[imgHeight * imgWidth];
                    MapColor.ToColors(ZValues, minValue, maxValue, 0, ref colors, (UInt32)(imgHeight * imgWidth));

                    Bitmap Bimage = new Bitmap(imgWidth, imgHeight);
                    BitmapData bmdata = Bimage.LockBits(new Rectangle(0, 0, imgWidth, imgHeight), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                    //IntPtr intptr = bmdata.Scan0;
                    byte[] Pixlemaps = new byte[bmdata.Stride * bmdata.Height];
                    // int offset = bmdata.Stride - bmdata.Width * 3;

                    unsafe
                    {
                        byte* pp = (byte*)(void*)bmdata.Scan0;

                        for (int k = 0; k < bmdata.Height; k++)
                        {
                            for (int m = 0; m < bmdata.Width; m++)
                            {
                                pp[0] = (byte)(colors[k * bmdata.Width + m].R);
                                pp[1] = (byte)(colors[k * bmdata.Width + m].G);
                                pp[2] = (byte)(colors[k * bmdata.Width + m].B);
                                pp += 3;
                            }
                            pp += bmdata.Stride - bmdata.Width * 3;
                        }
                    }
                    Bimage.UnlockBits(bmdata);
                    Bitmap aBM2 = Bimage.Clone(new RectangleF(0, 0, Bimage.Width, Bimage.Height), PixelFormat.Format24bppRgb);
                    ActGetBit(aBM2);
                    //pbxImage.Width = 150;
                    //pbxImage.Height = Convert.ToInt32(pbxImage.Width * scale);
                    //Bitmap imgShow_scale = new Bitmap(aBM2, pbxImage.Width, pbxImage.Height);
                    //pbxImage.Image = imgShow_scale;
                }
            }
        }
    }
}
