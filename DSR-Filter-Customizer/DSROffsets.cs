using System;
using System.Text.RegularExpressions;

namespace DSR_Filter_Customizer
{
    class DSROffsets
    {
        public static byte?[] GraphicsDataAOB = getAOB("48 8B 05 ? ? ? ? 48 8B 48 08 48 8B 01 48 8B 40 58");
        public IntPtr GraphicsDataPtr;
        public static int GraphicsDataOffset = 0x738;
        public enum GraphicsData
        {
            FilterOverride = 0x34D,
            FilterBrightnessR = 0x350,
            FilterBrightnessG = 0x354,
            FilterBrightnessB = 0x358,
            FilterSaturation = 0x35C,
            FilterContrastR = 0x360,
            FilterContrastG = 0x364,
            FilterContrastB = 0x368,
            FilterHue = 0x36C,
        }

        public static byte?[] WorldChrBaseAOB = getAOB("48 8B 05 ? ? ? ? 48 8B 48 68 48 85 C9 0F 84 ? ? ? ? 48 39 5E 10 0F 84 ? ? ? ? 48");
        public IntPtr WorldChrBasePtr;
        public static int ChrData1 = 0x68;
        public static int ChrMapData = 0x48;
        public static int ChrAnimData = 0x18;

        public static byte?[] Unknown1AOB = getAOB("05 ? ? ? ? 44 8B C1 48 8B 90 78 0B 00 00 48 83 7A 30 00");
        public IntPtr Unknown1Ptr;
        public enum Unknown1
        {
            Area = 0xA22,
            World = 0xA23,
        }

        public static byte?[] Unknown2AOB = getAOB("48 8B 05 ? ? ? ? 8B 40 30 89 44 24 28 48 8D 55 F0 48 8B 4B 68");
        public IntPtr Unknown2Ptr;
        public static int Unknown2Offset = 0x28;
        public enum Unknown2
        {
            Filter = 0x1F0,
        }

        private static byte?[] getAOB(string text)
        {
            MatchCollection matches = Regex.Matches(text, @"\S+");
            byte?[] aob = new byte?[matches.Count];
            for (int i = 0; i < aob.Length; i++)
            {
                Match match = matches[i];
                if (match.Value == "?")
                    aob[i] = null;
                else
                    aob[i] = Byte.Parse(match.Value, System.Globalization.NumberStyles.AllowHexSpecifier);
            }
            return aob;
        }
    }
}
