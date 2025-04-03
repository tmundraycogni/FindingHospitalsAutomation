@scraper
Feature: Scrape Hospital Results

  Scenario: Filter top-rated 24x7 hospitals using parallel scraping
    Given I navigate to the hospital results page from config
    When I extract and scrape the top 50 hospital links in parallel
    Then the valid hospital data should be saved to the CSV