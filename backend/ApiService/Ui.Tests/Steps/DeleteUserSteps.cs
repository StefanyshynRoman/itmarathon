using System;
using System.IO;
using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using OpenQA.Selenium;
using RestSharp;
using TechTalk.SpecFlow;
using Ui.Tests.Helpers;
using Ui.Tests.Pages;

namespace Ui.Tests.Steps
{
    [Binding]
    public class DeleteUserSteps
    {
        private readonly IWebDriver _driver;
        private readonly RoomPage _roomPage;
        private string _adminCode;
        private string _baseUrl;
        private string _apiBaseUrl;

        public DeleteUserSteps()
        {
            _driver = DriverFactory.GetDriver();
            _roomPage = new RoomPage(_driver);

            var json = File.ReadAllText("appsettings.json");
            var cfg = JsonSerializer.Deserialize<JsonElement>(json);

            _baseUrl = cfg.GetProperty("baseUrl").GetString();
            _apiBaseUrl = cfg.GetProperty("apiBaseUrl").GetString();
        }

        [Given(@"a room exists with admin and a user")]
        public void CreateRoom()
        {
            var client = new RestClient(_apiBaseUrl);

            // 1: create room
            var createRoomReq = new RestRequest("/api/rooms", Method.Post);
            createRoomReq.AddJsonBody(new
            {
                name = "Test Room",
                description = "",
                giftExchangeDate = DateTime.UtcNow.AddDays(5),
                giftMaximumBudget = 1000
            });
            var r1 = client.Execute(createRoomReq);

            var room = JsonDocument.Parse(r1.Content);
            var code = room.RootElement.GetProperty("invitationCode").GetString();

            // 2: add admin
            var addAdminReq = new RestRequest("/api/users", Method.Post);
            addAdminReq.AddQueryParameter("roomCode", code);
            addAdminReq.AddJsonBody(new
            {
                firstName = "Admin",
                lastName = "Test"
            });

            var r2 = client.Execute(addAdminReq);
            var admin = JsonDocument.Parse(r2.Content);
            _adminCode = admin.RootElement.GetProperty("userCode").GetString();

            // 3: add user
            var addUserReq = new RestRequest("/api/users", Method.Post);
            addUserReq.AddQueryParameter("roomCode", code);
            addUserReq.AddJsonBody(new
            {
                firstName = "User",
                lastName = "Delete"
            });
            client.Execute(addUserReq);
        }

        [Given(@"admin opens the room page")]
        public void OpenRoom()
        {
            _roomPage.Open(_baseUrl, _adminCode);
        }

        [When(@"admin deletes the user")]
        public void DeleteUser()
        {
            var row = _roomPage.FindUserRow("User Delete");
            Assert.NotNull(row);
            _roomPage.ClickDelete(row);
        }

        [Then(@"user disappears from participants list")]
        public void CheckRemoved()
        {
            System.Threading.Thread.Sleep(2000);
            var row = _roomPage.FindUserRow("User Delete");
            Assert.IsNull(row);
        }

        [AfterScenario]
        public void Cleanup()
        {
            DriverFactory.QuitDriver();
        }
    }
}
