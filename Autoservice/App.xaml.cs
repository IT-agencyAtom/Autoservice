using System;
using System.Reflection;
using System.Text;
using System.Windows;
using NLog;
using NLog.Config;
using NLog.Targets;
using Autoservice.ViewModel.Utils;

namespace Autoservice
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected Logger _logger;

        public App()
        {
            Startup += App_Startup;
            AppDomain.CurrentDomain.SetData("DataDirectory", Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));

#if DEBUG
#else
            Dispatcher.UnhandledException += OnDispatcherUnhandledException;
#endif

            _logger = LogManager.GetLogger(GetType().FullName);

            var myNLogConfiguration = new LoggingConfiguration();
            var myEmailTarget = new MailTarget { UseSystemNetMailSettings = false };
            var mailRule = new LoggingRule("*", LogLevel.Error, myEmailTarget);

            myEmailTarget.SmtpServer = "smtp.yandex.ru";
            myEmailTarget.SmtpAuthentication = SmtpAuthenticationMode.Basic;
            myEmailTarget.SmtpUserName = "feedback@constasoft.ru";
            myEmailTarget.SmtpPassword = "txtxiqktfubibmgo";

            myEmailTarget.Encoding = Encoding.UTF8;
            myEmailTarget.SmtpPort = 25;
            myEmailTarget.EnableSsl = true;
            myEmailTarget.AddNewLines = true;
            myEmailTarget.Html = true;
            //myEmailTarget.Layout = "${message}";
            myEmailTarget.To = "dir@constasoft.ru";
            myEmailTarget.From = "feedback@constasoft.ru";
            myEmailTarget.Subject = $"Error from {AppDomain.CurrentDomain.FriendlyName}";
            myEmailTarget.Body = "${message}";

            myNLogConfiguration.AddTarget("logemail", myEmailTarget);
            myNLogConfiguration.LoggingRules.Add(mailRule);

            var consoleTarget = new ColoredConsoleTarget();
            myNLogConfiguration.AddTarget("console", consoleTarget);

            var fileTarget = new FileTarget();
            myNLogConfiguration.AddTarget("file", fileTarget);

            // Step 3. Set target properties 
            consoleTarget.Layout = fileTarget.Layout = myEmailTarget.Layout = @"${longdate} | ${level:uppercase=true:padding=5} | ${message} | ${exception:format=type,tostring}";
            fileTarget.FileName = @"${specialfolder:ApplicationData}\ConstaSoft\" + Assembly.GetExecutingAssembly().GetName().Name + @"\Logs\${shortdate}.log";

            // Step 4. Define rules
            var rule1 = new LoggingRule("*", LogLevel.Debug, consoleTarget);
            myNLogConfiguration.LoggingRules.Add(rule1);

            var rule2 = new LoggingRule("*", LogLevel.Debug, fileTarget);
            myNLogConfiguration.LoggingRules.Add(rule2);

            LogManager.Configuration = myNLogConfiguration;
        }

        private void App_Startup(object sender, StartupEventArgs e)
        {
            StartupUri = new Uri($@"pack://application:,,,/{Assembly.GetExecutingAssembly().GetName().Name};Component/MainWindow.xaml");
        }

        void OnDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            var ex = e.Exception;

            Constants.printException(_logger, ex);

            MessageBox.Show(String.Format("Application exception. Send logs to us and we fix it fast."), "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            Environment.Exit(1);
        }
    }
}
