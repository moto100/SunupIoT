// <copyright file="FileProcess.cs" company="Sunup">
// Copyright (c) Sunup. All rights reserved.
// </copyright>

namespace Sunup.RuntimeSupport
{
    using System.IO;

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
            catch
            {
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
            catch
            {
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
            catch
            {
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
            catch
            {
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
            catch
            {
                ret = false;
            }

            return ret;
        }
    }
}
