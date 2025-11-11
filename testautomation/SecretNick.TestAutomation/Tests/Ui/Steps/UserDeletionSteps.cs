// testautomation/SecretNick.TestAutomation/Tests/Ui/Steps/UserDeletionSteps.cs
using FluentAssertions;
using Reqnroll;
using SecretNick.TestAutomation.Tests.Ui.Pages;
using Shouldly;
using Tests.Core.Drivers;

namespace SecretNick.TestAutomation.Tests.Ui.Steps;

[Binding]
public class UserDeletionSteps
{
    private readonly RoomManagementPage _roomPage;

    // DI автоматично "вставить" сюди потрібні об'єкти
    public UserDeletionSteps(BrowserDriver browserDriver)
    {
        _roomPage = new RoomManagementPage(browserDriver.Page);
    }

    // --- КРОКИ ДЛЯ СЦЕНАРІЇВ ---

    [Given(@"I am logged in as an ""(.*)""")]
    public void GivenIAmLoggedInAsAn(string role)
    {
        // Тут має бути логіка логіну (ймовірно, вона вже є в інших Step-файлах)
        // Наприклад: _loginPage.Login(user, pass);
        // Поки що можна пропустити, якщо припустити, що ми вже залогінені
    }

    [Given(@"I navigate to the ""(.*)"" management page")]
    public async Task GivenINavigateToTheRoomManagementPage(string roomName)
    {
        // Логіка переходу на сторінку кімнати
        // await _page.GotoAsync("http://localhost:8081/room/123");
    }

    [Given(@"user ""(.*)"" is visible in the list")]
    public async Task GivenUserIsVisibleInTheList(string userName)
    {
        (await _roomPage.IsUserVisible(userName)).Should().BeTrue();
    }

    [When(@"I click the delete button for ""(.*)""")]
    public async Task WhenIClickTheDeleteButtonFor(string userName)
    {
        await _roomPage.DeleteUser(userName);
    }

    [Then(@"user ""(.*)"" should no longer be visible in the list")]
    public async Task ThenUserShouldNoLongerBeVisibleInTheList(string userName)
    {
        (await _roomPage.IsUserVisible(userName)).Should().BeFalse();
    }

    [Then(@"the delete button for ""(.*)"" should be disabled")]
    public async Task ThenTheDeleteButtonForShouldBeDisabled(string userName)
    {
        (await _roomPage.IsDeleteButtonDisabled(userName)).Should().BeTrue();
    }
}