# EmployeeHub — Full-Stack Employee Management System

EmployeeHub is a full-stack employee management application built to demonstrate modern software engineering practices using .NET 10, Clean Architecture, SQL Server, Angular, and Blazor.

The project includes a secured REST API, two frontend implementations, automated testing, distributed caching, structured logging, and workflow automation.

## Technologies

- **Backend:** .NET 10, ASP.NET Core Web API, C#
- **Architecture:** Clean Architecture, Repository Pattern, Service Layer, Dependency Injection
- **Database:** SQL Server, Entity Framework Core
- **Caching:** Redis
- **Authentication and Authorization:** JWT, role-based authorization, policy-based authorization
- **Frontends:** Angular, TypeScript, Bootstrap, Blazor
- **API Documentation:** OpenAPI, Scalar, URL-based API versioning
- **Logging:** Serilog, structured logging, daily log files
- **Testing:** xUnit, Moq, automated unit and integration tests
- **Automation:** n8n


## Key Features

- Employee CRUD operations
- User authentication and role-based access control
- Admin-only restrictions for employee creation, updates, and deletion
- Server-side validation and client-side form validation
- Redis distributed caching
- Idempotent employee creation using idempotency keys
- Optimistic concurrency control using SQL Server row versions
- Consistent API versioning
- Structured application logging
- Automated unit and integration tests
- Angular and Blazor user interfaces
- Workflow automation using n8n

## Architecture

EmployeeHub separates business logic, application services, and infrastructure concerns into distinct projects.

- **EmployeeHub.Domain:** Core domain entities
- **EmployeeHub.Application:** DTOs, interfaces, and application services
- **EmployeeHub.Infrastructure:** Database context and data access implementations
- **EmployeeHub.Api:** REST API, controllers, authentication, and API configuration
- **EmployeeHub.Api.Tests:** Automated tests
- **EmployeeHub.Angular:** Angular frontend
- **EmployeeHub.Blazor:** Blazor frontend

![EmployeeHub Architecture](docs/screenshots/employeehub-architecture.png)

## Security

The application uses JWT authentication and authorization policies to protect API endpoints. Passwords are handled using password hashing, and administrative operations require appropriate authorization.

Sensitive configuration values must be supplied through local configuration or environment variables rather than committed to source control.

## Testing

The project includes automated unit and integration tests covering important application and API behavior.

Test results and instructions for running the tests will be documented here after the final repository verification.

## Running the Project

### Prerequisites

Install and configure the following tools:

- **.NET 10 SDK** — for the API, Blazor application, and automated tests.
- **SQL Server** — for employee and user data.
- **Redis** — for distributed caching and idempotency.
- **Node.js and npm** — for the Angular frontend.
- **Angular CLI** — if required by the frontend's startup workflow.

### Configuration

The API uses ASP.NET Core configuration to load connection settings.

- **SQL Server:** The database connection string is stored in ASP.NET Core User Secrets and is not committed to the repository.
- **Redis:** The connection string is read from the `Redis:ConnectionString` configuration key.
- **JWT authentication:** Configure the signing key securely according to the API's configuration requirements. Do not commit production secrets or credentials to source control.

Before starting the API, configure the required database, Redis, and JWT settings for your local environment.

### Starting the Applications

Detailed commands for database initialization, starting the API, running the Angular frontend, and launching the Blazor application will be documented after verifying each project's configuration.


## Project Purpose

EmployeeHub is a portfolio project demonstrating full-stack development, backend architecture, database integration, API security, frontend development, automated testing, and workflow automation.

## AI-Powered Department Report

EmployeeHub integrates with an n8n workflow to generate an employee department report using data from the EmployeeHub API.

The workflow demonstrates how application data can be retrieved through an API, processed by an automation workflow, and presented in a readable report with AI assistance.

![EmployeeHub AI-generated department report](docs/screenshots/employee-department-report.png)

**What this demonstrates:**
- Integration between the EmployeeHub API and n8n.
- Workflow automation for retrieving and processing employee data.
- AI-assisted report generation and presentation.
- Combining .NET application development with AI automation.




## Application Screenshots

EmployeeHub includes two frontend implementations built on the same backend API: Angular and Blazor.

### Angular Frontend

**Employee Management**

![EmployeeHub Angular employee list](docs/screenshots/angular-employees.png)

**Login**

![EmployeeHub Angular login](docs/screenshots/angular-login.png)

**Add Employee**

![EmployeeHub Angular add employee form](docs/screenshots/angular-add-employee.png)

**Edit Employee**

![EmployeeHub Angular edit employee form](docs/screenshots/angular-edit-employee.png)

**Delete Employee — Confirmation**

![EmployeeHub Angular delete confirmation](docs/screenshots/angular-delete-employee.png)

### Blazor Frontend

**Employee Management**

![EmployeeHub Blazor employee list](docs/screenshots/blazor-employees.png)

**Login**

![EmployeeHub Blazor login](docs/screenshots/blazor-login.png)

**Add Employee**

![EmployeeHub Blazor add employee form](docs/screenshots/blazor-add-employee.png)

**Edit Employee**

![EmployeeHub Blazor edit employee form](docs/screenshots/blazor-edit-employee.png)

