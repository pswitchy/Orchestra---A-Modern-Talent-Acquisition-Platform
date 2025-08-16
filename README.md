# Orchestra - A Modern Talent Acquisition Platform

![.NET](https://img.shields.io/badge/.NET-8.0-blue.svg) ![PostgreSQL](https://img.shields.io/badge/PostgreSQL-15-blue.svg) ![Docker](https://img.shields.io/badge/Docker-Powered-blue.svg) ![Architecture](https://img.shields.io/badge/Architecture-Microservices-green.svg)

Orchestra is a robust, scalable, and complex backend system designed to simulate a modern talent acquisition platform. Built with C# and .NET 8, it follows a microservices architecture to ensure a clean separation of concerns, high availability, and maintainability.

This project was created as a comprehensive showcase of modern backend engineering principles, inspired by the requirements for a senior .NET developer role.

## 🏛️ Architectural Vision

The system is designed as a distributed set of services that work together, or "in concert," like an orchestra. Each service has a distinct responsibility and its own isolated database, following the **database-per-service** pattern. This ensures loose coupling and allows services to be developed, deployed, and scaled independently.

### Core Services

1.  **Orchestra.IAM (Identity & Access Management):** A standalone service responsible for user registration, authentication, and the generation of JSON Web Tokens (JWT). It acts as the central authority for security.
2.  **Orchestra.ATS (Application Tracking System):** The core business domain service. It manages job postings, candidate profiles, and the application process. It relies on the IAM service to secure its endpoints.
3.  **(Future) Orchestra.Analytics:** A planned worker service that will process events from the ATS to provide insights, such as hiring velocity and candidate matching, likely using a message queue like RabbitMQ.

### System Flow Diagram
```
              +------------------+       +----------------------+
              |                  |       |                      |
User / Client +----(1. Register)---> Orchestra.IAM Service <----+
              |   |-(2. Login)----->       (Port 5433 DB)      | (Manages Users & Roles)
              |   |              |       |                      |
              +---+--------------+       +----------------------+
                  |
                  | (3. Receive JWT Token)
                  |
                  v
              +---+---------------------------------+
              |                                     |
              +----(4. API Call w/ JWT in Header)---> Orchestra.ATS Service
              |                                     |  (Port 5434 DB)
              +-------------------------------------+ (Manages Jobs, Candidates, Applications)
```

## ✨ Features Implemented

### Orchestra.IAM Service
-   ✅ Secure user registration with password hashing (BCrypt).
-   ✅ JWT-based authentication (`/api/auth/login`).
-   ✅ Role-based access control (RBAC) foundation via claims embedded in the JWT.

### Orchestra.ATS Service
-   ✅ **Jobs Management:**
    -   `GET /api/jobs`: Publicly accessible endpoint to view all active job listings.
    -   `POST /api/jobs`: Admin-only, protected endpoint for creating new job postings.
-   ✅ **Candidate Management:**
    -   `POST /api/candidates`: Admin-only endpoint to add candidates to the talent pool.
    -   `GET /api/candidates`: Protected endpoint to view all candidates.
-   ✅ **Application Workflow:**
    -   `POST /api/jobs/{jobId}/apply`: A public endpoint that allows anyone to apply for a job.
    -   Implements a **find-or-create** pattern: if the applying candidate's email doesn't exist, a new candidate profile is created automatically within the same transaction.

## 🛠️ Technology Stack

| Category          | Technology / Library                                      | Purpose                                                    |
| ----------------- | --------------------------------------------------------- | ---------------------------------------------------------- |
| **Backend**       | **.NET 8** & **ASP.NET Core**                             | Core application framework.                                |
|                   | **Entity Framework Core 8**                               | ORM for database interaction.                              |
| **Database**      | **PostgreSQL 15**                                         | Robust, open-source relational database.                   |
| **Containerization** | **Docker & Docker Compose**                            | For running databases and services in isolated containers. |
| **Security**      | **JWT Bearer Authentication**                             | Standard for securing stateless APIs.                      |
|                   | **BCrypt.Net-Next**                                       | For secure password hashing.                               |
| **Testing**       | **xUnit**                                                 | For writing unit and integration tests.                    |
|                   | **Microsoft.AspNetCore.Mvc.Testing**                      | For in-memory integration testing of API endpoints.        |
| **Observability** | **Serilog**                                               | For structured, production-ready logging.                  |
| **API Docs**      | **Swashbuckle (Swagger)**                                 | For interactive API documentation and testing.             |

## 🚀 Getting Started

Follow these instructions to get the entire Orchestra platform running on your local machine.

### Prerequisites

1.  **.NET 8 SDK:** [Download & Install](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
2.  **Docker Desktop:** [Download & Install](https://www.docker.com/products/docker-desktop/)
3.  **EF Core Global Tool:** Run `dotnet tool install --global dotnet-ef` in your terminal.
4.  **Git:** To clone the repository.

### 1. Clone & Setup

```bash
# Clone the repository
git clone <your-repository-url>
cd Orchestra
```

### 2. Launch Databases

Make sure Docker Desktop is running. Then, from the root `/Orchestra` directory, start the PostgreSQL containers.

```bash
docker-compose up -d
```
This command will start two separate PostgreSQL instances:
-   `orchestra-postgres-iam` on `localhost:5433`
-   `orchestra-postgres-ats` on `localhost:5434`

### 3. Prepare the Databases (Migrations)

You need to apply the EF Core migrations for each service to create the database schemas.

```bash
# First, for the IAM service
cd src/Orchestra.IAM
dotnet ef database update

# Second, for the ATS service
cd ../Orchestra.ATS
dotnet ef database update

# Return to the root
cd ../..```

### 4. Run the Services

You must run each service in a separate terminal.

-   **Terminal 1: Run the IAM Service**
    ```bash
    cd src/Orchestra.IAM
    dotnet run
    ```
    *(The service will be available at a URL like `https://localhost:7123`)*

-   **Terminal 2: Run the ATS Service**
    ```bash
    cd src/Orchestra.ATS
    dotnet run
    ```
    *(The service will be available at a URL like `https://localhost:7254`)*

You now have the entire platform running!

## 🧪 API Usage and Testing Workflow

Here’s how to use the services together.

1.  **Register a User (IAM Service):**
    -   Navigate to the Swagger UI for the IAM service (e.g., `https://localhost:7123/swagger`).
    -   Execute the `POST /api/auth/register` endpoint with an email and a strong password.

2.  **Get an Auth Token (IAM Service):**
    -   Execute the `POST /api/auth/login` endpoint with the credentials you just created.
    -   Copy the `token` string from the response body.

3.  **Authorize in the ATS Service:**
    -   Navigate to the Swagger UI for the ATS service (e.g., `https://localhost:7254/swagger`).
    -   Click the green `Authorize` button at the top right.
    -   In the dialog, paste your token in the format: `Bearer <your_copied_token>`.
    -   Click `Authorize`. You are now authenticated.

4.  **Test a Protected Endpoint (ATS Service):**
    -   Try to execute the `POST /api/jobs` endpoint. Since you are now authorized, you will receive a `201 Created` response.
    -   If you log out and try again, you will receive a `401 Unauthorized` error.

## ✅ Running the Tests

To verify the integrity of the codebase, run the suite of unit and integration tests.

```bash
# From the root /Orchestra/ directory
dotnet test
```

## 🏗️ Project Structure

The solution is organized with a clear separation of concerns:

-   `/src`: Contains all the source code for the running services.
    -   `/Orchestra.IAM`: The self-contained Identity service.
    -   `/Orchestra.ATS`: The self-contained Application Tracking service.
    -   `/Shared`: Contains code (like the auth extension method) shared across services to avoid duplication.
-   `/tests`: Contains all test projects.
    -   `Orchestra.ATS.UnitTests`: Fast, in-memory tests for business logic in the ATS service.
    -   `Orchestra.ATS.IntegrationTests`: Slower tests that check the full API pipeline for the ATS service.

Within each service project, the structure is organized by feature (e.g., `Api/Jobs`, `Api/Candidates`), a pattern that scales well and is related to Vertical Slice Architecture.

## 🛣️ Future Work & Roadmap

-   [ ] **Implement the Analytics Service:** Build out the `Orchestra.Analytics` worker service using RabbitMQ for event-driven processing.
-   [ ] **Implement Refresh Tokens:** Add a secure endpoint to the IAM service to refresh expired JWTs.
-   [ ] **Full Integration Test Coverage:** Expand the integration test suite to cover all endpoints, including a proper test database and authentication handling.
-   [ ] **CI/CD Pipeline:** Create a GitHub Actions or Azure DevOps pipeline to automatically build, test, and package the services into Docker images on every commit.
