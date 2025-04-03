@parallel
Feature: Validate Corporate Wellness Form
  Ensure that an error is displayed when the Corporate Wellness form is submitted with invalid details.

  Scenario: Submit Corporate Wellness form with invalid inputs
    Given I navigate to the Corporate Wellness form
    When I submit the form with invalid details
    Then a validation error should be shown
