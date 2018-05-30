using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace DSR_Filter_Customizer
{
    class DSRProcess
    {
        [DllImport("user32.dll")]
        private static extern IntPtr GetWindowThreadProcessId(IntPtr hWnd, out uint ProcessId);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        public static DSRProcess GetProcess()
        {
            DSRProcess result = null;
            Process[] candidates = Process.GetProcessesByName("DarkSoulsRemastered");
            if (candidates.Length > 0)
            {
                Process candidate = candidates[0];
                if (!candidate.HasExited)
                    result = new DSRProcess(candidate);
            }
            return result;
        }

        private readonly Process process;
        private readonly DSRInterface dsrInterface;
        private readonly DSROffsets offsets;

        public readonly int ID;
        public readonly string Version;
        public readonly bool Valid;

        public static readonly Dictionary<int, string> versions = new Dictionary<int, string>()
        {
            [0x4869400] = "1.01",
            [0x496BE00] = "1.01.1",
        };

        public DSRProcess(Process candidate)
        {
            process = candidate;
            ID = process.Id;

            int size = process.MainModule.ModuleMemorySize;
            if (versions.ContainsKey(size))
                Version = versions[size];
            else
                Version = String.Format("0x{0:X}", size);

            offsets = new DSROffsets();
            dsrInterface = new DSRInterface(process);
            try
            {
                DSRInterface.AOBScanner scanner = dsrInterface.GetAOBScanner();
                offsets.GraphicsDataPtr = scanner.Scan(DSROffsets.GraphicsDataAOB, 3);
                offsets.WorldChrBasePtr = scanner.Scan(DSROffsets.WorldChrBaseAOB, 3);
                offsets.Unknown1Ptr = scanner.Scan(DSROffsets.Unknown1AOB, 1);
                offsets.Unknown2Ptr = scanner.Scan(DSROffsets.Unknown2AOB, 3);
                Valid = true;
            }
            catch (ArgumentException)
            {
                Valid = false;
            }
        }

        public void Close()
        {
            dsrInterface.Close();
        }

        public bool Alive()
        {
            return !process.HasExited;
        }

        public bool Loaded()
        {
            if (Valid)
            {
                IntPtr chrAnimData = dsrInterface.ResolveAddress(offsets.WorldChrBasePtr, DSROffsets.ChrData1, DSROffsets.ChrMapData, DSROffsets.ChrAnimData);
                return chrAnimData != IntPtr.Zero;
            }
            else
                return false;
        }

        public bool Focused()
        {
            IntPtr hwnd = GetForegroundWindow();
            GetWindowThreadProcessId(hwnd, out uint pid);
            return pid == process.Id;
        }

        private struct DSRPointers
        {
            public IntPtr GraphicsData, Unknown1, Unknown2;
        }
        private DSRPointers pointers;

        public void LoadPointers()
        {
            pointers.GraphicsData = dsrInterface.ResolveAddress(offsets.GraphicsDataPtr, DSROffsets.GraphicsDataOffset);
            pointers.Unknown1 = dsrInterface.ReadIntPtr(offsets.Unknown1Ptr);
            pointers.Unknown2 = dsrInterface.ResolveAddress(offsets.Unknown2Ptr, DSROffsets.Unknown2Offset);
        }

        public int GetWorld()
        {
            return dsrInterface.ReadByte(pointers.Unknown1 + (int)DSROffsets.Unknown1.World);
        }

        public int GetFilter()
        {
            return dsrInterface.ReadByte(pointers.Unknown2 + (int)DSROffsets.Unknown2.Filter);
        }

        public void SetFilterOverride(bool value)
        {
            dsrInterface.WriteBool(pointers.GraphicsData + (int)DSROffsets.GraphicsData.FilterOverride, value);
        }

        public void SetFilterValues(float brightR, float brightG, float brightB, float contR, float contG, float contB, float saturation, float hue)
        {
            dsrInterface.WriteFloat(pointers.GraphicsData + (int)DSROffsets.GraphicsData.FilterBrightnessR, brightR);
            dsrInterface.WriteFloat(pointers.GraphicsData + (int)DSROffsets.GraphicsData.FilterBrightnessG, brightG);
            dsrInterface.WriteFloat(pointers.GraphicsData + (int)DSROffsets.GraphicsData.FilterBrightnessB, brightB);
            dsrInterface.WriteFloat(pointers.GraphicsData + (int)DSROffsets.GraphicsData.FilterContrastR, contR);
            dsrInterface.WriteFloat(pointers.GraphicsData + (int)DSROffsets.GraphicsData.FilterContrastG, contG);
            dsrInterface.WriteFloat(pointers.GraphicsData + (int)DSROffsets.GraphicsData.FilterContrastB, contB);
            dsrInterface.WriteFloat(pointers.GraphicsData + (int)DSROffsets.GraphicsData.FilterSaturation, saturation);
            dsrInterface.WriteFloat(pointers.GraphicsData + (int)DSROffsets.GraphicsData.FilterHue, hue);
        }
    }
}
