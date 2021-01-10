# DevCamper
DevCamper is a little webapp for discovering Development Bootcamps and their courses. Basically, there are two types of user: Normal, Publisher.
1. A Publisher user will be able to publish bootcamps (only one for account), create courses and manage both of them.
2. A Normal user will be able to explore all the info on the website, write reviews and so on.

## What do you need
You will need at least: 
- Visual Studio 2017/2019 with the last .NET Core SDK
- SQL Server Express/Developer installed and active
- SQL Server Management Studio 15.0

## How To Use
- Open ".sln" file with Visual Studio
- Right click on the project and select "Manage User Secret" and paste these lines:

  > "ConnectionStrings": {
    "DefaultConnection": "Server={SERVER_CONNECTION_STRING};Database=DevCamper;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
- Replace the "{SERVER_CONNECTION_STRING}" tag with your SQL Server connection string
- Go to Tools -> Manage Nuget Packages -> Console Nuget Packages
- Once the PowerShell Console will be open, paste this line:

  > Update-Database
  
  This will ensure the creation of the database with all the migrations applied
  
## SQL Data Test
Once the database has been created, execute these two query:
  > INSERT INTO dbo.Careers VALUES ('Web Development'), ('UI/UX'), ('Mobile Development'), ('Business'), ('Data Science');
  
  > INSERT INTO dbo.Skills VALUES ('Beginner'), ('Intermediate');


#### Run the project and try it out.

HTML Template by: https://github.com/bradtraversy
