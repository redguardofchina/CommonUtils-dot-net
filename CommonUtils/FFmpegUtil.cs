using System.Diagnostics;
using System.Threading;

namespace CommonUtils
{
    public class FFmpegUtil
    {
        /// <summary>
        /// 定义一个测试rtsp
        /// </summary>
        public const string TestRtsp = "rtsp://admin:tzadmin@192.168.1.128:554";

        /// <summary>
        /// 定义FFmpeg文件路径
        /// </summary>
        public const string FFmpegPhysicalPath = "D:\\ffmpeg.exe";

        /// <summary>
        /// 执行编码器
        /// </summary>
        public static bool ExcuteFFmpeg(string exePhysicalPath, string arguments)
        {
            try
            {
                Thread th = new Thread(new ThreadStart(delegate ()
                {
                    Process process = new Process();
                    process.StartInfo.FileName = exePhysicalPath;
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.CreateNoWindow = true;
                    process.StartInfo.Arguments = arguments;
                    process.Start();
                    process.WaitForExit();
                    process.Close();
                    process.Dispose();
                }));
                th.Start();
                return true;
            }
            catch
            { return false; }
        }

        /// <summary>
        /// 根据毫秒获取标准FFmpeg时间型为00:00:00.000
        /// </summary>
        public static string GetFFmpegTimeByMilliscond(int totalMs)
        {
            int totalSecond, TotalMinitue, TotalHour;
            string lastMs, lastSecond, lastMinitue, lastHour;
            totalSecond = totalMs / 1000;
            TotalMinitue = totalSecond / 60;
            TotalHour = TotalMinitue / 60;
            lastMs = (totalMs % 1000).ToString();
            lastSecond = (totalSecond % 60).ToString();
            lastMinitue = (TotalMinitue % 60).ToString();
            lastHour = TotalHour.ToString();
            return StringUtil_.AggrZero(lastHour, 2) + ":" + StringUtil_.AggrZero(lastMinitue, 2) + ":" + StringUtil_.AggrZero(lastSecond, 2) + "." + StringUtil_.AggrZero(lastMs, 3);
        }

        /// <summary>
        /// 视频截图
        /// </summary>
        public static bool GetPhotoFromVideo(string videoPhysicalPath, string imgPhysicalPath, int delayTime)
        {
            string standardDelayTime = GetFFmpegTimeByMilliscond(delayTime);
            FloderUtil.Create(imgPhysicalPath);
            return ExcuteFFmpeg(FFmpegPhysicalPath, " -i " + videoPhysicalPath + " -ss " + standardDelayTime + " " + imgPhysicalPath);
        }

        /// <summary>
        /// 实时视频截图
        /// </summary>
        public static bool GetPhotoFromRtsp(string rtspPath, string imgPhysicalPath)
        {
            FloderUtil.Create(imgPhysicalPath);
            return ExcuteFFmpeg(FFmpegPhysicalPath, " -i " + rtspPath + " " + imgPhysicalPath);
        }

        /// <summary>
        /// 实时视频抓取
        /// </summary>
        public static bool GetH264FromRtsp(string rtspPath, string h264PhysicalPath, int lastTime)
        {
            string standardLastTime = GetFFmpegTimeByMilliscond(lastTime);
            FloderUtil.Create(h264PhysicalPath);
            return ExcuteFFmpeg(FFmpegPhysicalPath, " -i " + rtspPath + " -vcodec copy -acodec copy -t " + standardLastTime + " " + h264PhysicalPath);
        }

        /// <summary>
        /// H264转Mp4
        /// </summary>
        public static bool GetMp4FromH264(string h264PhysicalPath, string Mp4PhysicalPath)
        {
            FloderUtil.Create(Mp4PhysicalPath);
            return ExcuteFFmpeg(FFmpegPhysicalPath, " -i " + h264PhysicalPath + " " + Mp4PhysicalPath);
        }

        /// <summary>
        /// 视频预览图
        /// </summary>
        public static bool SetVideoShoot(string videoPhysicalPath, string imgPhysicalPath, int delayTime)
        {
            string standardDelayTime = GetFFmpegTimeByMilliscond(delayTime);
            FloderUtil.Create(imgPhysicalPath);
            return ExcuteFFmpeg(FFmpegPhysicalPath, " -i " + videoPhysicalPath + " -s 200x124 -ss " + standardDelayTime + " " + imgPhysicalPath);
        }

        /// <summary>
        /// 设置实时视频缩略图
        /// </summary>
        public static bool SetDeviceShoot(string rtspPath, string imgPhysicalPath)
        {
            FloderUtil.Create(imgPhysicalPath);
            return ExcuteFFmpeg(FFmpegPhysicalPath, " -i " + rtspPath + " -s 200x124 " + imgPhysicalPath);
        }
    }
}
