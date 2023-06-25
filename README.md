# EKGApp
# Readme: C# EKGAPP
This readme file provides instructions for setting up and configuring your C# console app. Before running the application, make sure to follow these steps:
**Prerequisites**
- Installed SQL Server

**Installation**
1. Clone or download the project from the repository.
2. Open the project in your preferred Integrated Development Environment (IDE), such as Visual Studio.

**Configuring the Connection String**
To connect your C# console app to your SQL database, you need to update the connection string in the `DBContextClass.cs` file located in the Data Access Layer folder. Follow these steps:
1. Open the project in your IDE.
2. Locate the `DBContextClass.cs` file in the Data Access Layer folder inside the Data C# class library.
3. Open the file and find the connection string declaration. 
4. Replace "Data Source=*****************" with your actual SQL database connection string. Make sure to include the necessary details such as the server name, database name, username, password and security settings.
   
**Database Migration**
Before running the C# console app, you need to perform the necessary database migration using Windows PowerShell or another console app. Follow these steps:
1. Open Windows PowerShell.
2. Navigate to the project directory using the `cd` command. For example:
   cd C:\Path\To\Your\Project
3. Run the migration command using the .NET CLI towards an empty database or create a new:
  dotnet ef database update
This command will apply any pending migrations and create/update the corresponding database schema.

**Running the Application**
After completing the steps above, you are ready to run your C# console app. When you run the application it will add 10000 fake patients to your database if it is empty. You can use the methods provided in the DBController class to empty it or add fewer patients. 
Follow the readme instructions and interact with the console app as intended.
Please note that this readme assumes a basic understanding of C# and the .NET ecosystem. If you encounter any issues or have further questions, refer to the official documentation or seek assistance from the community.
