# SimpleTaskManager
## 1. Setup instructions
### Prerequisites
1. .NET 8 SDK
2. SQL Server
3. IDE (e.g., Visual Studio or VS Code).
### Clone the repository
Open the terminal, navigate to your desired folder.\
Run this command:
``` git clone https://github.com/Careless-boop/SimpleTaskManager.git ```
### Database connection
To run this application you must have **SQL Server** instance.\
Get ***connection string*** to your database instance, and\
In the ```appsettings.json``` file, update the **ConnectionStrings** section to point to your SQL Server instance.
### JWT Configuration
In ```appsettings.json```, configure the JWT settings with your desired secret key, issuer, and audience.
```
"JwtSettings": {
    "SecretKey": "CHANGEME",
    "Issuer": "CHANGEME",
    "Audience": "CHANGEME"
}
```
### Dependencies
Simply ensure all required dependencies are installed by running:\
```dotnet restore```
### Run the project
To run the API you should be in the root folder\
and run following command:\
```dotnet run --project SimpleTaskManager.WebApi```
## 2. API Documentation
### Interactive documentation
This project uses **Swagger**\
Once the application is running, you can access the interactive documentation at:\
```https://localhost:5022/swagger/index.html```
### Endpoints
All this endpoints routes must have API address (e.g., localhost:5022)\
**User**
1. **POST** /api/users/register\
Body:
```
{
  "email": "string", // required
  "username": "string", // required
  "password": "string" // required
}
```
Use this to register user with desired email, username and password.\
2. **POST** /api/users/login\
Body:
```
{
  "login": "string", // required
  "password": "string" // required
}
```
Use this to login as existing user.\
**Task**
1. **POST** /api/tasks
Body:
```
{
  "title": "string",
  "description": "string", // optional
  "dueDate": "string (DateTime)", // optional
  "status": 0 / 1 / 2, // required 0 - Pending | 1 - InProgress | 2 - Completed
  "priority": 0 / 1 / 2 // required 0 - Low | 1 - Medium | 2 - High
}
```
Description: Use this endpoint to create a new task. The task will be associated with the authenticated user, and only they can manage it.

2. **GET** /api/tasks
Query Parameters (Optional):

status: Filters tasks by status (Pending, InProgress, Completed)\
dueDate: Filters tasks by due date\
priority: Filters tasks by priority (Low, Medium, High)\
sortBy: Specifies the field to sort by (e.g., "Title", "DueDate", "Priority")\
sortDescending: Set to true for descending order, default is ascending (false)\
pageNumber: Specifies the page number for pagination (default is 1)\
pageSize: Specifies the number of tasks per page (default is 10)\

Description: Retrieve a list of tasks for the authenticated user. The list can be filtered, sorted, and paginated based on the specified parameters.\

Filtering:
You can filter tasks by status, dueDate, or priority. For example, to get tasks with Pending status and High priority:

```
/api/tasks?status=Pending&priority=High
```
Sorting:
You can sort tasks by Title, DueDate, or Priority. To sort tasks by DueDate in descending order:

```
/api/tasks?sortBy=DueDate&sortDescending=true
```
Pagination:
Tasks are paginated by default, meaning only a subset of tasks is returned on each request. Use pageNumber and pageSize to control pagination. For example, to get the second page with five tasks per page:

```
/api/tasks?pageNumber=2&pageSize=5
```
3. **GET** /api/tasks/{id}\
id - Guid of desired task\
Description: Use this endpoint to get the details of a specific task by its id. The task must belong to the authenticated user.

5. **PUT** /api/tasks/{id}\
id - Guid of desired task\
Body:
```
{
  "title": "string",
  "description": "string", // optional
  "dueDate": "string (DateTime)", // optional
  "status": 0 / 1 / 2, // required 0 - Pending | 1 - InProgress | 2 - Completed
  "priority": 0 / 1 / 2 // required 0 - Low | 1 - Medium | 2 - High
}
```
Description: Use this endpoint to update an existing task by its id. The task must belong to the authenticated user.

6. **DELETE** /api/tasks/{id}\
id - Guid of desired task\
Description: Use this endpoint to delete a task by its id. The task must belong to the authenticated user.
## 3. Architecture Explanation
### 1. 3-Layered Architecture
The project is designed with a 3-layer architecture for separation of concerns:

- Presentation Layer (WebAPI): Contains the API controllers that handle HTTP requests and responses. This is where the user interacts with the API.\
- Business Logic Layer (BLL): This layer contains the services, business rules, and application logic. It processes data received from the repository and performs validation and business-specific operations.\
- Data Access Layer (DAL): This layer is responsible for interacting with the database. The repository pattern is used here to abstract the direct interaction with the database.\
### 2. Entities
**User**: Represents the application user, storing login credentials and identifying information.\
**Task**: Represents a task belonging to a user, containing details such as title, description, due date, status, and priority.\
### 3. Authentication
**JWT** (***JSON Web Token***) is used for user authentication.\ 
After login, users receive a token, which they must include in\
the headers (Authorization: Bearer {token}) for authenticated routes (e.g., /tasks).
### 4. Repository Pattern
The repository pattern is used for database operations, which makes the codebase\
more testable, maintainable, and decoupled from the specific database technology (Entity Framework Core in this case).
### 5. Middleware
Custom middleware is used for user validation (authenticating the JWT) before accessing certain endpoints.
