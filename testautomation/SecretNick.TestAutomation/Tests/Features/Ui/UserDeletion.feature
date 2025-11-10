@ui @UserDeletion
Feature: User Deletion

  As an Admin
  I want to delete users from a room
  So that I can manage room participants

@positive
Scenario: Admin successfully deletes another user
  Given I am logged in as an "Admin"
  And I navigate to the "Test Room" management page
  And user "TestUser" is visible in the list
  When I click the delete button for "TestUser"
  Then user "TestUser" should no longer be visible in the list

@negative
Scenario: Admin cannot delete himself
  Given I am logged in as an "Admin"
  And I navigate to the "Test Room" management page
  Then the delete button for "Admin" should be disableduser "TestUser" should not be visible in the user list