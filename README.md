# How to Run This Project

Follow these steps to get the **BackendMarshProject** up and running on your local machine.

## Prerequisites
- **.NET 8.0+ SDK**
- **SQL Server**
- **Visual Studio 2022** or **VS Code**

## Step 1: Database Setup
1. Open SQL Server Management Studio (SSMS) or your preferred SQL tool.
2. Open sql scripts located in **backend-marsh-project\src\BackendMarshProject\Migrations**.
3. Execute **01_Schema.sql** to create the tables.
4. Execute **02_SeedData.sql** to populate the system with 2 Admins, 4 Inventory Managers, and 10 Employees.

Note: The seed script is idempotent; you can run it multiple times without
creating duplicate records.

## Step 2: Connection String Configuration
1. Open `appsettings.json` in the web api project.
2. Update the `ConnectionStrings:DefaultConnection` to match your local SQL Server instance.
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "server=localhost;Database=MarshProjectDatabase;Trusted_Connection=True;TrustServerCertificate=True"
   }

## Step 3: Running the Backend

### Using Visual Studio:
Open the ```.sln``` file, set the Web API project as the **Startup** Project, and press **F5**.

### Using Visual Studio:
```bash
dotnet restore
dotnet run --project BackendMarshProject
```

## Step 4: Accessing API Documentation
The project is equipped with Swagger for easy testing. Once running, navigate to:
```
https://localhost:XXXX/swagger/index.html
```

### Note
The AI Description Generator uses the **llama-3.1-8b-instant** model by **Groq Inc**. Remember to add the API Key to `appsettings.json` before starting the application
```json
I will attach it to the email.
```

## Step 5: Running Unit Tests
The solution includes a test suite for the Device Service logic. To run tests, use the Test
Explorer in Visual Studio or run:

```bash
dotnet test
```
# Authentication
Use the pre-seeded ```Administrator``` account for testing:
* **Email**: maria.popescu@itcorp.com
* **Password**: MyPass!1

Use the pre-seeded ```Inventory Manager``` account for testing:
* **Email**: manager.cluj@itcorp.com
* **Password**: MyPass!1

Use the pre-seeded ```Employee``` account for testing:
* **Email**: emp.doe@itcorp.com
* **Password**: MyPass!1

**Note**: All seeded account has pass **MyPass!1**.

# Key Features

## Role-Based Access Control
* **Secure Authentication**: Implemented using JWT (JSON Web Tokens) for stateless, secure user sessions.

* **Granular Authorization**: Multi-tier permission levels including Admin, Inventory Manager, and Employee, controlled via custom authorization policies and attributes.

## Advanced Inventory Search & Scoring
* **Relevance-Based Filtering**: Custom server-side scoring algorithm that ranks search results based on keyword matches across multiple fields (Name, Manufacturer, Processor).
* **Weighted Results**: Intelligent sorting that prioritizes direct name matches over technical specifications.

## Smart Pagination & Filtering
* **Optimized Data Retrieval**: Custom generic `ToPagedResult` extension methods for both `IQueryable` and `IEnumerable`, ensuring fast response times even with large datasets.
* **Dynamic Metadata**: API responses include full pagination metadata (Total Pages, HasNext, HasPrevious) for seamless frontend integration.

## Device Management Lifecycle
* **Automated Assignments**: Employees can view and claim unassigned devices, while Managers can oversee the entire company fleet.
* **Type Safety**: Strict validation on Device categories (Laptop, Phone, Tablet) using Enum data annotations and database constraints.

## Robust Data Integrity & Testing
* **Idempotent Seeding**: Database initialization scripts designed to run safely multiple times without data duplication.
* **Unit Testing Suite**: High test coverage for core business logic using **Moq** to isolate service dependencies and ensure reliable releases.

## Clean Architecture & Scalability
* **Repository Pattern**: Decouples data access from business logic, making the system easy to maintain and test.
* **Dependency Injection**: Fully utilizes .NET's DI container for manageable service lifetimes (Scoped/Transient).

# Free-Text Search

## The Logic: Weighted Relevance
The search doesn't treat every field the same. We implemented a Weighting System to ensure the most logical results appear at the top:
* Name -> 10 pts
* Manufacturer -> 5 pts
* Processor -> 3 pts
* RAM -> 1 pt

## How the Algorithm Processes a Query
1. **Tokenization**: The search string is split into individual "tokens" (words).
2. **Case-Insensitivity**: All comparisons are forced to `ToLower()` to ensure "iPhone" and "iphone" return the same results.
3. **Accumulation**: The system iterates through every device, adding points for every token found in the weighted fields.
4. **Filtering**: Any device with a score of **0 (no matches)** is discarded
5. **Ranking**: The list is ordered by `Score DESC`, so the "best" matches hit the user's screen first.

## Demo Video
https://www.youtube.com/watch?v=OW12Qj_i82M