Feature: Delete user
  Scenario: Admin deletes a normal user
    Given a room exists with admin and a user
    And admin opens the room page
    When admin deletes the user
    Then user disappears from participants list