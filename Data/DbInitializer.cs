using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using EmployeeManagement.Models;

namespace EmployeeManagement.Data
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("DbInitializer");

            try
            {
                // Ensures the database is created and migrations applied
                await context.Database.EnsureCreatedAsync();

                // Check if already seeded
                if (await context.Users.AnyAsync())
                {
                    return; // DB already has data
                }

                logger.LogInformation("Seeding database with initial data...");

                // 1. Seed Departments
                var itDept = new Department { DepartmentName = "IT & Software Development", Description = "Software engineering, infrastructure and IT support", IsActive = true };
                var hrDept = new Department { DepartmentName = "Human Resources", Description = "Talent acquisition, employee welfare, and onboarding", IsActive = true };
                var finDept = new Department { DepartmentName = "Finance & Accounting", Description = "Financial reporting, payroll, budgeting and audit", IsActive = true };
                var mktDept = new Department { DepartmentName = "Marketing & Sales", Description = "Brand growth, digital campaigns, customer relations", IsActive = true };

                context.Departments.AddRange(itDept, hrDept, finDept, mktDept);
                await context.SaveChangesAsync();

                // Password Hasher
                var passwordHasher = new PasswordHasher<User>();

                // 2. Seed Users
                var adminUser = new User
                {
                    Name = "System Admin",
                    Email = "admin@company.com",
                    RoleId = 1, // Admin
                    DepartmentId = itDept.DepartmentId,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow.AddMonths(-3)
                };
                adminUser.PasswordHash = passwordHasher.HashPassword(adminUser, "Admin@123");

                var managerUser = new User
                {
                    Name = "Sarah Jenkins (Project Manager)",
                    Email = "manager@company.com",
                    RoleId = 2, // Manager
                    DepartmentId = itDept.DepartmentId,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow.AddMonths(-3)
                };
                managerUser.PasswordHash = passwordHasher.HashPassword(managerUser, "Manager@123");

                var empRahul = new User
                {
                    Name = "Rahul Sharma",
                    Email = "rahul@company.com",
                    RoleId = 3, // Employee
                    DepartmentId = itDept.DepartmentId,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow.AddMonths(-2)
                };
                empRahul.PasswordHash = passwordHasher.HashPassword(empRahul, "Employee@123");

                var empPriya = new User
                {
                    Name = "Priya Verma",
                    Email = "priya@company.com",
                    RoleId = 3, // Employee
                    DepartmentId = hrDept.DepartmentId,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow.AddMonths(-2)
                };
                empPriya.PasswordHash = passwordHasher.HashPassword(empPriya, "Employee@123");

                var empAmit = new User
                {
                    Name = "Amit Patel",
                    Email = "amit@company.com",
                    RoleId = 3, // Employee
                    DepartmentId = finDept.DepartmentId,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow.AddMonths(-1)
                };
                empAmit.PasswordHash = passwordHasher.HashPassword(empAmit, "Employee@123");

                var empNeha = new User
                {
                    Name = "Neha Gupta",
                    Email = "neha@company.com",
                    RoleId = 3, // Employee
                    DepartmentId = mktDept.DepartmentId,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow.AddMonths(-1)
                };
                empNeha.PasswordHash = passwordHasher.HashPassword(empNeha, "Employee@123");

                context.Users.AddRange(adminUser, managerUser, empRahul, empPriya, empAmit, empNeha);
                await context.SaveChangesAsync();

                // 3. Seed Projects
                var erpProject = new Project
                {
                    ProjectName = "Employee Productivity & ERP Platform",
                    Description = "Next-generation company task tracking and performance analytics portal.",
                    StartDate = DateTime.Today.AddDays(-30),
                    EndDate = DateTime.Today.AddDays(45),
                    Status = "In Progress",
                    ManagerId = managerUser.UserId,
                    CreatedAt = DateTime.UtcNow.AddDays(-30)
                };

                var hrProject = new Project
                {
                    ProjectName = "HR Automation & Onboarding Portal",
                    Description = "Streamlining digital onboarding and employee handbook reviews.",
                    StartDate = DateTime.Today.AddDays(-15),
                    EndDate = DateTime.Today.AddDays(30),
                    Status = "In Progress",
                    ManagerId = managerUser.UserId,
                    CreatedAt = DateTime.UtcNow.AddDays(-15)
                };

                var finProject = new Project
                {
                    ProjectName = "Annual Financial Audit & Compliance",
                    Description = "Consolidation of fiscal records and statutory tax audits.",
                    StartDate = DateTime.Today.AddDays(-60),
                    EndDate = DateTime.Today.AddDays(-10),
                    Status = "Completed",
                    ManagerId = managerUser.UserId,
                    CreatedAt = DateTime.UtcNow.AddDays(-60)
                };

                context.Projects.AddRange(erpProject, hrProject, finProject);
                await context.SaveChangesAsync();

                // 4. Seed Tasks
                var task1 = new TaskItem
                {
                    ProjectId = erpProject.ProjectId,
                    TaskTitle = "Design Database Schema & Entities",
                    Description = "Create EF Core model mappings for Users, Projects, Tasks, and Audit Logs.",
                    AssignedTo = empRahul.UserId,
                    CreatedBy = managerUser.UserId,
                    Priority = "High",
                    Status = "Completed",
                    StartDate = DateTime.Today.AddDays(-25),
                    DueDate = DateTime.Today.AddDays(-15),
                    CompletedDate = DateTime.Today.AddDays(-16),
                    ProgressPercentage = 100,
                    EstimatedHours = 16,
                    ActualHours = 14,
                    CreatedAt = DateTime.UtcNow.AddDays(-25),
                    UpdatedAt = DateTime.UtcNow.AddDays(-16)
                };

                var task2 = new TaskItem
                {
                    ProjectId = erpProject.ProjectId,
                    TaskTitle = "Implement Cookie Authentication & Role Policies",
                    Description = "Configure multi-role authentication with Admin, Manager, and Employee access.",
                    AssignedTo = empRahul.UserId,
                    CreatedBy = managerUser.UserId,
                    Priority = "Critical",
                    Status = "In Progress",
                    StartDate = DateTime.Today.AddDays(-10),
                    DueDate = DateTime.Today.AddDays(3),
                    ProgressPercentage = 80,
                    EstimatedHours = 20,
                    ActualHours = 18,
                    CreatedAt = DateTime.UtcNow.AddDays(-10),
                    UpdatedAt = DateTime.UtcNow.AddDays(-1)
                };

                var task3 = new TaskItem
                {
                    ProjectId = erpProject.ProjectId,
                    TaskTitle = "Develop Productivity Calculation Engine",
                    Description = "Compute task completion rates, overdue impact, and project velocity.",
                    AssignedTo = empRahul.UserId,
                    CreatedBy = managerUser.UserId,
                    Priority = "High",
                    Status = "Pending",
                    StartDate = DateTime.Today.AddDays(-2),
                    DueDate = DateTime.Today.AddDays(10),
                    ProgressPercentage = 25,
                    EstimatedHours = 16,
                    ActualHours = 4,
                    CreatedAt = DateTime.UtcNow.AddDays(-2),
                    UpdatedAt = DateTime.UtcNow.AddDays(-1)
                };

                var task4 = new TaskItem
                {
                    ProjectId = hrProject.ProjectId,
                    TaskTitle = "Digital Document Verification Form",
                    Description = "Implement file upload and employee identification document review workflow.",
                    AssignedTo = empPriya.UserId,
                    CreatedBy = managerUser.UserId,
                    Priority = "Medium",
                    Status = "In Progress",
                    StartDate = DateTime.Today.AddDays(-8),
                    DueDate = DateTime.Today.AddDays(7),
                    ProgressPercentage = 50,
                    EstimatedHours = 12,
                    ActualHours = 6,
                    CreatedAt = DateTime.UtcNow.AddDays(-8),
                    UpdatedAt = DateTime.UtcNow.AddDays(-2)
                };

                var task5 = new TaskItem
                {
                    ProjectId = finProject.ProjectId,
                    TaskTitle = "Tax Deduction at Source (TDS) Reconciliation",
                    Description = "Audit withholding tax certificates against quarterly bank deposits.",
                    AssignedTo = empAmit.UserId,
                    CreatedBy = managerUser.UserId,
                    Priority = "High",
                    Status = "In Progress",
                    StartDate = DateTime.Today.AddDays(-20),
                    DueDate = DateTime.Today.AddDays(-3), // OVERDUE
                    ProgressPercentage = 60,
                    EstimatedHours = 15,
                    ActualHours = 12,
                    CreatedAt = DateTime.UtcNow.AddDays(-20),
                    UpdatedAt = DateTime.UtcNow.AddDays(-3)
                };

                var task6 = new TaskItem
                {
                    ProjectId = erpProject.ProjectId,
                    TaskTitle = "Quarterly Campaign Lead Generation Matrix",
                    Description = "Compile campaign performance analytics and convert inbound inquiries.",
                    AssignedTo = empNeha.UserId,
                    CreatedBy = managerUser.UserId,
                    Priority = "Low",
                    Status = "Pending",
                    StartDate = DateTime.Today,
                    DueDate = DateTime.Today.AddDays(14),
                    ProgressPercentage = 0,
                    EstimatedHours = 10,
                    ActualHours = 0,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                context.Tasks.AddRange(task1, task2, task3, task4, task5, task6);
                await context.SaveChangesAsync();

                // 5. Seed Comments & Activity Logs
                context.TaskComments.Add(new TaskComment
                {
                    TaskId = task2.TaskId,
                    UserId = empRahul.UserId,
                    CommentText = "Authentication pipeline configured with Role claims and anti-forgery tokens.",
                    CreatedAt = DateTime.UtcNow.AddDays(-2)
                });

                context.TaskComments.Add(new TaskComment
                {
                    TaskId = task2.TaskId,
                    UserId = managerUser.UserId,
                    CommentText = "Great progress Rahul! Ensure access denied redirections handle unauthorized views smoothly.",
                    CreatedAt = DateTime.UtcNow.AddDays(-1)
                });

                context.TaskActivityLogs.Add(new TaskActivityLog
                {
                    TaskId = task2.TaskId,
                    UserId = empRahul.UserId,
                    Action = "Status Changed",
                    OldValue = "Pending",
                    NewValue = "In Progress",
                    CreatedAt = DateTime.UtcNow.AddDays(-5)
                });

                context.TaskActivityLogs.Add(new TaskActivityLog
                {
                    TaskId = task2.TaskId,
                    UserId = empRahul.UserId,
                    Action = "Progress Updated",
                    OldValue = "40%",
                    NewValue = "80%",
                    CreatedAt = DateTime.UtcNow.AddDays(-1)
                });

                // 6. Seed Notifications
                context.Notifications.Add(new Notification
                {
                    UserId = empRahul.UserId,
                    Title = "Task Assigned",
                    Message = "You have been assigned: Develop Productivity Calculation Engine",
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow.AddDays(-2)
                });

                context.Notifications.Add(new Notification
                {
                    UserId = empAmit.UserId,
                    Title = "Task Overdue Warning",
                    Message = "Task 'Tax Deduction at Source (TDS) Reconciliation' is overdue.",
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow.AddDays(-1)
                });

                await context.SaveChangesAsync();
                logger.LogInformation("Database seeded successfully!");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while seeding the database.");
                throw;
            }
        }
    }
}
