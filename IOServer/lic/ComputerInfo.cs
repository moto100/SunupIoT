// <copyright file="ComputerInfo.cs" company="Sunup">
// Copyright (c) Sunup. All rights reserved.
// </copyright>

namespace Sunup.IOServer
{
    using System;
    using System.Management;
    using System.Net.NetworkInformation;
    using Microsoft.Win32;
    using Sunup.Diagnostics;

    /// <summary>
    /// ComputerInfo.
    /// </summary>
    public class ComputerInfo
    {
        /// <summary>
        /// GetComputerInfo.
        /// </summary>
        /// <returns>info.</returns>
        public static string GetComputerInfo()
        {
            string info = string.Empty;
            try
            {
                string cpu = GetCPUInfo();
                string baseBoard = GetBaseBoardInfo();
                string bios = GetBIOSInfo();
                string mac = GetMacAddressByNetworkInformation();
                info = string.Concat(cpu, baseBoard, bios, mac);
            }
            catch (Exception ex)
            {
                Logger.LogError($"[License] Failed to get computer info data.", ex);
                Logger.LogTrace($"[License] Failed to get computer info data.", ex);
            }

            return info;
        }

        private static string GetCPUInfo()
        {
            string info = string.Empty;
            info = GetHardWareInfo("Win32_Processor", "ProcessorId");
            return info;
        }

        private static string GetBIOSInfo()
        {
            string info = string.Empty;
            info = GetHardWareInfo("Win32_BIOS", "SerialNumber");
            return info;
        }

        private static string GetBaseBoardInfo()
        {
            string info = string.Empty;
            info = GetHardWareInfo("Win32_BaseBoard", "SerialNumber");
            return info;
        }

        ////private static string GetMACInfo()
        ////{
        ////    string info = string.Empty;
        ////    info = GetHardWareInfo("Win32_BaseBoard", "SerialNumber");
        ////    return info;
        ////}

        private static string GetHardWareInfo(string typePath, string key)
        {
            try
            {
                ManagementClass managementClass = new ManagementClass(typePath);
                ManagementObjectCollection mn = managementClass.GetInstances();
                PropertyDataCollection properties = managementClass.Properties;
                foreach (PropertyData property in properties)
                {
                    if (property.Name == key)
                    {
                        foreach (ManagementObject m in mn)
                        {
                            return m.Properties[property.Name].Value.ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"[License] Failed to get hardware data. type {typePath}, key {key}", ex);
                Logger.LogTrace($"[License] Failed to get hardware data. type {typePath}, key {key}", ex);
            }

            return string.Empty;
        }

        private static string GetMacAddressByNetworkInformation()
        {
            string key = "SYSTEM\\CurrentControlSet\\Control\\Network\\{4D36E972-E325-11CE-BFC1-08002BE10318}\\";
            string macAddress = string.Empty;
            try
            {
                NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
                foreach (NetworkInterface adapter in nics)
                {
                    if (adapter.NetworkInterfaceType == NetworkInterfaceType.Ethernet
                        && adapter.GetPhysicalAddress().ToString().Length != 0)
                    {
                        string fRegistryKey = key + adapter.Id + "\\Connection";
                        RegistryKey rk = Registry.LocalMachine.OpenSubKey(fRegistryKey, false);
                        if (rk != null)
                        {
                            string fPnpInstanceID = rk.GetValue("PnpInstanceID", string.Empty).ToString();
                            int fMediaSubType = Convert.ToInt32(rk.GetValue("MediaSubType", 0));
                            if (fPnpInstanceID.Length > 3 &&
                                fPnpInstanceID.Substring(0, 3) == "PCI")
                            {
                                macAddress = adapter.GetPhysicalAddress().ToString();
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"[License] Failed to get mac address data.", ex);
                Logger.LogTrace($"[License] Failed to get mac address data.", ex);
            }

            return macAddress;
        }
    }
}
