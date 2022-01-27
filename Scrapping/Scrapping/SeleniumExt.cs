using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace Scrapping
{
    // Classe de apoio
    public static class SeleniumExt
    {
        public static IWebElement FindElement(this IWebDriver driver, By by, int timeoutInSeconds)
        {
            if (timeoutInSeconds > 0)
            {
                // Timer para ser usado em conjunto com o webdriver
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds));
                return wait.Until(drv =>
                {
                    try
                    {
                        return drv.FindElement(by);
                    }
                    catch (Exception)
                    {
                        return null;
                    }
                });
            }
            return driver.FindElement(by);
        }
        public static void WaitMS(this IWebDriver driver, double delay)
        {
            var now = DateTime.Now;
            var wait = new WebDriverWait(driver, TimeSpan.FromMilliseconds(delay));
            wait.PollingInterval = TimeSpan.FromMilliseconds(delay);
            wait.Until(wd => (DateTime.Now - now) - TimeSpan.FromMilliseconds(delay) > TimeSpan.Zero);
        }
        public static Boolean ExistElement(this IWebDriver driver, By by, int timeoutInSeconds)
        {
            try
            {
                if (timeoutInSeconds > 0)
                {
                    var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds));
                    return wait.Until(drv =>
                    {
                        try
                        {
                            return drv.FindElement(by, timeoutInSeconds).Displayed;
                        }
                        catch (Exception)
                        {
                            return false;
                        }
                    });
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
