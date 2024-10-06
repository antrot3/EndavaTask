<h1>Endava Task</h1>
</br>
This project is an application built to demonstrate a task for Endava. It features a .NET solution and includes a simple database that is automatically created on startup.
<h2>How to Register</h2>
Use register endpoint and after succesful registration go to login and create JWT token with it. </br>
After that past that token in Jwt and then user can use other endpoints <br>
Note only Administrators can edit and create articles, 
All Rolese can view by id and get all articles
<h2>Project Structure</h2>
</br>
The main project files, including the solution (.sln) file, can be found in the Endava directory.
The project uses Entity Framework to manage the database, which is created and initialized automatically when the application starts.
Getting Started
<h2>Prerequisites</h2>
.NET Core 8 </br>
A code editor such as Visual Studio or Visual Studio Code </br>
SQL Server (if using a local database, though other DB providers may be supported with changes in the connection string)
Running the Application
Clone the repository:
<h2>Database</h2>
On startup, the application will automatically create the database if it does not already exist. This process is handled using Entity Framework migrations.
</br>
The database will be created locally with default configuration, but you can modify the connection string in the appsettings.json file to point to a different SQL server or database.

<h2>Features</h2>
Automatic database creation on application startup</br>
REST API for managing data</br>
Entity Framework for data management and migrations</br>
