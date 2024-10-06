#Endava Task
This project is an application built to demonstrate a task for Endava. It features a .NET solution and includes a simple database that is automatically created on startup.

##Project Structure
The main project files, including the solution (.sln) file, can be found in the Endava directory.
The project uses Entity Framework to manage the database, which is created and initialized automatically when the application starts.
Getting Started
Prerequisites
.NET Core 8
A code editor such as Visual Studio or Visual Studio Code
SQL Server (if using a local database, though other DB providers may be supported with changes in the connection string)
Running the Application
Clone the repository:
##Database
On startup, the application will automatically create the database if it does not already exist. This process is handled using Entity Framework migrations.

The database will be created locally with default configuration, but you can modify the connection string in the appsettings.json file to point to a different SQL server or database.

Features
Automatic database creation on application startup
REST API for managing data
Entity Framework for data management and migrations

This README.md assumes the project uses Entity Framework for database management and contains a REST API. Let me know if more specifics need to be included!
