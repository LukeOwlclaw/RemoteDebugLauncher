﻿
using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.Shell.Interop;

namespace RemoteDebugCommand
{
    public static class Logger
    {
        private static IVsOutputWindowPane pane;
        private static object _syncRoot = new object();
        private static IServiceProvider _provider;
        private static string _name;

        public static void Initialize(IServiceProvider provider, string name)
        {
            _provider = provider;
            _name = name;
        }

        [SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "Microsoft.VisualStudio.Shell.Interop.IVsOutputWindowPane.OutputString(System.String)")]
        public static void Log(string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                return;
            }

            try
            {
                if (EnsurePane())
                {
                    pane.OutputString(DateTime.Now.ToString() + ": " + message + Environment.NewLine);
                }
            }
            catch { }
        }

        public static void Log(Exception ex)
        {
            try
            {
                if (ex != null)
                {
                    Log(ex.ToString());
                }
            }
            catch { }
        }

        private static bool EnsurePane()
        {
            // during unit tests, _provider is not set. Do not try to get pane then.
            if (pane == null && _provider != null)
            {
                Guid guid = Guid.NewGuid();
                IVsOutputWindow output = (IVsOutputWindow)_provider.GetService(typeof(SVsOutputWindow));
                output.CreatePane(ref guid, _name, 1, 1);
                output.GetPane(ref guid, out pane);
            }

            return pane != null;
        }
    }
}
