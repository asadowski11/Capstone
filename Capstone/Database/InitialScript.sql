DROP TABLE IF EXISTS "TSettingOptions";
DROP TABLE IF EXISTS "TSettings";
DROP TABLE IF EXISTS "TSearchEngines";
DROP TABLE IF EXISTS "TSearchableWebsites";
DROP TABLE IF EXISTS "TAlarmDates";
DROP TABLE IF EXISTS "TAlarms";
DROP TABLE IF EXISTS "TReminderDates";
DROP TABLE IF EXISTS "TReminders";
DROP TABLE IF EXISTS "TMapProviders";
DROP TABLE IF EXISTS "TMapProvidersURLS";
DROP TABLE IF EXISTS "TMapProviderAccessTypes";
DROP TABLE IF EXISTS "TMapProvidersURLParts";
DROP TABLE IF EXISTS "TVoiceMemos";
DROP TABLE IF EXISTS "TUserInfos";
DROP TABLE IF EXISTS "TCaches";
DROP TABLE IF EXISTS "TBobResponses";
DROP TABLE IF EXISTS "TBobResponseInputs";
DROP TABLE IF EXISTS "TWeatherProviderURLS";
DROP TABLE IF EXISTS "TWeatherProviderURLParts";
DROP TABLE IF EXISTS "TWeatherProviderAccessTypes";
DROP TABLE IF EXISTS "TWeatherProviders";
DROP TABLE IF EXISTS "TJokes";

CREATE TABLE "TSettings"
(
	 "settingID"			INTEGER PRIMARY KEY NOT NULL
	,"settingDisplayName"	VARCHAR(255)	NOT NULL
);

CREATE TABLE "TSettingOptions"
(
	 "settingOptionID"	    INTEGER 		PRIMARY KEY NOT NULL
	,"optionDisplayName"    VARCHAR(255)	NOT NULL
	,"isSelected"           BIT				NOT NULL
	,"settingID"            INTEGER			NOT NULL
	,FOREIGN KEY ( settingID ) REFERENCES TSettings ( settingID )
);

CREATE TABLE "TSearchEngines"
(
	 "searchEngineID"			INTEGER			PRIMARY KEY	NOT NULL
	,"searchEngineName"			NVARCHAR(255)	NOT NULL
	,"searchEngineBaseURL"		NVARCHAR(255)	NOT NULL
	,"searchEngineQueryString"  NVARCHAR(255)	NOT NULL
);

CREATE TABLE "TSearchableWebsites"
(						
	 "searchableWebsitesID"				INTEGER			PRIMARY KEY NOT NULL
	,"searchableWebsiteName"			NVARCHAR(255)	NOT NULL
	,"searchableWebsiteBaseURL"			NVARCHAR(255)	NOT NULL
	,"searchableWebsiteQueryString"	    NVARCHAR(255)	NOT NULL
);

CREATE TABLE "TAlarms"
(						
	 "alarmID"	    INTEGER			PRIMARY KEY NOT NULL
	,"alarmTime"    TIME			NOT NULL
	,"alarmTitle"   NVARCHAR(255)	NOT NULL
	,"isDeleted"    BIT				NOT NULL
	,"isExpired"	BIT				NOT NULL
);

CREATE TABLE "TAlarmDates"
(						
	 "alarmDateID"  INTEGER			PRIMARY KEY NOT NULL
	,"alarmID"	    INTEGER			NOT NULL
	,"alarmDate"	DATE			NOT NULL
	,FOREIGN KEY ( alarmID ) REFERENCES TAlarms ( alarmID )
);

CREATE TABLE "TReminders"
(
	 "reminderID"		   INTEGER			PRIMARY KEY NOT NULL
	,"reminderTitle"       NVARCHAR(255)	NOT NULL
	,"reminderTime"	       TIME			    NOT NULL
	,"reminderDescription" NVARCHAR(1024)   NOT NULL
	,"isDeleted"		   BIT				NOT NULL
	,"isExpired"		   BIT				NOT NULL
);

CREATE TABLE "TReminderDates"
(
	 "reminderDateID"   INTEGER			PRIMARY KEY NOT NULL
	,"reminderID"	    INTEGER			NOT NULL
	,"reminderDate"     DATE			NOT NULL
	,FOREIGN KEY ( reminderID ) REFERENCES TReminders ( reminderID )
);


CREATE TABLE "TMapProviders"
(
	 "mapProviderID"    INTEGER			PRIMARY KEY NOT NULL
	,"mapProviderName"  NVARCHAR(255)	NOT NULL
);


CREATE TABLE "TMapProvidersURLS"
(
	 "mapProviderURLID" INTEGER			PRIMARY KEY NOT NULL
	,"mapProviderID"    INTEGER			NOT NULL
	,"mapProviderURL"   NVARCHAR(255)	NOT NULL
	,FOREIGN KEY ( mapProviderID ) REFERENCES TMapProviders ( mapProviderID )
);

CREATE TABLE "TMapProviderAccessTypes"
(
	 "mapProviderAccessTypeID"  INTEGER			PRIMARY KEY NOT NULL
	,"mapProviderID"            INTEGER			NOT NULL
	,"mapProviderAccessType"    NVARCHAR(255)	NOT NULL
	,FOREIGN KEY ( mapProviderID ) REFERENCES TMapProviders ( mapProviderID )			
);

CREATE TABLE "TMapProvidersURLParts"
(
	 "mapProviderURLPartID"     INTEGER			PRIMARY KEY NOT NULL
	,"mapProviderID"            INTEGER			NOT NULL
	,"mapProviderURLPartType"   VARCHAR(255)	NOT NULL
	,"mapProviderURLPartURL"    NVARCHAR(255)	NOT NULL
	,FOREIGN KEY ( mapProviderID ) REFERENCES TMapProviders ( mapProviderID )
);

CREATE TABLE "TVoiceMemos"
(
	 "voiceMemoID"          INTEGER			PRIMARY KEY NOT NULL
	,"fileName"         	VARCHAR(255)	NOT NULL
	,"displayName"          NVARCHAR(255)	NOT NULL
	,"recordingDuration"    INTEGER			NOT NULL
	,"filePath"         	NVARCHAR(255)	NOT NULL
	,"recordDate"           DATE			NOT NULL
	,"recordTime"           TIME			NOT NULL
);

CREATE TABLE "TUserInfos"
(
	 "userInfoID"           INTEGER			PRIMARY KEY NOT NULL
	,"userInfoTypeName"     VARCHAR(255)	NOT NULL
	,"userInfoTypeValue"    NVARCHAR(255)	NOT NULL
);


CREATE TABLE "TCaches"
(
	 "cacheKey"		        INTEGER			PRIMARY KEY NOT NULL
	,"cacheContents"	    NVARCHAR(255)	NOT NULL
	,"cacheExpirationDate"  DATETIME		NOT NULL
);

CREATE TABLE "TBobResponses"
(
	 "bobResponseID"        INTEGER			PRIMARY KEY NOT NULL
	,"bobResponseString"    NVARCHAR(255)	NOT NULL
);

CREATE TABLE "TBobResponseInputs"
(
	 "bobResponseInputID"   INTEGER			PRIMARY KEY NOT NULL
	,"bobResponseID"	    INTEGER			NOT NULL
	,FOREIGN KEY ( bobResponseID ) REFERENCES TBobResponses( bobResponseID )
);

CREATE TABLE "TWeatherProviders"
(
	 "weatherProviderID"    INTEGER			PRIMARY KEY NOT NULL
	,"weatherProviderName"  NVARCHAR(255)	NOT NULL
);

CREATE TABLE "TWeatherProviderURLS"
(
	 "weatherProviderURLID" INTEGER			PRIMARY KEY NOT NULL
	,"weatherProviderID"	INTEGER			NOT NULL
	,"weatherProviderURL"	NVARCHAR(255)	NOT NULL
	,FOREIGN KEY ( weatherProviderID ) REFERENCES TWeatherProviders ( weatherProviderID )
);


CREATE TABLE "TWeatherProviderURLParts"
(
	 "weatherProviderURLPartID"			INTEGER			PRIMARY KEY NOT NULL
	,"weatherProviderID"				INTEGER			NOT NULL
	,"weatherProviderURLPartURLString"  NVARCHAR(255)	NOT NULL
	,FOREIGN KEY ( weatherProviderID ) REFERENCES TWeatherProviders ( weatherProviderID )
);


CREATE TABLE "TWeatherProviderAccessTypes"
(
	 "weatherProviderAccessTypeID"  INTEGER			PRIMARY KEY NOT NULL
	,"weatherProviderID"            INTEGER			NOT NULL
	,"weatherProviderAccessType"    NVARCHAR(255)	NOT NULL
	,FOREIGN KEY ( weatherProviderID ) REFERENCES TWeatherProviders ( weatherProviderID )
);

CREATE TABLE "TJokes" (
	 "jokeID"	INTEGER PRIMARY KEY NOT NULL
	,"jokeText"	TEXT	NOT NULL	UNIQUE
);

/*INSERT STATEMENTS*/
INSERT INTO "TWeatherProviders" ("weatherProviderID", "weatherProviderName") VALUES ('1', 'National Weather Service');
INSERT INTO "TWeatherProviderAccessTypes" ("weatherProviderAccessTypeID", "weatherProviderID", "weatherProviderAccessType") VALUES ('1', '1', 'CURL');
INSERT INTO "TWeatherProviderURLParts" ("weatherProviderURLPartID", "weatherProviderID", "weatherProviderURLPartURLString") VALUES ('1', '1', '/gridpoints/:office/:gridX,:gridY/forecast');
INSERT INTO "TWeatherProviderURLParts" ("weatherProviderURLPartID", "weatherProviderID", "weatherProviderURLPartURLString") VALUES ('2', '1', '/points/:latitude,:longitude');
INSERT INTO "TWeatherProviderURLS" ("weatherProviderURLID", "weatherProviderID", "weatherProviderURL") VALUES ('1', '1', 'https://api.weather.gov');

INSERT INTO TSettings(settingDisplayName)
VALUES				 ("Search Engine")
,					 ("Voice Activation")
,					 ("_FirstTimeSetupPassed") -- it starts with an underscore, which means it should not display on a UI
,					 ("_ToldUserHowToUseBob");

INSERT INTO TSettingOptions(settingID, optionDisplayName, isSelected)
							-- search engine
VALUES					   (1, "Google", 0)
,						   (1, "Duck Duck Go", 0)
,						   (1, "Bing", 0)
						   -- voice activation
,						   (2, "Enabled", 0)
,						   (2, "Disabled", 0)
						   -- passed first time setup
,						   (3, "true", 0)
,						   (3, "false", 1)
						   -- has told the user how to use bob
,						   (4, "true", 0)
,						   (4, "false", 1);

INSERT INTO "TMapProviders" ("mapProviderID", "mapProviderName") VALUES ('1', 'Google');
INSERT INTO "TMapProviderAccessTypes" ("mapProviderAccessTypeID", "mapProviderID", "mapProviderAccessType") VALUES ('1', '1', 'EXTERNAL_URL');
INSERT INTO "TMapProvidersURLParts" ("mapProviderURLPartID", "mapProviderID", "mapProviderURLPartType", "mapProviderURLPartURL") VALUES ('1', '1', '/gridpoints/:office/:gridX,:gridY/forecast','');
INSERT INTO "TMapProvidersURLS" ("mapProviderURLID", "mapProviderID", "mapProviderURL") VALUES ('1', '1', 'https://api.weather.gov');

-- jokes
INSERT INTO TJokes(jokeText)
VALUES			  ('What do you get when you put a vest on an alligator? An investigator.')
,				  ('If Kermit the frog did something illegal, would he be Kermitting a crime?')
,				  ('A man walked into a bar. The second man ducked.')
,				  ('Please, let me out of here. This isn''t a joke!')
,				  ('I heard the brooms won the basketball game. It was a clean sweep.')
,				  ('What do you get when you drop a piano down a mineshaft? A-flat-miner.')
,				  ('What is a Greek mathematician''s favorite programming language? π-thon.')
,				  ('How do you make Bud Light? Put him on a diet!')
,				  ('How do you make Bud weiser? Send him to college!')
,				  ('You look good today.')
,				  ('What do you call a guy who''s hung up on a wall? Art!')
,				  ('What''s brown and sticky? A stick.')
,				  ('How do you make a tissue dance? Put a little boogie in it!')
,				  ('How do you make holy water? Boil the hell out of it.')
,				  ('What do you call a student that got C''s all the way through medical school? Hopefully not your doctor!')
,				  ('Did you hear about the restaurant on the moon? Great food, no atmosphere.')
,				  ('Did you hear about the baguette at the zoo? It was bread in captivity.')
,				  ('Don''t trust stairs. They''ll always let you down and they''re always up to something.')
,				  ('Why did the picture go to prison? Because it was framed.')
,				  ('Why didn''t the bike cross the road? Because it was two tired.')
,				  ('What do you call a pig that does karate? A pork chop.');
