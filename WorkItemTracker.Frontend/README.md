# Work Item Tracker — Angular Frontend

Angular frontend for the Craft Crew .NET / Angular Work Item Tracker assessment.

## Architecture

```text
src/app
├── core
│   ├── models
│   │   └── work-item.model.ts
│   └── services
│       ├── api-error.service.ts
│       └── work-items-api.service.ts
├── features
│   └── work-items
│       └── pages
│           ├── work-items-page.component.html
│           ├── work-items-page.component.scss
│           └── work-items-page.component.ts
├── app.component.ts
├── app.config.ts
└── app.routes.ts
```

## Features

- Create work items with client-side validation.
- Search by title with debounce and cancellation of stale requests.
- Filter by `Todo`, `InProgress`, and `Done`.
- Server-side pagination.
- Advance status only through the allowed workflow:
  `Todo -> InProgress -> Done`.
- Loading, empty, validation and API error states.
- Retry after API failures.
- Responsive UI.
- Typed API models and service layer.

## Requirements

- Node.js 20+ or 22+
- npm
- Running Work Item Tracker backend

## Run locally

```bash
npm install
npm start
```

The Angular development server runs at `http://localhost:4200`.

The development API URL is configured in:

`src/environments/environment.ts`

Default:

`http://localhost:5080/api`


## API contract

The frontend expects:

- `POST /api/work-items`
- `GET /api/work-items?Title=&Status=&Page=&PageSize=`
- `GET /api/work-items/{id}`
- `PATCH /api/work-items/{id}/status`