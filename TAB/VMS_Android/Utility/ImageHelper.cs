using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Net;
using Android.Graphics;
using Java.Lang;
using Android.Util;

namespace VMS_Android.Utility
{
    public static class ImageHelper
    {
        private static string hexStr = "0123456789ABCDEF";
        private static string[] binaryArray = { "0000", "0001", "0010", "0011",
            "0100", "0101", "0110", "0111", "1000", "1001", "1010", "1011",
            "1100", "1101", "1110", "1111" };

        public static Bitmap GetImageBitmapFromUrl(string url)
        {
            Bitmap imageBitmap = null;
            using (var webCient = new WebClient())
            {
                var images = webCient.DownloadData(url);
                if (images != null && images.Length > 0)
                {
                    imageBitmap = BitmapFactory.DecodeByteArray(images, 0, images.Length);
                }
            }
            return imageBitmap;
        }
        public static byte[] decodeBitmap(Bitmap bmp)
        {
            int bmpWidth = bmp.Width;
            int bmpHeight = bmp.Height;

            List<string> list = new List<string>();//binaryString list
            StringBuffer sb;


            int bitLen = bmpWidth / 8;
            int zeroCount = bmpWidth % 8;

            string zeroStr = "";
            if (zeroCount > 0)
            {
                bitLen = bmpWidth / 8 + 1;
                for (int i = 0; i < (8 - zeroCount); i++)
                {
                    zeroStr = zeroStr + "0";
                }
            }

            for (int i = 0; i < bmpHeight; i++)
            {
                sb = new StringBuffer();
                for (int j = 0; j < bmpWidth; j++)
                {
                    int color = bmp.GetPixel(j, i);

                    int r = (color >> 16) & 0xff;
                    int g = (color >> 8) & 0xff;
                    int b = color & 0xff;

                    // if color close to white，bit='0', else bit='1'
                    if (r > 160 && g > 160 && b > 160)
                        sb.Append("0");
                    else
                        sb.Append("1");
                }
                if (zeroCount > 0)
                {
                    sb.Append(zeroStr);
                }
                list.Add(sb.ToString());
            }

            List<string> bmpHexList =  binaryListToHexStringList(list);
            string commandHexString = "1D763000";
            string widthHexString = Integer
                    .ToHexString(bmpWidth % 8 == 0 ? bmpWidth / 8
                            : (bmpWidth / 8 + 1));
            if (widthHexString.Length > 2)
            {
                Log.Error("decodeBitmap error", " width is too large");
                return null;
            }
            else if (widthHexString.Length == 1)
            {
                widthHexString = "0" + widthHexString;
            }
            widthHexString = widthHexString + "00";

            string heightHexString = Integer.ToHexString(bmpHeight);
            if (heightHexString.Length > 2)
            {
                Log.Error("decodeBitmap error", " height is too large");
                return null;
            }
            else if (heightHexString.Length == 1)
            {
                heightHexString = "0" + heightHexString;
            }
            heightHexString = heightHexString + "00";

            List<string> commandList = new List<string>();
            commandList.Add(commandHexString + widthHexString + heightHexString);
            commandList.AddRange(bmpHexList);

            return hexList2Byte(commandList);
        }

        private static List<string> binaryListToHexStringList(List<string> list)
        {
            List<string> hexList = new List<string>();
            foreach (string binaryStr in list)
            {
                StringBuffer sb = new StringBuffer();
                for (int i = 0; i < binaryStr.Length; i += 8)
                {
                    string str = binaryStr.Substring(i, 8);

                    string hexString = myBinaryStrToHexString(str);
                    sb.Append(hexString);
                }
                hexList.Add(sb.ToString());
            }
            return hexList;
        }
        public static string myBinaryStrToHexString(string binaryStr)
        {
            string hex = "";
            string f4 = binaryStr.Substring(0, 4);
            string b4 = binaryStr.Substring(4, 4);
            for (int i = 0; i < binaryArray.Length; i++)
            {
                if (f4.Equals(binaryArray[i]))
                    hex += hexStr.Substring(i, 1);
            }
            for (int i = 0; i < binaryArray.Length; i++)
            {
                if (b4.Equals(binaryArray[i]))
                    hex += hexStr.Substring(i, 1);
            }

            return hex;
        }
        public static byte[] hexList2Byte(List<string> list)
        {
            List<byte[]> commandList = new List<byte[]>();

            foreach (string hexStr in list)
            {
                commandList.Add(hexStringToBytes(hexStr));
            }
            byte[] bytes = sysCopy(commandList);
            return bytes;
        }
        public static byte[] hexStringToBytes(string hexString)
        {
            if (hexString == null || hexString.Equals(""))
            {
                return null;
            }
            hexString = hexString.ToUpper();
            int length = hexString.Length / 2;
            char[] hexChars = hexString.ToCharArray();
            byte[] d = new byte[length];
            for (int i = 0; i < length; i++)
            {
                int pos = i * 2;
                d[i] = (byte)(charToByte(hexChars[pos]) << 4 | charToByte(hexChars[pos + 1]));
            }
            return d;
        }

        public static byte[] sysCopy(List<byte[]> srcArrays)
        {
            int len = 0;
            foreach (byte[] srcArray in srcArrays)
            {
                len += srcArray.Length;
            }
            byte[] destArray = new byte[len];
            int destLen = 0;
            foreach (byte[] srcArray in srcArrays)
            {
                Array.Copy(srcArray, 0, destArray, destLen, srcArray.Length);
                destLen += srcArray.Length;
            }
            return destArray;
        }

        private static byte charToByte(char c)
        {
            return (byte)"0123456789ABCDEF".IndexOf(c);
        }
    }
}