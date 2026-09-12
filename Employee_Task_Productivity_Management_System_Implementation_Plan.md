# Employee Task & Productivity Management System

## BTech 3rd-Year Minor Project — Full Implementation Plan

---

## 1. Project Title

**Employee Task & Productivity Management System**

A web-based application for managing employees, departments, projects, tasks, deadlines, task progress, comments, notifications, and productivity reports.

---

## 2. Project Objective

The system should help an organization answer:

- What tasks are assigned to each employee?
- What is the current status of each task?
- Which tasks are overdue?
- What is the progress of each project?
- How many tasks has an employee completed?
- What is the employee's task completion rate?
- Which employees or tasks need attention?
- What is the overall productivity for a selected period?

The goal is to create a practical business application rather than a simple CRUD project.

---

## 3. Recommended Technology Stack

### Backend
- ASP.NET Core MVC
- C#
- Entity Framework Core

### Database
- SQL Server

### Frontend
- HTML
- CSS
- Bootstrap
- JavaScript
- Razor Views

### Authentication
- ASP.NET Core Identity
- Role-based authorization

### Optional Advanced Technologies
- ASP.NET Core Web API
- SignalR
- LINQ
- Excel/PDF export
- Background services

For the minor project, first complete the core application with MVC + EF Core + SQL Server. Add advanced technologies only after the core system is stable.

---

# 4. User Roles

The system will contain three major roles.

## Admin

Responsibilities:

- Manage employees
- Manage departments
- Manage users and roles
- Manage projects
- View all tasks
- View reports
- Monitor overall productivity

## Manager

Responsibilities:

- Create projects
- Create tasks
- Assign tasks to employees
- Monitor project progress
- View team productivity
- Review overdue tasks

## Employee

Responsibilities:

- View assigned tasks
- Update task status
- Update task progress
- Add comments
- View task history
- View personal productivity

---

# 5. Main Modules

1. Authentication and Authorization
2. Admin Dashboard
3. Employee Management
4. Department Management
5. Project Management
6. Task Management
7. Employee Task Dashboard
8. Task Comments
9. Notifications
10. Productivity Calculation
11. Overdue Task Detection
12. Search and Filtering
13. Reports and Analytics
14. Optional Web API
15. Optional Real-Time Notifications

---

# 6. Database Design

## 6.1 Users

| Column | Type | Key |
|---|---|---|
| UserId | int | PK |
| Name | nvarchar(100) | |
| Email | nvarchar(150) | Unique |
| PasswordHash | nvarchar(max) | |
| RoleId | int | FK |
| DepartmentId | int | FK |
| IsActive | bit | |
| CreatedAt | datetime2 | |

---

## 6.2 Roles

| Column | Type | Key |
|---|---|---|
| RoleId | int | PK |
| RoleName | nvarchar(50) | Unique |

Initial data:

- 1 — Admin
- 2 — Manager
- 3 — Employee

---

## 6.3 Departments

| Column | Type | Key |
|---|---|---|
| DepartmentId | int | PK |
| DepartmentName | nvarchar(100) | |
| Description | nvarchar(500) | |
| IsActive | bit | |

Example departments:

- IT
- HR
- Finance
- Marketing
- Sales

---

## 6.4 Projects

| Column | Type | Key |
|---|---|---|
| ProjectId | int | PK |
| ProjectName | nvarchar(150) | |
| Description | nvarchar(max) | |
| StartDate | date | |
| EndDate | date | |
| Status | nvarchar(30) | |
| ManagerId | int | FK |
| CreatedAt | datetime2 | |

Project statuses:

- Not Started
- In Progress
- Completed
- On Hold
- Cancelled

---

## 6.5 Tasks

| Column | Type | Key |
|---|---|---|
| TaskId | int | PK |
| ProjectId | int | FK |
| TaskTitle | nvarchar(200) | |
| Description | nvarchar(max) | |
| AssignedTo | int | FK |
| CreatedBy | int | FK |
| Priority | nvarchar(30) | |
| Status | nvarchar(30) | |
| StartDate | date | |
| DueDate | date | |
| CompletedDate | date | Nullable |
| ProgressPercentage | int | |
| EstimatedHours | decimal(8,2) | |
| ActualHours | decimal(8,2) | |
| CreatedAt | datetime2 | |
| UpdatedAt | datetime2 | |

Priorities:

- Low
- Medium
- High
- Critical

Statuses:

- Pending
- In Progress
- Completed
- On Hold
- Cancelled

---

## 6.6 TaskComments

| Column | Type | Key |
|---|---|---|
| CommentId | int | PK |
| TaskId | int | FK |
| UserId | int | FK |
| CommentText | nvarchar(max) | |
| CreatedAt | datetime2 | |

---

## 6.7 TaskAttachments

Optional table for file uploads.

| Column | Type | Key |
|---|---|---|
| AttachmentId | int | PK |
| TaskId | int | FK |
| FileName | nvarchar(255) | |
| FilePath | nvarchar(500) | |
| UploadedBy | int | FK |
| UploadedAt | datetime2 | |

---

## 6.8 Notifications

| Column | Type | Key |
|---|---|---|
| NotificationId | int | PK |
| UserId | int | FK |
| Title | nvarchar(200) | |
| Message | nvarchar(max) | |
| IsRead | bit | |
| CreatedAt | datetime2 | |

---

## 6.9 TaskActivityLogs

This table records important task changes.

| Column | Type | Key |
|---|---|---|
| ActivityId | int | PK |
| TaskId | int | FK |
| UserId | int | FK |
| Action | nvarchar(100) | |
| OldValue | nvarchar(500) | Nullable |
| NewValue | nvarchar(500) | Nullable |
| CreatedAt | datetime2 | |

Example:

`Pending -> In Progress`

---

# 7. Database Relationships

Basic relationship structure:

```text
Department
    |
    +----< Users
              |
              +----< Tasks
              |
              +----< TaskComments
              |
              +----< Notifications

User/Manager
    |
    +----< Projects
              |
              +----< Tasks
                         |
                         +----< TaskComments
                         |
                         +----< TaskAttachments
                         |
                         +----< TaskActivityLogs
```

---

# 8. Authentication Module

Create a login page:

```text
Employee Task Management System

Email
[____________________]

Password
[____________________]

[ Login ]
```

After successful login:

```text
Admin      -> Admin Dashboard
Manager    -> Manager Dashboard
Employee   -> Employee Dashboard
```

Features:

- Login
- Logout
- Password hashing
- Authentication
- Authorization
- Role-based access
- Active/inactive user checking

---

# 9. Admin Dashboard

Display summary cards:

```text
Total Employees       120
Active Projects        18
Total Tasks           560
Completed Tasks       380
Pending Tasks          90
Overdue Tasks          30
```

Charts:

- Tasks by Status
- Tasks by Priority
- Projects by Status
- Department-wise Employees
- Monthly Completed Tasks

---

# 10. Employee Management

Admin can:

- Add employee
- Edit employee
- View employee
- Activate/deactivate employee
- Assign department
- Assign role
- Search employees
- Filter employees
- Paginate employee list

Example:

```text
ID | Name | Email | Department | Role | Status | Action
---------------------------------------------------------
1  | Rahul| ...   | IT         | Emp  | Active | Edit
2  | Priya| ...   | HR         | Emp  | Active | Edit
```

---

# 11. Department Management

Admin can:

- Add department
- Edit department
- Deactivate department
- Search department

Example:

```text
IT
HR
Finance
Marketing
Sales
```

---

# 12. Project Management

Admin/Manager can create:

```text
Project Name:
Employee Task Management System

Description:
Development of employee productivity platform

Start Date:
01-09-2026

End Date:
30-11-2026

Manager:
Selected Manager

Status:
In Progress
```

Project details page:

```text
Project: HRMS

Progress: 78%

Start: 01-Sep-2026
End:   30-Nov-2026

Total Tasks:     42
Completed:       30
Pending:          5
In Progress:     7
Overdue:         2
```

---

# 13. Task Management

Task creation form:

```text
Project
[HRMS]

Task Title
[Create Login API]

Description
[......................]

Assign Employee
[Rahul]

Priority
[High]

Start Date
[10-Sep-2026]

Due Date
[15-Sep-2026]

Estimated Hours
[8]

[Create Task]
```

Core features:

- Create task
- Edit task
- Delete/deactivate task
- Assign task
- Set priority
- Set status
- Set deadline
- Set estimated hours
- Track actual hours
- Track progress

---

# 14. Employee Task Board

Employee dashboard can display tasks by status:

```text
+----------------+----------------+----------------+
| PENDING        | IN PROGRESS    | COMPLETED      |
+----------------+----------------+----------------+
| Task 1         | Task 3         | Task 5         |
| Task 2         | Task 4         | Task 6         |
+----------------+----------------+----------------+
```

A Kanban-style board can be added as an advanced feature.

---

# 15. Task Progress

Employee can update:

```text
Task: Create Login API

Status:
[In Progress]

Progress:
60%

Actual Hours:
6

Comment:
Login API completed except validation.

[Update]
```

Business rules:

```text
If Status = Completed
    ProgressPercentage = 100
    CompletedDate = Current Date
```

Other validation:

- Progress must be between 0 and 100.
- Due date should not be before start date.
- Completed task should have 100% progress.
- Only authorized users can modify a task.

---

# 16. Task Comments

Users can discuss a task.

Example:

```text
Task: Login API

Rahul:
API implementation completed.

Priya:
Please add validation.

Rahul:
Validation added and tested.
```

Each comment stores:

- User
- Task
- Message
- Date/time

---

# 17. Notifications

Generate notifications for important events.

## New task

```text
New Task Assigned

You have been assigned:
"Create Login API"
```

## Deadline

```text
Deadline Approaching

Task "Login API" is due tomorrow.
```

## Completion

```text
Task Completed

Rahul completed:
"Create Login API"
```

Initially, database notifications are sufficient.

SignalR can be added later for real-time notifications.

---

# 18. Productivity Calculation

Example:

```text
Assigned Tasks = 20
Completed Tasks = 16
Pending Tasks = 3
Overdue Tasks = 1
```

Completion rate:

```text
Completion Rate =
Completed Tasks / Assigned Tasks * 100

16 / 20 * 100 = 80%
```

Display:

```text
Employee Productivity

Rahul       90%
Priya       85%
Amit        72%
Neha        68%
```

The exact productivity formula can later be improved by including:

- Completion rate
- Overdue tasks
- Estimated vs actual hours
- Task priority
- Project deadlines

For the minor project, keep the first version simple and explainable.

---

# 19. Overdue Task Detection

A task is overdue when:

```text
DueDate < Today
AND
Status != Completed
```

Example:

```text
OVERDUE TASKS

Task              Employee      Due Date
------------------------------------------------
Login API         Rahul         05-Sep
Report Module     Priya         07-Sep
Dashboard         Amit          08-Sep
```

Prefer calculating overdue status dynamically rather than permanently storing a potentially stale `IsOverdue` flag.

---

# 20. Search and Filtering

Task list should support:

```text
Search Task:
[Login________]

Status:
[All ▼]

Priority:
[High ▼]

Employee:
[Rahul ▼]

Project:
[HRMS ▼]

From:
[01-Sep-2026]

To:
[30-Sep-2026]

[Search]
```

This will also give you good SQL/LINQ examples for your viva.

---

# 21. Reports

## Employee Report

Columns:

```text
Employee
Department
Assigned Tasks
Completed Tasks
Pending Tasks
Overdue Tasks
Completion %
```

## Project Report

```text
Project
Total Tasks
Completed
Pending
Overdue
Progress %
```

## Monthly Report

```text
September 2026

Tasks Created       150
Tasks Completed     120
Tasks Pending        20
Tasks Overdue        10
```

Optional:

- Excel export
- PDF export

---

# 22. Recommended Architecture

Use:

```text
Controller
     |
     v
Service
     |
     v
Repository
     |
     v
DbContext
     |
     v
SQL Server
```

Example:

```text
TaskController
      |
      v
TaskService
      |
      v
TaskRepository
      |
      v
ApplicationDbContext
      |
      v
Tasks Table
```

Controllers should remain thin.

Example:

```csharp
public async Task<IActionResult> UpdateTask(UpdateTaskViewModel model)
{
    await _taskService.UpdateTaskAsync(model);

    return RedirectToAction("Index");
}
```

Business rules should live in the service layer.

---

# 23. Suggested Project Structure

```text
EmployeeTaskManagement
|
+-- Controllers
|   +-- AccountController.cs
|   +-- DashboardController.cs
|   +-- EmployeeController.cs
|   +-- DepartmentController.cs
|   +-- ProjectController.cs
|   +-- TaskController.cs
|   +-- ReportController.cs
|   +-- NotificationController.cs
|
+-- Services
|   +-- IEmployeeService.cs
|   +-- EmployeeService.cs
|   +-- IProjectService.cs
|   +-- ProjectService.cs
|   +-- ITaskService.cs
|   +-- TaskService.cs
|   +-- ProductivityService.cs
|
+-- Repositories
|   +-- IEmployeeRepository.cs
|   +-- EmployeeRepository.cs
|   +-- ITaskRepository.cs
|   +-- TaskRepository.cs
|
+-- Models
|   +-- Employee.cs
|   +-- Department.cs
|   +-- Project.cs
|   +-- TaskItem.cs
|   +-- TaskComment.cs
|   +-- Notification.cs
|
+-- ViewModels
|   +-- LoginViewModel.cs
|   +-- DashboardViewModel.cs
|   +-- EmployeeViewModel.cs
|   +-- TaskViewModel.cs
|   +-- ProductivityViewModel.cs
|
+-- Data
|   +-- ApplicationDbContext.cs
|
+-- Views
|   +-- Account
|   +-- Dashboard
|   +-- Employees
|   +-- Departments
|   +-- Projects
|   +-- Tasks
|   +-- Reports
|
+-- wwwroot
    +-- css
    +-- js
    +-- uploads
```

---

# 24. Optional Web API

After the MVC application works, add APIs.

Example endpoints:

```text
GET     /api/tasks
GET     /api/tasks/{id}
POST    /api/tasks
PUT     /api/tasks/{id}
DELETE  /api/tasks/{id}

GET     /api/employees/{id}/tasks

GET     /api/dashboard/summary

GET     /api/productivity/employee/{id}
```

This is optional but useful if you want to demonstrate Web API knowledge.

---

# 25. Development Phases

## Phase 1 — Project Setup

Tasks:

1. Create ASP.NET Core MVC project
2. Configure SQL Server
3. Configure EF Core
4. Create DbContext
5. Create entities
6. Configure relationships
7. Create migration
8. Update database

---

## Phase 2 — Authentication

Tasks:

1. Create user authentication
2. Create roles
3. Create login
4. Create logout
5. Configure authorization
6. Test role-based navigation

Test:

```text
Admin -> Admin Dashboard
Manager -> Manager Dashboard
Employee -> Employee Dashboard
```

---

## Phase 3 — Master Data

Implement:

```text
Department
Employee
Role
```

Complete CRUD and validation.

---

## Phase 4 — Projects

Implement:

```text
Create Project
Edit Project
View Project
Deactivate Project
Project List
Project Details
```

---

## Phase 5 — Tasks

Implement:

```text
Create Task
Assign Task
Edit Task
Delete/Deactivate Task
Task Details
Task List
Priority
Status
Progress
Deadline
Estimated Hours
Actual Hours
```

---

## Phase 6 — Employee Features

Implement:

```text
Employee Dashboard
My Tasks
Update Status
Update Progress
Add Comments
View Task History
```

---

## Phase 7 — Business Logic

Implement:

```text
Overdue Detection
Completion Percentage
Project Progress
Employee Productivity
Deadline Alerts
Task Activity Logging
```

---

## Phase 8 — Reports and UI

Implement:

```text
Charts
Search
Filters
Pagination
Employee Reports
Project Reports
Monthly Reports
Dashboard Analytics
```

---

## Phase 9 — Advanced Features

Only after the core system works:

```text
SignalR Notifications
File Attachments
Kanban Board
Excel Export
PDF Export
Web API
Background Services
```

---

# 26. Eight-Week Development Plan

## Week 1 — Foundation

- Project creation
- Solution structure
- SQL Server
- EF Core
- Models
- Relationships
- Migrations

## Week 2 — Authentication

- Login
- Logout
- Roles
- Authorization
- Employee management
- Department management

## Week 3 — Projects

- Project CRUD
- Project details
- Project status
- Manager assignment

## Week 4 — Tasks

- Task CRUD
- Assignment
- Priority
- Status
- Progress
- Deadlines

## Week 5 — Employee Module

- Employee dashboard
- My tasks
- Task updates
- Comments
- Activity history

## Week 6 — Productivity

- Completion %
- Overdue detection
- Employee productivity
- Project progress
- Notifications

## Week 7 — Reports and UI

- Dashboard charts
- Search
- Filters
- Reports
- Responsive UI

## Week 8 — Testing and Documentation

- Unit/manual testing
- Bug fixing
- Screenshots
- ER diagram
- DFD
- UML diagrams
- Project report
- Presentation
- Viva preparation

---

# 27. Minimum Viable Project

If the deadline becomes tight, prioritize these features:

```text
[✓] Login
[✓] Role-based authorization
[✓] Employee CRUD
[✓] Department CRUD
[✓] Project CRUD
[✓] Task CRUD
[✓] Task assignment
[✓] Task status
[✓] Task progress
[✓] Deadline
[✓] Employee dashboard
[✓] Manager dashboard
[✓] Productivity calculation
[✓] Search/filter
[✓] Reports
```

This is already enough for a strong BTech minor project.

---

# 28. Advanced Version

After the MVP is stable:

```text
                 EMPLOYEE TASK SYSTEM
                          |
       +------------------+------------------+
       |                  |                  |
       v                  v                  v
   Management        Productivity      Communication
       |                  |                  |
   Employees          Analytics          Comments
   Projects           Reports            Notifications
   Tasks              Charts             SignalR
       |                  |
       +----------+-------+
                  |
                  v
            Smart Features
                  |
          Deadline Alerts
          Overdue Detection
          Performance Trends
```

AI is not required. A well-designed rule-based productivity system is sufficient for a BTech minor project.

---

# 29. Testing Plan

Test at least:

### Authentication

- Valid login
- Invalid login
- Logout
- Unauthorized page access
- Role restrictions

### Employee

- Add employee
- Duplicate email
- Edit employee
- Deactivate employee

### Project

- Create project
- Invalid dates
- Edit project
- Project status

### Task

- Create task
- Assign task
- Invalid deadline
- Update status
- Update progress
- Complete task
- Overdue detection

### Reports

- Correct task counts
- Correct completion percentage
- Correct filters
- Correct date ranges

---

# 30. Important Business Rules

Implement and document these rules:

1. Email must be unique.
2. Inactive employees cannot receive new tasks.
3. Task due date cannot be earlier than start date.
4. Progress must remain between 0 and 100.
5. Completed task automatically becomes 100%.
6. Completed task gets a completion date.
7. Overdue means due date has passed and task is not completed.
8. Only authorized users can update tasks.
9. Managers can manage their assigned projects.
10. Employees can update only their own assigned tasks.
11. Project progress can be calculated from task progress.
12. Important changes should be stored in activity logs.

---

# 31. Project Documentation

Your BTech report should contain:

## Chapter 1 — Introduction

- Background
- Problem statement
- Objectives
- Scope

## Chapter 2 — Existing System

- Problems with manual task management
- Limitations of existing approach

## Chapter 3 — Proposed System

- Proposed solution
- Advantages
- Main features

## Chapter 4 — Requirements

### Hardware
- Computer/Laptop
- 8 GB+ RAM recommended
- Internet connection for development resources

### Software
- Windows
- Visual Studio
- .NET SDK
- SQL Server
- SQL Server Management Studio
- Browser

## Chapter 5 — System Design

- Architecture diagram
- ER diagram
- DFD
- Use-case diagram
- Class diagram
- Sequence diagrams

## Chapter 6 — Database Design

- Tables
- Relationships
- Primary keys
- Foreign keys
- Constraints

## Chapter 7 — Implementation

- Authentication
- Employee module
- Project module
- Task module
- Productivity module
- Reports

## Chapter 8 — Testing

- Test cases
- Expected result
- Actual result
- Status

## Chapter 9 — Results

- Screenshots
- Dashboard
- Task management
- Reports

## Chapter 10 — Conclusion and Future Scope

---

# 32. Future Scope

Potential future features:

- Mobile application
- Advanced analytics
- AI-based task recommendations
- Intelligent workload distribution
- Email notifications
- Calendar integration
- Attendance integration
- Performance appraisal integration
- Cloud deployment
- Advanced predictive analytics

Do not implement all of these for the minor project. Mention them as future scope.

---

# 33. Viva Preparation

This project gives you questions covering:

### C#

- OOP
- Classes and objects
- Inheritance
- Polymorphism
- Interfaces
- Exception handling
- Collections
- LINQ
- async/await

### ASP.NET Core

- MVC
- Controllers
- Models
- ViewModels
- Dependency Injection
- Middleware
- Routing
- Authentication
- Authorization
- Model validation

### EF Core

- DbContext
- DbSet
- Relationships
- Migrations
- LINQ
- Tracking
- Include
- CRUD

### SQL

- Primary key
- Foreign key
- Joins
- Constraints
- Normalization
- Indexes
- GROUP BY
- Aggregate functions
- Subqueries

---

# 34. Recommended Implementation Strategy

Do NOT start by creating every module.

Follow this order:

```text
Database
   ↓
Models
   ↓
DbContext
   ↓
Repositories
   ↓
Services
   ↓
Controllers
   ↓
Views
   ↓
Validation
   ↓
Business Logic
   ↓
Dashboard
   ↓
Reports
   ↓
Advanced Features
```

Most importantly:

**Build one complete module at a time and test it before moving to the next module.**

---

# 35. Final Recommended Scope

For the BTech minor project, the final application should contain:

```text
Employee Task & Productivity Management System

├── Authentication
├── Role Management
├── Employee Management
├── Department Management
├── Project Management
├── Task Management
├── Task Assignment
├── Task Progress
├── Task Comments
├── Task Activity History
├── Notifications
├── Overdue Detection
├── Employee Productivity
├── Project Progress
├── Search & Filtering
├── Admin Dashboard
├── Manager Dashboard
├── Employee Dashboard
└── Reports & Analytics
```

The **core stack** should remain:

```text
C#
ASP.NET Core MVC
Entity Framework Core
SQL Server
HTML
CSS
Bootstrap
JavaScript
```

Optional additions:

```text
Web API
SignalR
Excel/PDF Export
```

This scope is substantial enough for a BTech 3rd-year individual minor project while still being realistic to implement and explain in a viva.
