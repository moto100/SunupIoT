// <copyright file="Application.cs" company="Sunup">
// Copyright (c) Sunup. All rights reserved.
// </copyright>

namespace Sunup.IOServer
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml;
    using Sunup.App;

    /// <summary>
    /// Application.
    /// </summary>
    public class Application
    {
        private const string ApplicationFileName = "App.xml";
        private bool isAppLoaded = false;
        private bool isAppStarted = false;
        private object appLock = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="Application"/> class.
        /// </summary>
        /// <param name="appRootPath">The root folder path of project.</param>
        public Application(string appRootPath)
        {
            this.AppRootPath = appRootPath;
        }

        /// <summary>
        /// Gets AppContainerModel.
        /// </summary>
        public AppContainer AppContainerModel { get; private set; }

        /// <summary>
        /// Gets app root path.
        /// </summary>
        public string AppRootPath { get; private set; }

        /// <summary>
        /// Start.
        /// </summary>
        /// <param name="appId">appId.</param>
        public void Start(string appId)
        {
            if (this.isAppStarted)
            {
                return;
            }

            lock (this.appLock)
            {
                if (this.isAppStarted)
                {
                    return;
                }

                ////Hjs39d7Hssj9de.Yhkeh77Uhbeds(appId);

                if (this.AppContainerModel != null)
                {
                    this.AppContainerModel.Run();
                }

                this.isAppStarted = true;
            }
        }

        /// <summary>
        /// Load App config.
        /// </summary>
        /// <returns>true if load app successfully.</returns>
        public bool LoadAppConfig()
        {
            if (this.isAppLoaded)
            {
                return true;
            }

            lock (this.appLock)
            {
                if (this.isAppLoaded)
                {
                    return true;
                }

                try
                {
                    var appFilePath = Path.Combine(this.AppRootPath, ApplicationFileName);
                    if (!string.IsNullOrEmpty(this.AppRootPath) && File.Exists(appFilePath))
                    {
                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.Load(appFilePath);

                        var appNode = xmlDoc.SelectSingleNode("App");
                        if (appNode != null)
                        {
                            var platformModelNode = appNode.SelectSingleNode("PlatformModel");
                            var pathAttr = platformModelNode.Attributes["Path"].Value;
                            var path = Path.Combine(this.AppRootPath, pathAttr);
                            if (File.Exists(path))
                            {
                                this.AppContainerModel = new AppContainer(path);
                                this.isAppLoaded = true;
                                return true;
                            }
                            else
                            {
                                Diagnostics.Logger.LogError($"[Application]LoadAppConfig >> PlatformModel is not existing.");
                            }
                        }

                        return false;
                    }
                    else
                    {
                        Diagnostics.Logger.LogError($"[Application]LoadAppConfig >> App.xml is not existing.");
                        this.isAppLoaded = false;
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    Diagnostics.Logger.LogTrace($"[Application]LoadAppConfig >> Failed to load App.xml.", ex);
                    return false;
                }
            }
        }

        /////// <summary>
        /////// Get window.
        /////// </summary>
        /////// <param name="windowName">Window name.</param>
        /////// <returns>WindowInfo.</returns>
        ////public WindowInfo GetWindowInfo(string windowName)
        ////{
        ////    WindowInfo window = this.Windows.Find(x => x.Name.Equals(windowName, System.StringComparison.CurrentCultureIgnoreCase));
        ////    if (window != null)
        ////    {
        ////        if (window.Load(this))
        ////        {
        ////            this.Windows.Add(window);
        ////        }
        ////        else
        ////        {
        ////            window = null;
        ////        }
        ////    }

        ////    return window;
        ////}

        ////private List<WindowInfo> LoadWindowsInfo(XmlNode windowsNode)
        ////{
        ////    List<WindowInfo> windows = new List<WindowInfo>();
        ////    if (windowsNode != null)
        ////    {
        ////        var count = windowsNode.ChildNodes.Count;
        ////        for (int i = 0; i < count; i++)
        ////        {
        ////            var node = windowsNode.ChildNodes[i];
        ////            var name = node.Attributes["Name"].Value;
        ////            var identifier = node.Attributes["Identifier"].Value;
        ////            windows.Add(new WindowInfo()
        ////            {
        ////                Name = name,
        ////                Identifier = identifier,
        ////            });
        ////        }
        ////    }

        ////    return windows;
        ////}

        ////private List<SymbolInfo> LoadSymbolsInfo(XmlNode symbolsNode)
        ////{
        ////    List<SymbolInfo> symbols = new List<SymbolInfo>();
        ////    if (symbolsNode != null)
        ////    {
        ////        var count = symbolsNode.ChildNodes.Count;
        ////        for (int i = 0; i < count; i++)
        ////        {
        ////            var node = symbolsNode.ChildNodes[i];
        ////            var name = node.Attributes["Name"].Value;
        ////            var identifier = node.Attributes["Identifier"].Value;
        ////            symbols.Add(new SymbolInfo()
        ////            {
        ////                Name = name,
        ////                Identifier = identifier,
        ////            });
        ////        }
        ////    }

        ////    return symbols;
        ////}

        ////private List<LayoutInfo> LoadLayoutsInfo(XmlNode layoutsNode)
        ////{
        ////    List<LayoutInfo> layous = new List<LayoutInfo>();
        ////    if (layoutsNode != null)
        ////    {
        ////        var count = layoutsNode.ChildNodes.Count;
        ////        for (int i = 0; i < count; i++)
        ////        {
        ////            var node = layoutsNode.ChildNodes[i];
        ////            var name = node.Attributes["Name"].Value;
        ////            var identifier = node.Attributes["Identifier"].Value;
        ////            layous.Add(new LayoutInfo()
        ////            {
        ////                Name = name,
        ////                Identifier = identifier,
        ////            });
        ////        }
        ////    }

        ////    return layous;
        ////}
    }
}
