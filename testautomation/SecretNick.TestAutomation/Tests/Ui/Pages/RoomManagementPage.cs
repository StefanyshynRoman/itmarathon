// testautomation/SecretNick.TestAutomation/Tests/Ui/Pages/RoomManagementPage.cs
using Microsoft.Playwright;

namespace SecretNick.TestAutomation.Tests.Ui.Pages;

public class RoomManagementPage
{
    private readonly IPage _page;

    // Конструктор
    public RoomManagementPage(IPage page)
    {
        _page = page;
    }

    // Локатори (приклади, вам потрібно буде знайти справжні)
    private ILocator UserRow(string userName) =>
        _page.Locator($"//tr[contains(., '{userName}')]");

    private ILocator DeleteButtonForUser(string userName) =>
        UserRow(userName).Locator("button.delete-user-btn"); // Приклад CSS селектора

    // Методи (Дії)
    public async Task DeleteUser(string userName)
    {
        await DeleteButtonForUser(userName).ClickAsync();
        // Тут можна додати логіку підтвердження (натискання "Так" у діалоговому вікні)
        // await _page.Locator("#confirm-delete-btn").ClickAsync();
    }

    public async Task<bool> IsUserVisible(string userName)
    {
        return await UserRow(userName).IsVisibleAsync();
    }

    public async Task<bool> IsDeleteButtonDisabled(string userName)
    {
        return await DeleteButtonForUser(userName).IsDisabledAsync();
    }
}