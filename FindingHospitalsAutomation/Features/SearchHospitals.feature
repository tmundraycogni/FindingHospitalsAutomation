Feature: Search Hospitals
  As a user
  I want to search for hospitals from the homepage
  So that I can view hospital results and return to the homepage

  Scenario: Perform a hospital search via homepage
    Given I open the hospital search page
    Then I should be taken to the hospital search results page
    And I should be navigated back to the homepage

