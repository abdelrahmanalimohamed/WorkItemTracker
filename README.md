# Work Item Tracker

A full-stack implementation of the **Craft Crew .NET / Angular hiring assessment**.

The solution consists of an ASP.NET Core Web API backend and an Angular frontend for managing work items.

## Solution Structure

```text
WorkItemTracker/
│
├── WorkItemTracker.Backend/
│   ├── src/
│   ├── tests/
│   └── README.md
│
├── WorkItemTracker.Frontend/
│   ├── src/
│   └── README.md
│
└── README.md
```

## Architecture

The backend is intentionally implemented as a **simple monolithic ASP.NET Core application**.

Clean Architecture and microservices were not introduced in order to keep the assessment focused on the required functionality and avoid unnecessary complexity.

The frontend and backend remain separate applications and communicate through a REST API.

## Technology Stack

### Backend

* ASP.NET Core 8 Web API
* Entity Framework Core 8
* SQLite
* xUnit
* Swagger/OpenAPI

### Frontend

* Angular
* TypeScript

See the individual README files for detailed implementation and setup instructions.

## Main Features

* Create work items
* Search and filter work items
* Server-side pagination
* Work item status transitions
* Client-side and server-side validation
* API error handling
* Automated backend tests

## Running the Application

### Backend

```bash
cd WorkItemTracker.Backend
dotnet restore
dotnet run --project src/WorkItemTracker.API
```

Backend:

```text
http://localhost:5080
```

Swagger:

```text
http://localhost:5080/swagger
```

### Frontend

Open another terminal:

```bash
cd WorkItemTracker.Frontend
npm install
npm start
```

Frontend:

```text
http://localhost:4200
```

## Work Item Workflow

```text
Todo → InProgress → Done
```

Only the defined transitions are allowed.

## Documentation

* [Backend README](./WorkItemTracker.Backend/README.md)
* [Frontend README](./WorkItemTracker.Frontend/README.md)
