﻿using System;
using System.Management;

namespace DomiLibrary.Utility.Helper
{
    /// <summary>
    /// WMI Helper
    /// </summary>
    public class WmiHelper
    {
        /// <summary>
        /// Invoca un comando en un servidor remotamente.
        /// </summary>
        /// <param name="ipServer"></param>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        public static string InvokeRemoteCommand(string ipServer, string user, string password, string command)
        {
            try
            {
                var processToRun = new[] { command };
                var connection = new ConnectionOptions { Username = user, Password = password };
                var wmiScope = new ManagementScope(String.Format("\\\\{0}\\root\\cimv2", ipServer), connection);
                var wmiProcess = new ManagementClass(wmiScope, new ManagementPath("Win32_Process"), new ObjectGetOptions());
                var result = wmiProcess.InvokeMethod("Create", processToRun);

                return result.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
