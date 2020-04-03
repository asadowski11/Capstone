DROP TABLE IF EXISTS "TSettings";
DROP TABLE IF EXISTS "TSettingOptions";
DROP TABLE IF EXISTS "TSearchEngines";
DROP TABLE IF EXISTS "TSearchableWebsites";
DROP TABLE IF EXISTS "TAlarms";
DROP TABLE IF EXISTS "TAlarmDates";
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
DROP TABLE IF EXISTS "TWeatherProviders";
DROP TABLE IF EXISTS "TWeatherProviderURLS";
DROP TABLE IF EXISTS "TWeatherProviderURLParts";
DROP TABLE IF EXISTS "TWeatherProviderAccessTypes";

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
);

CREATE TABLE "TAlarmDates"
(						
	 "alarmDateID"  INTEGER			PRIMARY KEY NOT NULL
	,"alarmID"	    INTEGER			NOT NULL
	,"alarmDate"	DATETIME		NOT NULL
    ,FOREIGN KEY ( alarmID ) REFERENCES TAlarms ( alarmID )
);

CREATE TABLE "TReminders"
(
	 "reminderID"		   INTEGER			PRIMARY KEY NOT NULL
	,"reminderTitle"       NVARCHAR(255)	NOT NULL
	,"reminderTime"	       TIME			    NOT NULL
	,"reminderDescription" NVARCHAR(1024)   NOT NULL
	,"isDeleted"		   BIT				NOT NULL
);

CREATE TABLE "TReminderDates"
(
	 "reminderDateID"   INTEGER			PRIMARY KEY NOT NULL
	,"reminderID"	    INTEGER			NOT NULL
	,"reminderDate"     DATETIME		NOT NULL
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