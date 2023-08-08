// <copyright file="FileProcess.cs" company="Sunup">
// Copyright (c) Sunup. All rights reserved.
// </copyright>

namespace Sunup.Utility
{
    using System;
    using System.IO;
    using Sunup.Diagnostics;

    /// <summary>
    /// FileProcess.
    /// </summary>
    public class FileProcess
    {
        /// <summary>
        /// SaveFile.
        /// </summary>
        /// <param name="filePath">filePath.</param>
        /// <param name="data">data.</param>
        /// <returns>true is ok.</returns>
        public bool SaveFile(string filePath, byte[] data)
        {
            var ret = true;
            System.IO.FileStream file = null;
            try
            {
                file = new System.IO.FileStream(filePath, FileMode.OpenOrCreate);
                file.Write(data, 0, data.Length);
            }
            catch (Exception ex)
            {
                Logger.LogError("[FileProcess] Failed to Save file", ex);
                ret = false;
            }
            finally
            {
                if (file != null)
                {
                    file.Close();
                    file.Dispose();
                }
            }

            return ret;
        }

        /// <summary>
        /// SaveFile.
        /// </summary>
        /// <param name="filePath">filePath.</param>
        /// <param name="data">data.</param>
        /// <returns>true is ok.</returns>
        public bool SaveFile(string filePath, string data)
        {
            var ret = true;
            try
            {
                File.WriteAllText(filePath, data);
            }
            catch (Exception ex)
            {
                Logger.LogError("[FileProcess] Failed to Save file", ex);
                ret = false;
            }

            return ret;
        }

        /// <summary>
        /// ReadFile.
        /// </summary>
        /// <param name="filePath">filePath.</param>
        /// <param name="data">data.</param>
        /// <returns>true is ok.</returns>
        public bool ReadFile(string filePath, out string data)
        {
            var ret = true;
            data = null;
            try
            {
                data = File.ReadAllText(filePath);
            }
            catch (Exception ex)
            {
                Logger.LogError("[FileProcess] Failed to read file", ex);
                ret = false;
            }

            return ret;
        }

        /// <summary>
        /// ReadFile.
        /// </summary>
        /// <param name="filePath">filePath.</param>
        /// <param name="data">data.</param>
        /// <returns>true is ok.</returns>
        public bool ReadFile(string filePath, out byte[] data)
        {
            var ret = true;
            data = null;
            System.IO.FileStream file = null;
            try
            {
                file = new System.IO.FileStream(filePath, FileMode.OpenOrCreate);
                data = new byte[file.Length];
                file.Read(data, 0, data.Length);
            }
            catch (Exception ex)
            {
                Logger.LogError("[FileProcess] Failed to read file", ex);
                ret = false;
            }
            finally
            {
                if (file != null)
                {
                    file.Close();
                    file.Dispose();
                }
            }

            return ret;
        }

        /// <summary>
        /// CopyDirectory.
        /// </summary>
        /// <param name="sourcePath">SourcePath.</param>
        /// <param name="destinationPath">DestinationPath.</param>
        /// <param name="overwriteExisting">overwriteExisting.</param>
        /// <returns>true.</returns>
        public bool CopyDirectory(string sourcePath, string destinationPath, bool overwriteExisting)
        {
            bool ret = false;
            try
            {
                sourcePath = sourcePath.EndsWith(@"\") ? sourcePath : sourcePath + @"\";
                destinationPath = destinationPath.EndsWith(@"\") ? destinationPath : destinationPath + @"\";

                if (Directory.Exists(sourcePath))
                {
                    if (Directory.Exists(destinationPath) == false)
                    {
                        Directory.CreateDirectory(destinationPath);
                    }

                    foreach (string fls in Directory.GetFiles(sourcePath))
                    {
                        FileInfo flinfo = new FileInfo(fls);
                        flinfo.CopyTo(destinationPath + flinfo.Name, overwriteExisting);
                    }

                    foreach (string drs in Directory.GetDirectories(sourcePath))
                    {
                        DirectoryInfo drinfo = new DirectoryInfo(drs);
                        if (this.CopyDirectory(drs, destinationPath + drinfo.Name, overwriteExisting) == false)
                        {
                            ret = false;
                        }
                    }
                }

                ret = true;
            }
            catch (Exception ex)
            {
                Logger.LogError("[CopyDirectory] Failed to find file", ex);
                ret = false;
            }

            return ret;
        }

        /// <summary>
        /// GetFile.
        /// </summary>
        /// <param name="currentDir">currentDir.</param>
        /// <param name="extion">extion.</param>
        /// <param name="filePath">filePath.</param>
        /// <returns>if true.</returns>
        public bool GetFile(string currentDir, string extion, out string filePath)
        {
            var ret = true;
            filePath = null;
            try
            {
                string[] files = System.IO.Directory.GetFiles(currentDir, extion, System.IO.SearchOption.TopDirectoryOnly);
                if (files.Length > 0)
                {
                    filePath = files[0];
                }
                else
                {
                    ret = false;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("[FileProcess] Failed to find file", ex);
                ret = false;
            }

            return ret;
        }
    }
}
