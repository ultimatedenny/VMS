// Decompiled with JetBrains decompiler
// Type: VMS_Android.Utility.PrinterCommand
// Assembly: VMS_Android, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A98E6FC-BB78-418F-953B-DFB736DA6BD8
// Assembly location: C:\Users\sbm_adminbn\Desktop\VMS_Android.VMS_Android-Signed\assemblies\VMS_Android.dll

namespace VMS_Android.Utility
{
    public class PrinterCommand
    {
        public static byte[] ESC_FONT_COLOR_DEFAULT = new byte[3]
        {
      (byte) 27,
      byte.Parse("r"),
      (byte) 0
        };
        public static byte[] FS_FONT_ALIGN = new byte[6]
        {
      (byte) 28,
      (byte) 33,
      (byte) 1,
      (byte) 27,
      (byte) 33,
      (byte) 1
        };
        public static byte[] ESC_ALIGN_LEFT = new byte[3]
        {
      (byte) 27,
      byte.Parse("a"),
      (byte) 0
        };
        public static byte[] ESC_ALIGN_RIGHT = new byte[3]
        {
      (byte) 27,
      byte.Parse("a"),
      (byte) 2
        };
        public static byte[] ESC_ALIGN_CENTER = new byte[3]
        {
      (byte) 27,
      byte.Parse("a"),
      (byte) 1
        };
        public static byte[] ESC_CANCEL_BOLD = new byte[3]
        {
      (byte) 27,
      (byte) 69,
      (byte) 0
        };
        public static byte[] ESC_HORIZONTAL_CENTERS = new byte[5]
        {
      (byte) 27,
      (byte) 68,
      (byte) 20,
      (byte) 28,
      (byte) 0
        };
    }
}
