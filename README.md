# Work Item Tracker — Backend

Backend implementation for the **Craft Crew .NET / Angular hiring assessment**.

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

**GET** `/api/work-items?Title=report&Status=Todo&Page=1&PageSize=10`

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

Supported `Status` values:

* `Todo`
* `InProgress`
* `Done`

---

### Get Work Item by ID

**GET** `/api/work-items/{id}`

---

### Change Work Item Status

**PATCH** `/api/work-items/{id}/status`

#### Request Body

```json
{
  "status": "InProgress"
}
```

#### Allowed Status Transitions

* `Todo` → `InProgress`
* `InProgress` → `Done`

Other status transitions return **409 Conflict**.

Missing work items return **404 Not Found**.

Invalid request input returns **400 Bad Request**.

## Running the Application

Restore the project dependencies:

```bash
dotnet restore
```

Run the API:

```bash
dotnet run --project src/WorkItemTracker.API
```

### Swagger

Swagger is available at:

`http://localhost:5080/swagger`

This URL applies when using the included HTTP launch profile.

### Database

The application uses SQLite for runtime persistence.

The database file is:

```text
workitems.db
```

It is created in the API working directory and persists across application restarts.

## Running the Tests

Run the automated tests with:

```bash
dotnet test tests/WorkItemTracker.API.Tests
```

The tests cover:

* Valid status transitions
* Invalid status transitions
* Missing work items
* Persistence across different `DbContext` instances

## Assumptions

* IDs are integer auto-increment keys.
* Created timestamps are stored in UTC.
* Title search is a case-insensitive, SQLite `LIKE`-style contains search according to the database collation.
* The default page is `1`.
* The default page size is `10`.
* The maximum page size is `100`.
* No authentication, deployment, or microservices are included because they are explicitly outside the assessment scope.
* `EnsureCreated` is used to keep the assessment setup simple. A production system would use EF Core migrations.