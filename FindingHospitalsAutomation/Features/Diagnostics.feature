Feature: Extract Diagnostics Top Cities

  Scenario: Extract top city names from the Lab Tests page
    Given I navigate to the diagnostics page from the homepage
    Then I extract the top city names and save them
