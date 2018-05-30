﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace DSR_Filter_Customizer
{
    class DSRInterface
    {
        private const uint PROCESS_ALL_ACCESS = 0x1F0FFF;
        private const uint MEM_COMMIT = 0x1000;
        private const uint MEM_RESERVE = 0x2000;
        private const uint MEM_RELEASE = 0x8000;
        private const uint PAGE_READWRITE = 0x4;
        private const uint PAGE_EXECUTE_READWRITE = 0x40;
        private const uint PAGE_EXECUTE_ANY = 0xF0;
        private const uint PAGE_GUARD = 0x100;

        [DllImport("kernel32.dll")]
        private static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint nSize, uint lpNumberOfBytesRead);

        [DllImport("kernel32.dll")]
        private static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint nSize, uint lpNumberOfBytesWritten);

        [DllImport("kernel32.dll")]
        private static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, uint flAllocationType, uint flProtect);

        [DllImport("kernel32.dll")]
        private static extern bool VirtualFreeEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, uint dwFreeType);

        [DllImport("kernel32.dll")]
        private static extern uint VirtualQueryEx(IntPtr hProcess, IntPtr lpAddress, out MEMORY_BASIC_INFORMATION lpBuffer, uint dwLength);

        [StructLayout(LayoutKind.Sequential)]
        protected struct MEMORY_BASIC_INFORMATION
        {
            public IntPtr BaseAddress;
            public IntPtr AllocationBase;
            public uint AllocationProtect;
            public ulong RegionSize;
            public uint State;
            public uint Protect;
            public uint Type;
        }

        [DllImport("kernel32.dll")]
        private static extern IntPtr CreateRemoteThread(IntPtr hProcess, IntPtr lpThreadAttributes, uint dwStackSize, IntPtr lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, IntPtr lpThreadId);

        [DllImport("kernel32.dll")]
        private static extern int WaitForSingleObject(IntPtr hHandle, uint dwMilliseconds);


        private Process process;
        private IntPtr handle;

        public DSRInterface(Process setProcess)
        {
            process = setProcess;
            handle = process.Handle;
        }

        public void Close()
        {
            process.Close();
        }

        private byte[] ReadProcessMemory(IntPtr address, uint size)
        {
            byte[] result = new byte[size];
            ReadProcessMemory(handle, address, result, size, 0);
            return result;
        }

        private bool WriteProcessMemory(IntPtr address, byte[] bytes)
        {
            return WriteProcessMemory(handle, address, bytes, (uint)bytes.Length, 0);
        }

        private IntPtr VirtualAllocEx(int size, uint protect = PAGE_READWRITE)
        {
            return VirtualAllocEx(handle, IntPtr.Zero, (uint)size, MEM_COMMIT | MEM_RESERVE, protect);
        }

        private bool VirtualFreeEx(IntPtr address)
        {
            return VirtualFreeEx(handle, address, 0, MEM_RELEASE);
        }

        private IntPtr CreateRemoteThread(IntPtr address)
        {
            return CreateRemoteThread(handle, IntPtr.Zero, 0, address, IntPtr.Zero, 0, IntPtr.Zero);
        }

        public IntPtr Allocate(int size)
        {
            return VirtualAllocEx(size);
        }

        public bool Free(IntPtr address)
        {
            return VirtualFreeEx(address);
        }

        public void Execute(byte[] asm)
        {
            IntPtr address = VirtualAllocEx(asm.Length, PAGE_EXECUTE_READWRITE);
            WriteProcessMemory(address, asm);
            IntPtr thread = CreateRemoteThread(address);
            WaitForSingleObject(thread, 0xFFFFFFFF);
            VirtualFreeEx(address);
        }

        public AOBScanner GetAOBScanner()
        {
            return new AOBScanner(process, handle, this);
        }

        public IntPtr ResolveAddress(IntPtr address, params int[] offsets)
        {
            foreach (int offset in offsets)
                address = ReadIntPtr(address) + offset;
            return ReadIntPtr(address);
        }

        public byte[] ReadBytes(IntPtr address, int size)
        {
            return ReadProcessMemory(address, (uint)size);
        }

        public bool ReadBool(IntPtr address)
        {
            byte[] bytes = ReadProcessMemory(address, 1);
            return BitConverter.ToBoolean(bytes, 0);
        }

        public byte ReadByte(IntPtr address)
        {
            byte[] bytes = ReadProcessMemory(address, 1);
            return bytes[0];
        }

        public float ReadFloat(IntPtr address)
        {
            byte[] bytes = ReadProcessMemory(address, 4);
            return BitConverter.ToSingle(bytes, 0);
        }

        public double ReadDouble(IntPtr address)
        {
            byte[] bytes = ReadProcessMemory(address, 8);
            return BitConverter.ToDouble(bytes, 0);
        }

        public short ReadInt16(IntPtr address)
        {
            byte[] bytes = ReadProcessMemory(address, 2);
            return BitConverter.ToInt16(bytes, 0);
        }

        public ushort ReadUInt16(IntPtr address)
        {
            byte[] bytes = ReadProcessMemory(address, 2);
            return BitConverter.ToUInt16(bytes, 0);
        }

        public int ReadInt32(IntPtr address)
        {
            byte[] bytes = ReadProcessMemory(address, 4);
            return BitConverter.ToInt32(bytes, 0);
        }

        public uint ReadUInt32(IntPtr address)
        {
            byte[] bytes = ReadProcessMemory(address, 4);
            return BitConverter.ToUInt32(bytes, 0);
        }

        public long ReadInt64(IntPtr address)
        {
            byte[] bytes = ReadProcessMemory(address, 8);
            return BitConverter.ToInt64(bytes, 0);
        }

        public ulong ReadUInt64(IntPtr address)
        {
            byte[] bytes = ReadProcessMemory(address, 8);
            return BitConverter.ToUInt64(bytes, 0);
        }

        public IntPtr ReadIntPtr(IntPtr address)
        {
            return (IntPtr)ReadInt64(address);
        }

        public bool ReadFlag32(IntPtr address, uint mask)
        {
            byte[] bytes = ReadProcessMemory(address, 4);
            uint flags = BitConverter.ToUInt32(bytes, 0);
            return (flags & mask) != 0;
        }


        public void WriteBool(IntPtr address, bool value)
        {
            WriteProcessMemory(address, BitConverter.GetBytes(value));
        }

        public void WriteByte(IntPtr address, byte value)
        {
            // Note: do not BitConverter.GetBytes this, stupid
            WriteProcessMemory(address, new byte[] { value });
        }

        public void WriteBytes(IntPtr address, byte[] bytes)
        {
            WriteProcessMemory(address, bytes);
        }

        public void WriteFloat(IntPtr address, float value)
        {
            WriteProcessMemory(address, BitConverter.GetBytes(value));
        }

        public void WriteInt16(IntPtr address, short value)
        {
            WriteProcessMemory(address, BitConverter.GetBytes(value));
        }

        public void WriteUInt16(IntPtr address, ushort value)
        {
            WriteProcessMemory(address, BitConverter.GetBytes(value));
        }

        public void WriteInt32(IntPtr address, int value)
        {
            WriteProcessMemory(address, BitConverter.GetBytes(value));
        }

        public void WriteUInt32(IntPtr address, uint value)
        {
            WriteProcessMemory(address, BitConverter.GetBytes(value));
        }

        public void WriteInt64(IntPtr address, long value)
        {
            WriteProcessMemory(address, BitConverter.GetBytes(value));
        }

        public void WriteUInt64(IntPtr address, ulong value)
        {
            WriteProcessMemory(address, BitConverter.GetBytes(value));
        }

        public void WriteFlag32(IntPtr address, uint mask, bool enable)
        {
            uint flags = ReadUInt32(address);
            if (enable)
                flags |= mask;
            else
                flags &= ~mask;
            WriteUInt32(address, flags);
        }

        public class AOBScanner
        {
            private DSRInterface dsrInterface;
            private List<MEMORY_BASIC_INFORMATION> memRegions;
            private Dictionary<IntPtr, byte[]> readMemory;

            public AOBScanner(Process process, IntPtr handle, DSRInterface setDSRInterface)
            {
                dsrInterface = setDSRInterface;
                memRegions = new List<MEMORY_BASIC_INFORMATION>();
                IntPtr memRegionAddr = process.MainModule.BaseAddress;
                IntPtr mainModuleEnd = process.MainModule.BaseAddress + process.MainModule.ModuleMemorySize;
                uint queryResult;

                do
                {
                    MEMORY_BASIC_INFORMATION memInfo = new MEMORY_BASIC_INFORMATION();
                    queryResult = VirtualQueryEx(handle, memRegionAddr, out memInfo, (uint)Marshal.SizeOf(memInfo));
                    if (queryResult != 0)
                    {
                        if ((memInfo.State & MEM_COMMIT) != 0 && (memInfo.Protect & PAGE_GUARD) == 0 && (memInfo.Protect & PAGE_EXECUTE_ANY) != 0)
                            memRegions.Add(memInfo);
                        memRegionAddr = (IntPtr)((ulong)memInfo.BaseAddress.ToInt64() + memInfo.RegionSize);
                    }
                } while (queryResult != 0 && memRegionAddr.ToInt64() < mainModuleEnd.ToInt64());

                readMemory = new Dictionary<IntPtr, byte[]>();
                foreach (MEMORY_BASIC_INFORMATION memRegion in memRegions)
                    readMemory[memRegion.BaseAddress] = dsrInterface.ReadBytes(memRegion.BaseAddress, (int)memRegion.RegionSize);
            }

            public IntPtr Scan(byte?[] aob)
            {
                List<IntPtr> results = new List<IntPtr>();
                foreach (IntPtr baseAddress in readMemory.Keys)
                {
                    byte[] bytes = readMemory[baseAddress];

                    for (int i = 0; i < bytes.Length - aob.Length; i++)
                    {
                        bool found = true;
                        for (int j = 0; j < aob.Length; j++)
                        {
                            if (aob[j] != null && aob[j] != bytes[i + j])
                            {
                                found = false;
                                break;
                            }
                        }

                        if (found)
                            results.Add(baseAddress + i);
                    }
                }

                if (results.Count == 0)
                    throw new ArgumentException("AOB not found: " + aob.ToString());
                else if (results.Count > 1)
                    throw new ArgumentException("AOB found " + results.Count + " times: " + aob.ToString());
                return results[0];
            }

            public IntPtr Scan(byte?[] aob, int offset)
            {
                IntPtr result = Scan(aob);
                return result + dsrInterface.ReadInt32(result + offset) + offset + 4;
            }
        }
    }
}
