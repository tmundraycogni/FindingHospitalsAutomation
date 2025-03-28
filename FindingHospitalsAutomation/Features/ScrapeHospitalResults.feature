Feature: Scrape Hospital Results

  Scenario: Filter top-rated 24x7 hospitals from the results page
    Given I navigate to the hospital results page from config
    When I process the top 50 hospitals
