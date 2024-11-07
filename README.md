# Library Management Backend

The backend for a library management system built with **ASP.NET Core** (C#). It handles user authentication, data storage, and role-based access. The backend is connected to a SQL database.

## Tech Stack
- **ASP.NET Core (C#)**
- **JWT**: For secure user authentication
- **bcrypt**: For password hashing

## Features
- User authentication with JWT
- Role-based access (admin, manager, user)
- CRUD operations for books and borrowers
- Debt management for borrowers

## Frontend

The frontend for this project can be found here:  
[Library Management Frontend](https://github.com/gabwowce/LibraryManagement-frontend)

## Installation

To run this project locally, clone the repository and install the dependencies:

```bash
git clone https://github.com/gabwowce/LibraryManagement-backend.git
cd LibraryManagement-backend
dotnet restore
dotnet run
