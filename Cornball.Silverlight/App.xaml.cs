using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Browser;
using Cornball.Resources;

namespace Cornball
{
    public partial class App
    {
        public App()
        {
            Startup += ApplicationStartup;
            Exit += ApplicationExit;
            UnhandledException += ApplicationUnhandledException;

            InitializeComponent();
        }

        private void ApplicationStartup(object sender, StartupEventArgs e)
        {
            Strings.Culture = Thread.CurrentThread.CurrentUICulture;

            RootVisual = new Main();
        }

        private void ApplicationExit(object sender, EventArgs e)
        {
        }

        private void ApplicationUnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (!Debugger.IsAttached)
            {
                e.Handled = true;
                Deployment.Current.Dispatcher.BeginInvoke(() => ReportErrorToDom(e));
            }
        }

        private void ReportErrorToDom(ApplicationUnhandledExceptionEventArgs e)
        {
            if (e != null)
            {
                string error = e.ExceptionObject.Message + e.ExceptionObject.StackTrace;
                error = error.Replace('"', '\'').Replace("\r\n", @"\n");
                HtmlPage.Window.Eval("throw new Error(\"Unhandled Error in Silverlight Application " + error + "\");");
            }
        }
    }
}