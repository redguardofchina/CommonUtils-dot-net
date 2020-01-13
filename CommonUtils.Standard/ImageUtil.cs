using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace CommonUtils
{
    /// <summary>
    /// 绘图工具
    /// </summary>
    public static class ImageUtil
    {
        public static Bitmap GetRandomFace()
        {
            var stream = HttpUtil.GetStream("https://thispersondoesnotexist.com/image");
            return Get(stream);
        }

        /// <summary>
        /// 获取图片类
        /// </summary>
        public static Bitmap Get(string path)
        {
            try
            {
                return new Bitmap(path);
            }
            catch (Exception ex)
            {
                LogUtil.Log(ex);
                return null;
            }
        }

        /// <summary>
        /// 获取图片类
        /// </summary>
        public static Bitmap Get(Stream stream)
        {
            try
            {
                var image = new Bitmap(stream);
                //这个地方不能关，关了图片就不能用了
                //stream.Close();
                return image;
            }
            catch (Exception ex)
            {
                LogUtil.Log(ex);
                return null;
            }
        }

        /// <summary>
        /// 获取图片类
        /// </summary>
        public static Bitmap Get(byte[] bytes)
        => Get(bytes.ToStream());

        public static bool Compare(this ImageFormat format1, ImageFormat format2)
        => format1.Guid == format2.Guid;

        public static ImageFormat[] GetFormats()
        {
            return new ImageFormat[] {
             ImageFormat.MemoryBmp ,
             ImageFormat.Bmp ,
             ImageFormat.Emf ,
             ImageFormat.Wmf ,
             ImageFormat.Gif ,
             ImageFormat.Jpeg ,
             ImageFormat.Png ,
             ImageFormat.Tiff ,
             ImageFormat.Exif ,
             ImageFormat.Icon
            };
        }

        /// <summary>
        /// 获取图片内存流
        /// </summary>
        public static MemoryStream GetStream(this Image image, ImageFormat format = null)
        {
            if (format == null)
                format = image.RawFormat;
            if (format.Compare(ImageFormat.MemoryBmp) || format.Compare(ImageFormat.Icon))
                format = ImageFormat.Png;

            var memory = new MemoryStream();
            image.Save(memory, format);
            memory.Seek();
            return memory;
        }

        public static byte[] GetBytes(this Image image, ImageFormat format = null)
        => GetStream(image, format).GetBytes();

        public static string GetBase64(this Image image, ImageFormat format = null)
        => GetStream(image, format).Base64Encode();

        /// <summary>
        /// 克隆
        /// </summary>
        public static Bitmap Clone(this Image image, ImageFormat format = null)
        => Get(image.GetStream(format));

        /// <summary>
        /// 通过图片类创建图片文件
        /// </summary>
        public static void SaveTo(this Image image, string path)
        {
            try
            {
                FileUtil.CreateFloder(path);
                ImageFormat imageFormat = GetFormat(path);
                ConsoleUtil.Print("图片存储 Format:{0} Path:{1}", imageFormat, path);
                image.Save(path, imageFormat);
            }
            catch (Exception ex)
            {
                LogUtil.Log("图片存储异常", ex);
            }
        }

        /// <summary>
        /// 图片类型转换
        /// </summary>
        public static void ConvertToIcon(string src, string dest, int border = 64)
        {
            var image = new Bitmap(Get(src), border, border);
            var pngStream = image.GetStream(ImageFormat.Png);

            var iconStream = new MemoryStream();
            var binaryWriter = new BinaryWriter(iconStream);

            //写图标头部
            binaryWriter.Write((short)0);           //0-1保留
            binaryWriter.Write((short)1);           //2-3文件类型。1=图标, 2=光标
            binaryWriter.Write((short)1);           //4-5图像数量（图标可以包含多个图像）
            binaryWriter.Write((byte)image.Width);  //6图标宽度
            binaryWriter.Write((byte)image.Height); //7图标高度
            binaryWriter.Write((byte)0);            //8颜色数（若像素位深>=8，填0。这是显然的，达到8bpp的颜色数最少是256，byte不够表示）
            binaryWriter.Write((byte)0);            //9保留。必须为0
            binaryWriter.Write((short)0);           //10-11调色板
            binaryWriter.Write((short)32);          //12-13位深
            binaryWriter.Write((int)pngStream.Length); //14-17位图数据大小
            binaryWriter.Write(22);                 //18-21位图数据起始字节
            binaryWriter.Write(pngStream.ToArray()); //写图像数据
            pngStream.Close();

            //binaryWriter不能关闭，应该flush
            binaryWriter.Flush();
            iconStream.Seek();
            FileUtil.Save(dest, iconStream);
            binaryWriter.Close();
            iconStream.Close();
        }

        /// <summary>
        /// 图片类型转换
        /// </summary>
        public static void Convert(string src, string dest)
        {
            if (src.ToLower() == dest.ToLower())
                return;
            var image = Get(src);
            image.SaveTo(dest);
        }

        public static Stream ConvertTo(this Image image, ImageFormat format)
        {
            var stream = new MemoryStream();
            image.Save(stream, format);
            stream.Seek();
            return stream;
        }

        /// <summary>
        /// 第一个不透明的颜色
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static Color FirstColor(this Bitmap image)
        {
            for (int y = 0; y < image.Height; y++)
                for (int x = 0; x < image.Width; x++)
                    if (image.GetPixel(x, y).A != 0)
                        return image.GetPixel(x, y);
            return Color.Transparent;
        }

        /// <summary>
        /// 抠图
        /// </summary>
        public static Bitmap Tranparent(this Image image)
        {
            var bitmap = image.Clone(ImageFormat.Png);
            var color = bitmap.FirstColor();
            //这里format会变成memory类型
            bitmap.MakeTransparent(color);
            return bitmap.Clone(ImageFormat.Png);
        }

        /// <summary>
        /// 图片旋转
        /// </summary>
        public static bool Rotate(string path)
        {
            try
            {
                var image = Get(path);
                image.RotateFlip(RotateFlipType.Rotate90FlipNone);
                image.SaveTo(path);
                image.Dispose();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 首先在页面返回验证码图片然后返回验证码值
        /// </summary>
        public static Bitmap GetIdentifyCode(out string text)
        {
            char letter;
            text = string.Empty;
            Random random = new Random();

            //1
            do letter = (char)('A' + (char)(random.Next() % 26));
            while (letter == 'O' || letter == 'I');

            text += letter.ToString();

            //2
            do letter = (char)('0' + (char)(random.Next() % 10));
            while (letter == '0');

            text += letter.ToString();

            //3
            do letter = (char)('A' + (char)(random.Next() % 26));
            while (letter == 'O' || letter == 'I');

            text += letter.ToString();

            //4
            do letter = (char)('0' + (char)(random.Next() % 10));
            while (letter == '0');

            text += letter.ToString();

            //绘图
            Color[] colors = new Color[] { Color.Red, Color.Green, Color.Orange, Color.Brown, Color.DarkBlue };
            string[] fonts = new string[] { "Arial" };
            Bitmap image = new Bitmap((int)Math.Ceiling((text.Length * 13.0)), 20);
            Graphics graphics = Graphics.FromImage(image);
            graphics.Clear(Color.White);
            Font font = new Font(fonts[random.Next(fonts.Length)], 12, (FontStyle.Bold | FontStyle.Regular));
            LinearGradientBrush brush = new LinearGradientBrush(new Rectangle(0, 0, image.Width, image.Height), colors[random.Next(colors.Length)], colors[random.Next(colors.Length)], 1.2f, true);
            graphics.DrawString(text, font, brush, 2, 2);
            graphics.DrawRectangle(new Pen(Color.Silver), 0, 0, image.Width - 1, image.Height - 1);
            graphics.Dispose();

            return image;
        }

        /// <summary>
        /// 返回Gif最后一帧
        /// </summary>
        public static Bitmap GetGifLast(string path)
        {
            var gif = Get(path);
            FrameDimension dimension = new FrameDimension(gif.FrameDimensionsList[0]);
            //帧数-1
            int index = gif.GetFrameCount(dimension) - 1;
            //选择GIF的指针
            gif.SelectActiveFrame(dimension, index);
            return gif;

            //获取帧延时的方法
            //PropertyItem frameDelay = gif.GetPropertyItem((int)EnumPropertyType.PropertyTagFrameDelay);
            //float delay = BitConverter.ToInt32(frameDelay.Value, 0) / 100f; // 秒
            //Console.WriteLine("gif frame index {0} delay {1}.", index, delay);
        }

        #region 图片压缩/缩放
        /// <summary>
        /// 获取图片缩放
        /// </summary>
        public static bool Zoom(string src, string dest, int width, int height, long quality)
        {
            ImageCodecInfo codecInfo = GetCodecInfo(dest);
            if (codecInfo == null)
                return false;

            try
            {
                Bitmap srcImage = new Bitmap(src);
                Bitmap destImage = new Bitmap(srcImage, width, height);
                srcImage.Dispose();

                EncoderParameters encoderParams = new EncoderParameters();
                encoderParams.Param = new EncoderParameter[] { new EncoderParameter(Encoder.Quality, quality) };
                FileUtil.CreateFloder(dest);
                destImage.Save(dest, codecInfo, encoderParams);
                destImage.Dispose();
                encoderParams.Dispose();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 获取图片缩放
        /// </summary>
        public static bool Zoom(string srcImagePath, string destImagePath, int destWidth, long quality)
        {
            if (!IsGdi(srcImagePath))
                return false;

            Bitmap srcImage = new Bitmap(srcImagePath);
            int srcWidth = srcImage.Width;
            int srcHeight = srcImage.Height;
            srcImage.Dispose();

            int destHeight = (int)((decimal)srcHeight / srcWidth * destWidth);
            return Zoom(srcImagePath, destImagePath, destWidth, destHeight, quality);
        }

        /// <summary>
        /// 获取图片缩放
        /// </summary>
        public static bool Zoom(string srcImagePath, string destImagePath, int destWidth, int destHeight)
        {
            long quality = 60L;
            return Zoom(srcImagePath, destImagePath, destWidth, destHeight, quality);
        }

        /// <summary>
        /// 获取图片缩放
        /// </summary>
        public static bool Zoom(string src, string dest, int width)
        {
            if (!IsGdi(src))
                return false;

            Bitmap srcImage = new Bitmap(src);
            int srcWidth = srcImage.Width;
            int srcHeight = srcImage.Height;
            srcImage.Dispose();
            int destHeight = (int)((decimal)srcHeight / srcWidth * width);
            return Zoom(src, dest, width, destHeight);
        }

        /// <summary>
        /// 图片缩放
        /// </summary>
        public static Bitmap Zoom(this Bitmap image, int width, int height = 0)
        {
            if (height == 0)
                height = (int)(image.Height * (float)width / image.Width);
            return new Bitmap(image, width, height);
        }

        /// <summary>
        /// 压缩图片(KB)
        /// </summary>
        public static bool Compress(string path, int size, long quality)
        {
            ImageCodecInfo codecInfo = GetCodecInfo(path);
            if (codecInfo == null)
                return false;

            FileInfo fileInfo = new FileInfo(path);
            if (fileInfo.Length / 1024 <= size)
                return true;

            var image = Get(path);
            image.Dispose();
            EncoderParameters encoderParams = new EncoderParameters();
            encoderParams.Param = new EncoderParameter[] { new EncoderParameter(Encoder.Quality, quality) };
            FileUtil.CreateFloder(path);
            image.Save(path, codecInfo, encoderParams);
            encoderParams.Dispose();
            image.Dispose();
            fileInfo = new FileInfo(path);
            if (fileInfo.Length / 1024 <= size)
                return true;

            while (fileInfo.Length / 1024 > size)
            {
                image = Get(path);
                int srcWidth = image.Width;
                image.Dispose();
                int destWidth = (int)(0.8 * srcWidth);
                Zoom(path, path, destWidth, quality);
                fileInfo = new FileInfo(path);
            }

            return true;
        }

        /// <summary>
        /// 压缩图片(KB)
        /// </summary>
        public static bool Compress(string path, int size)
        {
            long quality = 60L;
            return Compress(path, size, quality);
        }
        #endregion

        #region ImagePropertyType
        /// <summary>
        /// 图片属性类型
        /// </summary>
        private enum EnumPropertyType
        {
            PropertyTagGpsVer = 0x0000,
            PropertyTagGpsLatitudeRef = 0x0001,
            PropertyTagGpsLatitude = 0x0002,
            PropertyTagGpsLongitudeRef = 0x0003,
            PropertyTagGpsLongitude = 0x0004,
            PropertyTagGpsAltitudeRef = 0x0005,
            PropertyTagGpsAltitude = 0x0006,
            PropertyTagGpsGpsTime = 0x0007,
            PropertyTagGpsGpsSatellites = 0x0008,
            PropertyTagGpsGpsStatus = 0x0009,
            PropertyTagGpsGpsMeasureMode = 0x000A,
            PropertyTagGpsGpsDop = 0x000B,
            PropertyTagGpsSpeedRef = 0x000C,
            PropertyTagGpsSpeed = 0x000D,
            PropertyTagGpsTrackRef = 0x000E,
            PropertyTagGpsTrack = 0x000F,
            PropertyTagGpsImgDirRef = 0x0010,
            PropertyTagGpsImgDir = 0x0011,
            PropertyTagGpsMapDatum = 0x0012,
            PropertyTagGpsDestLatRef = 0x0013,
            PropertyTagGpsDestLat = 0x0014,
            PropertyTagGpsDestLongRef = 0x0015,
            PropertyTagGpsDestLong = 0x0016,
            PropertyTagGpsDestBearRef = 0x0017,
            PropertyTagGpsDestBear = 0x0018,
            PropertyTagGpsDestDistRef = 0x0019,
            PropertyTagGpsDestDist = 0x001A,
            PropertyTagNewSubfileType = 0x00FE,
            PropertyTagSubfileType = 0x00FF,
            PropertyTagImageWidth = 0x0100,
            PropertyTagImageHeight = 0x0101,
            PropertyTagBitsPerSample = 0x0102,
            PropertyTagCompression = 0x0103,
            PropertyTagPhotometricInterp = 0x0106,
            PropertyTagThreshHolding = 0x0107,
            PropertyTagCellWidth = 0x0108,
            PropertyTagCellHeight = 0x0109,
            PropertyTagFillOrder = 0x010A,
            PropertyTagDocumentName = 0x010D,
            PropertyTagImageDescription = 0x010E,
            PropertyTagEquipMake = 0x010F,
            PropertyTagEquipModel = 0x0110,
            PropertyTagStripOffsets = 0x0111,
            PropertyTagOrientation = 0x0112,
            PropertyTagSamplesPerPixel = 0x0115,
            PropertyTagRowsPerStrip = 0x0116,
            PropertyTagStripBytesCount = 0x0117,
            PropertyTagMinSampleValue = 0x0118,
            PropertyTagMaxSampleValue = 0x0119,
            PropertyTagXResolution = 0x011A,
            PropertyTagYResolution = 0x011B,
            PropertyTagPlanarConfig = 0x011C,
            PropertyTagPageName = 0x011D,
            PropertyTagXPosition = 0x011E,
            PropertyTagYPosition = 0x011F,
            PropertyTagFreeOffset = 0x0120,
            PropertyTagFreeByteCounts = 0x0121,
            PropertyTagGrayResponseUnit = 0x0122,
            PropertyTagGrayResponseCurve = 0x0123,
            PropertyTagT4Option = 0x0124,
            PropertyTagT6Option = 0x0125,
            PropertyTagResolutionUnit = 0x0128,
            PropertyTagPageNumber = 0x0129,
            PropertyTagTransferFunction = 0x012D,
            PropertyTagSoftwareUsed = 0x0131,
            PropertyTagDateTime = 0x0132,
            PropertyTagArtist = 0x013B,
            PropertyTagHostComputer = 0x013C,
            PropertyTagPredictor = 0x013D,
            PropertyTagWhitePoint = 0x013E,
            PropertyTagPrimaryChromaticities = 0x013F,
            PropertyTagColorMap = 0x0140,
            PropertyTagHalftoneHints = 0x0141,
            PropertyTagTileWidth = 0x0142,
            PropertyTagTileLength = 0x0143,
            PropertyTagTileOffset = 0x0144,
            PropertyTagTileByteCounts = 0x0145,
            PropertyTagInkSet = 0x014C,
            PropertyTagInkNames = 0x014D,
            PropertyTagNumberOfInks = 0x014E,
            PropertyTagDotRange = 0x0150,
            PropertyTagTargetPrinter = 0x0151,
            PropertyTagExtraSamples = 0x0152,
            PropertyTagSampleFormat = 0x0153,
            PropertyTagSMinSampleValue = 0x0154,
            PropertyTagSMaxSampleValue = 0x0155,
            PropertyTagTransferRange = 0x0156,
            PropertyTagJPEGProc = 0x0200,
            PropertyTagJPEGInterFormat = 0x0201,
            PropertyTagJPEGInterLength = 0x0202,
            PropertyTagJPEGRestartInterval = 0x0203,
            PropertyTagJPEGLosslessPredictors = 0x0205,
            PropertyTagJPEGPointTransforms = 0x0206,
            PropertyTagJPEGQTables = 0x0207,
            PropertyTagJPEGDCTables = 0x0208,
            PropertyTagJPEGACTables = 0x0209,
            PropertyTagYCbCrCoefficients = 0x0211,
            PropertyTagYCbCrSubsampling = 0x0212,
            PropertyTagYCbCrPositioning = 0x0213,
            PropertyTagREFBlackWhite = 0x0214,
            PropertyTagGamma = 0x0301,
            PropertyTagICCProfileDescriptor = 0x0302,
            PropertyTagSRGBRenderingIntent = 0x0303,
            PropertyTagImageTitle = 0x0320,
            PropertyTagResolutionXUnit = 0x5001,
            PropertyTagResolutionYUnit = 0x5002,
            PropertyTagResolutionXLengthUnit = 0x5003,
            PropertyTagResolutionYLengthUnit = 0x5004,
            PropertyTagPrintFlags = 0x5005,
            PropertyTagPrintFlagsVersion = 0x5006,
            PropertyTagPrintFlagsCrop = 0x5007,
            PropertyTagPrintFlagsBleedWidth = 0x5008,
            PropertyTagPrintFlagsBleedWidthScale = 0x5009,
            PropertyTagHalftoneLPI = 0x500A,
            PropertyTagHalftoneLPIUnit = 0x500B,
            PropertyTagHalftoneDegree = 0x500C,
            PropertyTagHalftoneShape = 0x500D,
            PropertyTagHalftoneMisc = 0x500E,
            PropertyTagHalftoneScreen = 0x500F,
            PropertyTagJPEGQuality = 0x5010,
            PropertyTagGridSize = 0x5011,
            PropertyTagThumbnailFormat = 0x5012,
            PropertyTagThumbnailWidth = 0x5013,
            PropertyTagThumbnailHeight = 0x5014,
            PropertyTagThumbnailColorDepth = 0x5015,
            PropertyTagThumbnailPlanes = 0x5016,
            PropertyTagThumbnailRawBytes = 0x5017,
            PropertyTagThumbnailSize = 0x5018,
            PropertyTagThumbnailCompressedSize = 0x5019,
            PropertyTagColorTransferFunction = 0x501A,
            PropertyTagThumbnailData = 0x501B,
            PropertyTagThumbnailImageWidth = 0x5020,
            PropertyTagThumbnailImageHeight = 0x5021,
            PropertyTagThumbnailBitsPerSample = 0x5022,
            PropertyTagThumbnailCompression = 0x5023,
            PropertyTagThumbnailPhotometricInterp = 0x5024,
            PropertyTagThumbnailImageDescription = 0x5025,
            PropertyTagThumbnailEquipMake = 0x5026,
            PropertyTagThumbnailEquipModel = 0x5027,
            PropertyTagThumbnailStripOffsets = 0x5028,
            PropertyTagThumbnailOrientation = 0x5029,
            PropertyTagThumbnailSamplesPerPixel = 0x502A,
            PropertyTagThumbnailRowsPerStrip = 0x502B,
            PropertyTagThumbnailStripBytesCount = 0x502C,
            PropertyTagThumbnailResolutionX = 0x502D,
            PropertyTagThumbnailResolutionY = 0x502E,
            PropertyTagThumbnailPlanarConfig = 0x502F,
            PropertyTagThumbnailResolutionUnit = 0x5030,
            PropertyTagThumbnailTransferFunction = 0x5031,
            PropertyTagThumbnailSoftwareUsed = 0x5032,
            PropertyTagThumbnailDateTime = 0x5033,
            PropertyTagThumbnailArtist = 0x5034,
            PropertyTagThumbnailWhitePoint = 0x5035,
            PropertyTagThumbnailPrimaryChromaticities = 0x5036,
            PropertyTagThumbnailYCbCrCoefficients = 0x5037,
            PropertyTagThumbnailYCbCrSubsampling = 0x5038,
            PropertyTagThumbnailYCbCrPositioning = 0x5039,
            PropertyTagThumbnailRefBlackWhite = 0x503A,
            PropertyTagThumbnailCopyRight = 0x503B,
            PropertyTagLuminanceTable = 0x5090,
            PropertyTagChrominanceTable = 0x5091,
            PropertyTagFrameDelay = 0x5100,
            PropertyTagLoopCount = 0x5101,
            PropertyTagGlobalPalette = 0x5102,
            PropertyTagIndexBackground = 0x5103,
            PropertyTagIndexTransparent = 0x5104,
            PropertyTagPixelUnit = 0x5110,
            PropertyTagPixelPerUnitX = 0x5111,
            PropertyTagPixelPerUnitY = 0x5112,
            PropertyTagPaletteHistogram = 0x5113,
            PropertyTagCopyright = 0x8298,
            PropertyTagExifExposureTime = 0x829A,
            PropertyTagExifFNumber = 0x829D,
            PropertyTagExifIFD = 0x8769,
            PropertyTagICCProfile = 0x8773,
            PropertyTagExifExposureProg = 0x8822,
            PropertyTagExifSpectralSense = 0x8824,
            PropertyTagGpsIFD = 0x8825,
            PropertyTagExifISOSpeed = 0x8827,
            PropertyTagExifOECF = 0x8828,
            PropertyTagExifVer = 0x9000,
            PropertyTagExifDTOrig = 0x9003,
            PropertyTagExifDTDigitized = 0x9004,
            PropertyTagExifCompConfig = 0x9101,
            PropertyTagExifCompBPP = 0x9102,
            PropertyTagExifShutterSpeed = 0x9201,
            PropertyTagExifAperture = 0x9202,
            PropertyTagExifBrightness = 0x9203,
            PropertyTagExifExposureBias = 0x9204,
            PropertyTagExifMaxAperture = 0x9205,
            PropertyTagExifSubjectDist = 0x9206,
            PropertyTagExifMeteringMode = 0x9207,
            PropertyTagExifLightSource = 0x9208,
            PropertyTagExifFlash = 0x9209,
            PropertyTagExifFocalLength = 0x920A,
            PropertyTagExifMakerNote = 0x927C,
            PropertyTagExifUserComment = 0x9286,
            PropertyTagExifDTSubsec = 0x9290,
            PropertyTagExifDTOrigSS = 0x9291,
            PropertyTagExifDTDigSS = 0x9292,
            PropertyTagExifFPXVer = 0xA000,
            PropertyTagExifColorSpace = 0xA001,
            PropertyTagExifPixXDim = 0xA002,
            PropertyTagExifPixYDim = 0xA003,
            PropertyTagExifRelatedWav = 0xA004,
            PropertyTagExifInterop = 0xA005,
            PropertyTagExifFlashEnergy = 0xA20B,
            PropertyTagExifSpatialFR = 0xA20C,
            PropertyTagExifFocalXRes = 0xA20E,
            PropertyTagExifFocalYRes = 0xA20F,
            PropertyTagExifFocalResUnit = 0xA210,
            PropertyTagExifSubjectLoc = 0xA214,
            PropertyTagExifExposureIndex = 0xA215,
            PropertyTagExifSensingMethod = 0xA217,
            PropertyTagExifFileSource = 0xA300,
            PropertyTagExifSceneType = 0xA301,
            PropertyTagExifCfaPattern = 0xA302,
        }
        #endregion

        #region ImageCodecInfo
        /// <summary>
        /// 获取所有的图片编码信息
        /// </summary>
        /// <returns></returns>
        private static ImageCodecInfo[] GdiCodecInfos { get; set; }

        /// <summary>
        /// 获取所有的图片MimeType信息
        /// </summary>
        /// <returns></returns>
        private static string[] GdiMimeTypes { get; set; }

        /// <summary>
        /// 获取所有的图片后缀名信息
        /// </summary>
        /// <returns></returns>
        private static string[] GdiExtensions { get; set; }

        /// <summary>
        /// 初始化
        /// </summary>
        static ImageUtil()
        {
            GdiCodecInfos = ImageCodecInfo.GetImageEncoders();

            List<string> listMimeType = new List<string>();
            foreach (ImageCodecInfo codecInfo in GdiCodecInfos)
                listMimeType.Add(codecInfo.MimeType);
            GdiMimeTypes = listMimeType.ToArray();

            List<string> listExtension = new List<string>();
            foreach (ImageCodecInfo codecInfo in GdiCodecInfos)
            {
                string extensionStrs = codecInfo.FilenameExtension.Replace("*", "").ToLower();
                string[] extensions = extensionStrs.Split(';');
                listExtension.AddRange(extensions);
            }
            GdiExtensions = listExtension.OrderBy(m => m).ToArray();
        }

        /// <summary>
        /// 根据后缀名,返回编码器
        /// </summary>
        private static ImageCodecInfo GetCodecInfo(string filePath)
        {
            string fileExtension = FileUtil.GetExtension(filePath);
            ImageCodecInfo[] imageCodecInfos = GdiCodecInfos;
            foreach (ImageCodecInfo imageCodecInfo in imageCodecInfos)
            {
                string[] imageExtensions = imageCodecInfo.FilenameExtension.Replace("*", "").Split(';');
                foreach (string imageExtension in imageExtensions)
                    if (fileExtension.ToUpper() == imageExtension.ToUpper())
                        return imageCodecInfo;
            }
            return null;
        }

        /// <summary>
        /// 判断路径是否为程序支持的图片
        /// </summary>
        private static bool IsGdi(string filePath)
        => GetCodecInfo(filePath) != null;

        /// <summary>
        /// 根据 mime 类型,返回编码器
        /// </summary>
        private static ImageCodecInfo GetCodecInfoByMimeType(string mimeType)
        {
            ImageCodecInfo[] codecInfos = ImageCodecInfo.GetImageEncoders();
            foreach (ImageCodecInfo codecInfo in codecInfos)
            {
                if (codecInfo.MimeType.ToUpper() == mimeType.ToUpper())
                {
                    return codecInfo;
                }
            }
            return null;
        }

        /// <summary>
        /// 根据后缀名,返回编码类型
        /// </summary>
        public static ImageFormat GetFormat(string imagePath)
        {
            var extension = FileUtil.GetExtension(imagePath).ToLower();
            switch (extension)
            {
                case ".jpg":
                case ".jpeg":
                    return ImageFormat.Jpeg;
                case ".bmp":
                    return ImageFormat.Bmp;
                case ".png":
                    return ImageFormat.Png;
                case ".gif":
                    return ImageFormat.Gif;
                case ".icon":
                case ".ico":
                    return ImageFormat.Icon;
            }
            Console.WriteLine(extension + "未获取到图片编码");
            return null;
        }
        #endregion
    }
}
