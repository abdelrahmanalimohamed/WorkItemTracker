# Work Item Tracker

A full-stack implementation of the **Craft Crew .NET / Angular hiring assessment**.

The solution consists of an ASP.NET Core backend API and an Angular frontend application for creating, searching, filtering, paginating, and updating work items.

## Solution Structure

```text
WorkItemTracker/
│
├── WorkItemTracker.Backend/
│   ├── src/
│   │   ├── WorkItemTracker.API/
│   │   ├── WorkItemTracker.Application/
│   │   ├── WorkItemTracker.Domain/
│   │   └── WorkItemTracker.Infrastructure/
│   │
│   ├── tests/
│   │   └── WorkItemTracker.API.Tests/
│   │
│   └── README.md
│
├── WorkItemTracker.Frontend/
│   ├── src/
│   │   └── app/
│   │
│   ├── package.json
│   └── README.md
│
└── README.md
```

---

# Backend

The backend is an ASP.NET Core Web API responsible for work item management, validation, persistence, searching, filtering, pagination, and status transitions.

## Technology Stack

* **ASP.NET Core 8 Web API**
* **Entity Framework Core 8**
* **SQLite** for runtime persistence
* **xUnit + EF Core InMemory** for automated tests
* **Swagger/OpenAPI** in Development

## API Endpoints

### Create Work Item

**POST** `/api/work-items`

#### Request Body

```json
{
  "title": "Prepare monthly report",
  "description": "Finish the report before Friday"
}
```

New work items always start with the `Todo` status.

---

### Search, Filter & Pagination

**GET**

```text
/api/work-items?Title=report&Status=Todo&Page=1&PageSize=10
```

Query parameters are mapped from `WorkItemSearchDto`.

| Parameter  | Description                                                          |
| ---------- | -------------------------------------------------------------------- |
| `Title`    | Optional. Filters by title using a case-insensitive substring match. |
| `Status`   | Optional. Filters by status: `Todo`, `InProgress`, or `Done`.        |
| `Page`     | Optional. Defaults to `1`. Must be ≥ `1`.                            |
| `PageSize` | Optional. Defaults to `10`. Must be between `1` and `100`.           |

#### Response

Returns **200 OK** with:

`PagedResult<WorkItemResponse>`

The response contains:

* `items`
* `page`
* `pageSize`
* `totalCount`
* `totalPages`

#### Validation

Returns **400 Bad Request** with a validation problem response if:

* `Page` is less than `1`
* `PageSize` is less than `1`
* `PageSize` is greater than `100`

Supported status values:

* `Todo`
* `InProgress`
* `Done`

---

### Get Work Item by ID

**GET**

```text
/api/work-items/{id}
```

---

### Change Work Item Status

**PATCH**

```text
/api/work-items/{id}/status
```

#### Request Body

```json
{
  "status": "InProgress"
}
```

#### Allowed Status Transitions

```text
Todo       → InProgress
InProgress → Done
```

Other status transitions return **409 Conflict**.

Missing work items return **404 Not Found**.

Invalid request input returns **400 Bad Request**.

---

## Running the Backend

Navigate to the backend directory:

```bash
cd WorkItemTracker.Backend
```

Restore dependencies:

```bash
dotnet restore
```

Run the API:

```bash
dotnet run --project src/WorkItemTracker.API
```

### Swagger

Swagger is available at:

```text
http://localhost:5080/swagger
```

This URL applies when using the included HTTP launch profile.

### Database

The application uses SQLite for runtime persistence.

The database file is:

```text
workitems.db
```

It is created in the API working directory and persists across application restarts.

## Running Backend Tests

Run the automated tests with:

```bash
dotnet test tests/WorkItemTracker.API.Tests
```

The tests cover:

* Valid status transitions
* Invalid status transitions
* Missing work items
* Persistence across different `DbContext` instances

## Backend Assumptions

* IDs are integer auto-increment keys.
* Created timestamps are stored in UTC.
* Title search is a case-insensitive, SQLite `LIKE`-style contains search according to the database collation.
* The default page is `1`.
* The default page size is `10`.
* The maximum page size is `100`.
* `EnsureCreated` is used to keep the assessment setup simple. A production system would use EF Core migrations.

---

# Frontend

The frontend is an Angular application that provides the user interface for managing work items.

## Technology Stack

* **Angular**
* **TypeScript**
* **RxJS**
* Responsive UI
* Typed API models
* Service-based API integration

## Frontend Architecture

```text
src/app
│
├── core
│   ├── models
│   │   └── work-item.model.ts
│   │
│   └── services
│       ├── api-error.service.ts
│       └── work-items-api.service.ts
│
├── features
│   └── work-items
│       └── pages
│           ├── work-items-page.component.html
│           ├── work-items-page.component.scss
│           └── work-items-page.component.ts
│
├── app.component.ts
├── app.config.ts
└── app.routes.ts
```

## Frontend Features

* Create work items with client-side validation.
* Search by title with debounce and cancellation of stale requests.
* Filter by `Todo`, `InProgress`, and `Done`.
* Server-side pagination.
* Advance status only through the allowed workflow:
  `Todo → InProgress → Done`.
* Loading states.
* Empty states.
* Validation states.
* API error handling.
* Retry after API failures.
* Responsive UI.
* Typed API models and service layer.

## Requirements

* **Node.js 20+ or 22+**
* **npm**
* Running Work Item Tracker backend

## Running the Frontend

Navigate to the frontend directory:

```bash
cd WorkItemTracker.Frontend
```

Install dependencies:

```bash
npm install
```

Start the development server:

```bash
npm start
```

The Angular development server runs at:

```text
http://localhost:4200
```

### API Configuration

The development API URL is configured in:

```text
src/environments/environment.ts
```

Default API URL:

```text
http://localhost:5080/api
```

---

# API Contract

The frontend communicates with the backend through the following endpoints:

| Method  | Endpoint                                         | Purpose                                |
| ------- | ------------------------------------------------ | -------------------------------------- |
| `POST`  | `/api/work-items`                                | Create a work item                     |
| `GET`   | `/api/work-items?Title=&Status=&Page=&PageSize=` | Search, filter and paginate work items |
| `GET`   | `/api/work-items/{id}`                           | Get a work item by ID                  |
| `PATCH` | `/api/work-items/{id}/status`                    | Change work item status                |

---

# Running the Complete Application

The backend and frontend need to be running separately during local development.

### 1. Start the Backend

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

### 2. Start the Frontend

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

### 3. Open the Application

Navigate to:

```text
http://localhost:4200
```

The Angular frontend will communicate with the ASP.NET Core API running on port `5080`.

---

# Work Item Workflow

A work item follows a simple state transition workflow:

```text
┌──────┐
│ Todo │
└──┬───┘
   │
   ▼
┌───────────┐
│ InProgress│
└─────┬─────┘
      │
      ▼
┌──────┐
│ Done │
└──────┘
```

Only the following transitions are allowed:

```text
Todo       → InProgress
InProgress → Done
```

Invalid transitions are rejected by the backend.

---

# Development Notes

The application is intentionally structured with a separation between the frontend and backend responsibilities.

The backend owns:

* Business rules
* Validation
* Persistence
* Search and filtering
* Pagination
* Status transition rules
* API contract

The frontend owns:

* User interaction
* Client-side validation
* Search and filtering UI
* Pagination UI
* Loading and error states
* API communication
* Responsive presentation

This separation keeps business rules on the server while allowing the Angular application to remain focused on presentation and user interaction.

---

# Assessment Scope

The solution implements the requested Work Item Tracker functionality using:

* ASP.NET Core 8
* Angular
* Entity Framework Core
* SQLite
* Automated tests
* Server-side search, filtering and pagination
* Controlled work item status transitions
