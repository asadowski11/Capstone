-----------------------------------------
-- DATABASE VERSION 2
-----------------------------------------

-- adding more specific websites to search
CREATE UNIQUE INDEX website_name ON TSearchableWebsites(searchableWebsiteName); -- make sure website name is now unique
ALTER TABLE TSearchableWebsites ADD COLUMN spaceReplacement TEXT DEFAULT '+'; -- used to represent what each website replaces spaces with
ALTER TABLE TAlarms ADD COLUMN isSet BIT DEFAULT 0;
ALTER TABLE TReminders ADD COLUMN isSet BIT DEFAULT 0;

INSERT OR IGNORE INTO TSearchableWebsites(searchableWebsiteName, searchableWebsiteBaseURL, searchableWebsiteQueryString, spaceReplacement)
VALUES						   ('Wikipedia', 'https://en.wikipedia.org/wiki/', '', '_')
							  ,('Target', 'https://www.target.com/', 's?searchTerm=', '+')
							  ,('GameStop', 'https://www.gamestop.com/', 'search/?q=', '+')
							  ,('Reddit', 'https://www.reddit.com/', 'search/?q=', '%20')
							  ,('Twitch', 'https://www.twitch.tv/', 'search?term=', '%20')
							  ,('Ebay', 'https://www.ebay.com/', 'sch/i.html?_nkw=', '+')
							  ,('Apple', 'https://www.apple.com/', '/search/', '-')
							  ,('StackOverflow', 'https://stackoverflow.com/', 'search?q=', '+')
							  ,('Sephora', 'https://www.sephora.com/', 'search?keyword=', '%20')
							  ,('Twitter', 'https://twitter.com/', 'search?q=', '%20')
							  ,('Pinterest', 'https://www.pinterest.com/', 'search/pins/?q=', '%20')
							  ,('Facebook', 'https://www.facebook.com/', 'search/top/?q=', '%20');

-- add yahoo to the list of search engines because we forgot about it
INSERT INTO TSearchEngines(searchEngineName, searchEngineBaseURL, searchEngineQueryString)
VALUES					  ('Yahoo', 'https://search.yahoo.com/', 'search?p=');
INSERT INTO TSettingOptions(optionDisplayName, isSelected, settingID)
VALUES					   ('Yahoo', 0, 1);

-- create a table for the database version. The database version does not have to match with the application release version. It is used to keep track of when database changes need to be made
DROP TABLE IF EXISTS "TVersion";
CREATE TABLE TVersion(
	 versionID INTEGER PRIMARY KEY
	,versionName TEXT NOT NULL UNIQUE
);

INSERT INTO TVersion(versionName)
VALUES				 ('2');
