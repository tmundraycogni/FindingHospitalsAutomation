Feature: Search for hospitals in a specific city
  As a user
  I want to search for hospitals in Bangalore
  So that I can view available hospital options

  Scenario: Perform a hospital search via homepage
    Given I open the hospital search page
    Then I should be taken to the hospital search results page
    And I should be navigated back to the homepage
