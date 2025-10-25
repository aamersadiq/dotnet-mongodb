# Bank Account Management System

A .NET 8 Web API application for managing bank accounts, transactions, and transfers with MongoDB as the database.

## Features

- Account management (create, retrieve accounts)
- Money transfers between accounts with balance validation
- Transaction history tracking
- RESTful API with Swagger documentation
- MongoDB integration

## Technical Stack

- .NET 8 Web API
- MongoDB for data storage
- MongoDB Entity Framework Core Provider
- Docker for containerization
- Swagger/OpenAPI for API documentation
- FluentValidation for request validation
- xUnit, Moq for unit testing

## Project Structure

The solution follows a clean architecture approach with the following projects:

- **BankAccountManagement.API**: API controllers, DTOs, and validators
- **BankAccountManagement.Core**: Domain entities, interfaces, and services
- **BankAccountManagement.Infrastructure**: Data access implementation, MongoDB context
- **BankAccountManagement.Tests**: Unit tests for services and validators

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker](https://www.docker.com/products/docker-desktop)
- [Docker Compose](https://docs.docker.com/compose/install/)

### Running the Application

1. Clone the repository:
   ```
   git clone <repository-url>
   cd dotnet-mongodb
   ```

2. Start MongoDB using Docker Compose:
   ```
   docker-compose up -d
   ```
   This will start MongoDB on port 27017 and MongoDB Express (a web-based MongoDB admin interface) on port 8081.

3. Build and run the application:
   ```
   dotnet build
   dotnet run --project BankAccountManagement.API
   ```

4. Access the Swagger UI:
   ```
   https://localhost:5001/swagger
   ```
   or
   ```
   http://localhost:5000/swagger
   ```

5. Access MongoDB Express:
   ```
   http://localhost:8081
   ```
   Use the following credentials:
   - Username: admin
   - Password: password

### Running Tests

```
dotnet test
```

## API Endpoints

### Accounts

- `GET /api/accounts` - Get all accounts
- `GET /api/accounts/{id}` - Get account by ID
- `POST /api/accounts` - Create a new account
- `DELETE /api/accounts/{id}` - Delete an account

### Transactions

- `GET /api/transactions/account/{accountId}` - Get transactions by account ID
- `POST /api/transactions/transfer` - Transfer money between accounts

## MongoDB Configuration

The application is configured to connect to MongoDB using the following settings in `appsettings.json`:

```json
"MongoDbSettings": {
  "ConnectionString": "mongodb://admin:password@localhost:27017",
  "DatabaseName": "BankAccountDb"
}
```

## Docker Compose Configuration

The `docker-compose.yml` file includes:

- MongoDB container
- MongoDB Express container for database management

```yaml
version: '3.8'

services:
  mongodb:
    image: mongo:latest
    container_name: bank-account-mongodb
    ports:
      - "27017:27017"
    environment:
      - MONGO_INITDB_ROOT_USERNAME=admin
      - MONGO_INITDB_ROOT_PASSWORD=password
    volumes:
      - mongodb_data:/data/db
    networks:
      - bank-network
    restart: unless-stopped

  mongo-express:
    image: mongo-express:latest
    container_name: bank-account-mongo-express
    ports:
      - "8081:8081"
    environment:
      - ME_CONFIG_MONGODB_ADMINUSERNAME=admin
      - ME_CONFIG_MONGODB_ADMINPASSWORD=password
      - ME_CONFIG_MONGODB_SERVER=mongodb
    networks:
      - bank-network
    depends_on:
      - mongodb
    restart: unless-stopped

networks:
  bank-network:
    driver: bridge

volumes:
  mongodb_data: