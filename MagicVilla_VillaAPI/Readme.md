# MagicVilla_VillaAPI

MagicVilla_VillaAPI is a RESTful API built with ASP.NET Core that allows users to manage villa information in a magical resort setting. The API supports CRUD operations for villas, providing endpoints for creating, retrieving, updating, and deleting villa records.

## Table of Contents

- [Features](#features)
- [Installation](#installation)
- [Usage](#usage)
- [API Endpoints](#api-endpoints)
- [Technologies Used](#technologies-used)
- [Project Structure](#project-structure)

## Features

- **Get All Villas**: Retrieve a list of all villas.
- **Get Villa by ID**: Retrieve details of a specific villa by ID.
- **Create Villas**: Add new villas to the database, with feedback on existing entries.
- **Update Villa**: Update the details of an existing villa.
- **Delete Villa**: Remove a villa from the database.
- **Patch Villa**: Update specific fields of a villa using JSON Patch.

## Installation

Follow these steps to set up the project locally:

1. **Clone the repository:**

   ```bash
   git clone https://github.com/your-username/MagicVilla_VillaAPI.git
   ```
2. **Navigate to the project directory:**
	```bash
	cd MagicVilla_VillaAPI
	 ```
3. **Restore the NuGet packages:**			
    ```bash
	dotnet restore
	```
4. **Update your database connection string in appsettings.json:**
	 ```json
	 "ConnectionStrings": {"DefaultConnection": "Server=your-server;Database=MagicVilla;Trusted_Connection=True;MultipleActiveResultSets=true"}
	 ```
5. **Apply the migrations to create the database schema:**
	 ```bash
	 dotnet ef database update
	 ```
6. **Run the Application**
	 ```bash
	 dotnet run
	 ```

## Usage

To use the API, you can utilize tools like Postman or curl. Here are some basic examples of how to interact with the API:

1. **Get All Villas**
	```
    GET /api/VillaAPI/GetAllVilla
	curl -X GET https://localhost:5001/api/VillaAPI/GetAllVilla
	``` 
2. **Create Villas**
	```
	POST /api/VillaAPI/CreateVillas
	curl -X POST https://localhost:5001/api/VillaAPI/CreateVillas -H "Content-Type: application/json" -d "[{\"name\":\"Ocean View Villa\",\"details\":\"A beautiful villa with ocean views.\",\"amenity\":\"Pool, Spa\",\"imageUrl\":\"http://example.com/oceanview.jpg\",\"occupancy\":4,\"rate\":500.0,\"sqft\":1500}]"
	Example JSON Body:
	[
	  {
		"name": "Ocean View Villa",
		"details": "A beautiful villa with ocean views.",
		"amenity": "Pool, Spa",
		"imageUrl": "http://example.com/oceanview.jpg",
		"occupancy": 4,
		"rate": 500.0,
		"sqft": 1500
	  }
	]
	``` 

## API Endpoints

| Method   | Endpoint                                 | Description                             |
| -------- | ---------------------------------------- | --------------------------------------- |
| `GET`    | `/api/VillaAPI/GetAllVilla`              | Get all villas.                         |
| `GET`    | `/api/VillaAPI/GetVillaById/{id}`        | Get a villa by ID.                      |
| `POST`   | `/api/VillaAPI/CreateVillas`             | Create new villas.                      |
| `PUT`    | `/api/VillaAPI/UpdateVilla`              | Update an existing villa.               |
| `PATCH`  | `/api/VillaAPI/UpdatePartialVilla/{id}`  | Partially update a villa.               |
| `DELETE` | `/api/VillaAPI/DeleteVilla/{id}`         | Delete a villa.                         |

## Technologies Used

- **ASP.NET Core**: A modern, cross-platform framework for building web applications.
- **Entity Framework Core**: An ORM for .NET to work with databases using .NET objects.
- **SQL Server**: Relational database management system for storing villa data.

## Project Structure

MagicVilla_VillaAPI/
│
├── Controllers/
│   └── VillaAPIController.cs
│
├── Data/
│   └── ApplicationDbContext.cs
│
├── Logging/
│   ├── ILogging.cs
│   └── Logging.cs
│
├── Models/
│   ├── Villa.cs
│   └── Dto/
│       └── VillaDto.cs
│
├── Repositories/
│   ├── IVillaRepository.cs
│   └── VillaRepository.cs
│
├── Migrations/
│   └── [Timestamp]_[MigrationName].cs
│
├── Properties/
│   └── launchSettings.json
│
├── appsettings.json
├── Program.cs
├── Startup.cs
├── MagicVilla_VillaAPI.csproj
└── README.md
