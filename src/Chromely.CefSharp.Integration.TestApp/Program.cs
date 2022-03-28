// Copyright ?2017-2020 Chromely Projects. All rights reserved.
// Use of this source code is governed by Chromely MIT licensed and CefSharp BSD-style license that can be found in the LICENSE file.

using System;
using Chromely.Core;
using Chromely.Core.Configuration;
using Chromely.Core.Infrastructure;
using Chromely.Core.Host;
using Chromely.CefSharp;
using CefSharp;

namespace Chromely.Integration.TestApp
{
    /// <summary>
    /// 
    /// This is a minimal chromely application to be used during integration tests.
    ///
    /// 
    /// It will emit console outputs starting with "CI-TRACE:" which are checked
    /// in the test run - so DON'T REMOVE them.
    /// 
    /// </summary>
    internal static class Program
    {
        private const string TraceSignature = "CI-TRACE:";

        private static void CiTrace(string key, string value)
        {
            Console.WriteLine($"{TraceSignature} {key}={value}");
        }

        private static int Main(string[] args)
        {
            CiTrace("Application", "Started");

            var core = typeof(IChromelyConfiguration).Assembly;
            CiTrace("Chromely.Core", core.GetName().Version.ToString());
            CiTrace("Platform", ChromelyRuntime.Platform.ToString());

            var appDirectory = AppDomain.CurrentDomain.BaseDirectory;
            CiTrace("AppDirectory", appDirectory);
            var startUrl = $"file:///{appDirectory}/index.html";
            //var startUrl = "https://coding2233.github.io/webgl/imgui/index.html";

            var config = DefaultConfiguration.CreateForRuntimePlatform();
            config.CefDownloadOptions = new CefDownloadOptions(true, true);
            config.WindowOptions.Position = new WindowPosition(0, 0);//1,2
            config.WindowOptions.Size = new WindowSize(1280, 720);
            config.StartUrl = startUrl;
            config.DebuggingMode = true;
            config.WindowOptions.RelativePathToIconFile = "chromely.ico";
            config.WindowOptions.Title = "Online Tool Kit";

            CiTrace("Configuration", "Created");

            try
            {
                var builder = AppBuilder.Create();
                builder = builder.UseConfig<DefaultConfiguration>(config);
                builder = builder.UseWindow<TestWindow>();
                builder = builder.UseApp<ChromelyBasicApp>();
                builder = builder.Build();
                builder.Run(args);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            CiTrace("Application", "Done");
            return 0;
        }
    }

    public class TestWindow : Window
    {
        public TestWindow(IChromelyNativeHost nativeHost,
                      IChromelyConfiguration config,
                      ChromelyHandlersResolver handlersResolver)
            : base(nativeHost, config, handlersResolver)
        {
            #region Events
            FrameLoadStart += TestWindow_FrameLoadStart;
            FrameLoadEnd += TestWindow_FrameLoadEnd;
            LoadingStateChanged += TestWindow_LoadingStateChanged;
            ConsoleMessage += TestWindow_ConsoleMessage;
            AddressChanged += TestWindow_AddressChanged;
            #endregion Events

        }

        #region Events
        private void TestWindow_AddressChanged(object sender, AddressChangedEventArgs e)
        {
            Console.WriteLine("AddressChanged event called.");
        }

        private void TestWindow_ConsoleMessage(object sender, ConsoleMessageEventArgs e)
        {
            Console.WriteLine("ConsoleMessage event called.");
        }

        private void TestWindow_LoadingStateChanged(object sender, LoadingStateChangedEventArgs e)
        {
            Console.WriteLine("LoadingStateChanged event called.");
        }

        private void TestWindow_FrameLoadStart(object sender, FrameLoadStartEventArgs e)
        {
            Console.WriteLine("FrameLoadStart event called.");
        }

        private void TestWindow_FrameLoadEnd(object sender, FrameLoadEndEventArgs e)
        {
            Console.WriteLine("FrameLoadEnd event called.");
        }

        #endregion Events
    }
}

