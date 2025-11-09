using OpenQA.Selenium;
using System.Linq;

namespace Ui.Tests.Pages
{
    public class RoomPage
    {
        private readonly IWebDriver _driver;

        public RoomPage(IWebDriver driver)
        {
            _driver = driver;
        }

        public void Open(string baseUrl, string userCode)
        {
            _driver.Navigate().GoToUrl($"{baseUrl}/room?userCode={userCode}");
        }

        public IWebElement FindUserRow(string name)
        {
            var rows = _driver.FindElements(By.CssSelector("li[app-participant-card]"));
            return rows.FirstOrDefault(r => r.Text.Contains(name));
        }

        public void ClickDelete(IWebElement row)
        {
            row.FindElement(By.CssSelector("[aria-label='Delete user']")).Click();
        }
    }
}