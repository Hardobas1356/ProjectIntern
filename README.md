# ProjectIntern

A modern, containerized web application designed for administrators to smoothly schedule, manage, and assign technical topics and presentation dates to interns.

## Description

ProjectIntern provides unified administrative dashboard. Admins can seamlessly map out curriculum calendars, create specific technical topics, and assign them directly to interns. The system is built using a decoupled, multi-project architecture with **ASP.NET Core (targeting .NET 10)** on the frontend/API layer and a persistent **PostgreSQL 15** database on the backend. Database updates are handled via an automated Entity Framework Core migration runner container, ensuring that database schemas are always kept safely in sync with the source code upon service initialization.

## Getting Started

### Dependencies

* **Operating System:** Windows 10/11, macOS, or Linux.
* **Docker Engine:** Docker Desktop installed and running (with Linux Containers selected if running on Windows).
* **Git:** For cloning and version control management.
* *Note: You do not need the .NET 10 SDK or PostgreSQL installed physically on your local computer; everything compiles and runs entirely inside isolated Docker containers framework.*

### Installing

Clone the project repository from GitHub to your target working directory:
  ```
   git clone [https://github.com/Hardobas1356/ProjectIntern.git](https://github.com/Hardobas1356/ProjectIntern.git)
   cd ProjectIntern
  ```
Create a secure environment configuration file named .env directly in the root directory (the same folder containing your docker-compose.yml). This file is hidden by .gitignore and keeps your private credentials secure:
    
    touch .env

Open the .env file in your preferred text editor and declare your secure PostgreSQL master password:

    DB_PASSWORD=YourHighlySecureDatabasePassword123!

Executing program

Follow these step-by-step instructions to compile your solution assemblies, automatically build the tracking schema, and launch the platform:

Launch your command-line terminal (PowerShell, Command Prompt, or Bash) in the project root directory.

Build your container layout clean and bring your microservices ecosystem fully online in detached mode:

    docker compose up -d --build

Docker will automatically sequence the execution stack:

  * **db-server** boots up and configures PostgreSQL.

  * **db-migrator** runs a localized NuGet package restore, executes dotnet ef database update, applies your table migrations, and gracefully terminates upon completion.

  * **web-app** detects the successful migration pass and fires up the ASP.NET Core hosting framework.

Access the live, running web application platform via your internet browser at:

    http://localhost:8080

To shut down the application stack while keeping all saved database information intact inside your local persistent storage volume:

    docker compose down
