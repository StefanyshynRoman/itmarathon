using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;

namespace Ui.Tests.Helpers
{
    public static class DriverFactory
    {
        [ThreadStatic]
        private static IWebDriver _driver;

        public static IWebDriver GetDriver()
        {
            if (_driver == null)
            {
                var options = new ChromeOptions();
                options.AddArgument("--start-maximized");

                _driver = new ChromeDriver(options);
            }

            return _driver;
        }

        public static void QuitDriver()
        {
            try
            {
                _driver?.Quit();
                _driver = null;
            }
            catch { }
        }
    }
}