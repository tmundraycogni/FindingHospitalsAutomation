@accessibility @parallel
Feature: Accessibility scan for the Hospital Search page

  Scenario: Perform Axe accessibility scan on the hospital search results page
    Given I open the hospital search page for accessibility testing
    Then I perform an accessibility scan on the hospital search results page
