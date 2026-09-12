using EmployeeManagement.Models;
using EmployeeManagement.Repositories;
using EmployeeManagement.ViewModels;

namespace EmployeeManagement.Services
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepository;
        private readonly INotificationRepository _notificationRepository;
        private readonly IUserRepository _userRepository;

        public TaskService(
            ITaskRepository taskRepository,
            INotificationRepository notificationRepository,
            IUserRepository userRepository)
        {
            _taskRepository = taskRepository;
            _notificationRepository = notificationRepository;
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<TaskItem>> GetAllTasksAsync()
        {
            return await _taskRepository.GetAllWithDetailsAsync();
        }

        public async Task<TaskItem?> GetTaskByIdAsync(int id)
        {
            return await _taskRepository.GetByIdWithDetailsAsync(id);
        }

        public async Task<IEnumerable<TaskItem>> GetTasksByAssigneeAsync(int employeeId)
        {
            return await _taskRepository.GetTasksByAssigneeAsync(employeeId);
        }

        public async Task<IEnumerable<TaskItem>> GetTasksByProjectAsync(int projectId)
        {
            return await _taskRepository.GetTasksByProjectAsync(projectId);
        }

        public async Task<IEnumerable<TaskItem>> GetFilteredTasksAsync(TaskFilterViewModel filter)
        {
            return await _taskRepository.GetFilteredTasksAsync(
                filter.SearchTerm,
                filter.Status,
                filter.Priority,
                filter.AssignedTo,
                filter.ProjectId,
                filter.FromDate,
                filter.ToDate);
        }

        public async Task<TaskItem> CreateTaskAsync(TaskFormViewModel model, int creatorUserId)
        {
            // Business rule: Completed tasks are 100%
            DateTime? completedDate = null;
            if (model.Status == "Completed" || model.ProgressPercentage == 100)
            {
                model.Status = "Completed";
                model.ProgressPercentage = 100;
                completedDate = DateTime.Today;
            }

            var task = new TaskItem
            {
                ProjectId = model.ProjectId,
                TaskTitle = model.TaskTitle,
                Description = model.Description,
                AssignedTo = model.AssignedTo,
                CreatedBy = creatorUserId,
                Priority = model.Priority,
                Status = model.Status,
                StartDate = model.StartDate,
                DueDate = model.DueDate,
                CompletedDate = completedDate,
                ProgressPercentage = model.ProgressPercentage,
                EstimatedHours = model.EstimatedHours,
                ActualHours = model.ActualHours,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _taskRepository.AddAsync(task);
            await _taskRepository.SaveChangesAsync();

            // Activity Log
            await _taskRepository.AddActivityLogAsync(new TaskActivityLog
            {
                TaskId = task.TaskId,
                UserId = creatorUserId,
                Action = "Task Created",
                NewValue = task.TaskTitle,
                CreatedAt = DateTime.UtcNow
            });

            // Notification for assigned employee
            await _notificationRepository.AddAsync(new Notification
            {
                UserId = model.AssignedTo,
                Title = "New Task Assigned",
                Message = $"You have been assigned task '{task.TaskTitle}'.",
                CreatedAt = DateTime.UtcNow
            });

            await _taskRepository.SaveChangesAsync();

            return task;
        }

        public async Task<bool> UpdateTaskAsync(TaskFormViewModel model, int currentUserId)
        {
            var task = await _taskRepository.GetByIdAsync(model.TaskId);
            if (task == null) return false;

            // Track changes for activity log
            if (task.Status != model.Status)
            {
                await _taskRepository.AddActivityLogAsync(new TaskActivityLog
                {
                    TaskId = task.TaskId,
                    UserId = currentUserId,
                    Action = "Status Changed",
                    OldValue = task.Status,
                    NewValue = model.Status,
                    CreatedAt = DateTime.UtcNow
                });
            }

            if (task.ProgressPercentage != model.ProgressPercentage)
            {
                await _taskRepository.AddActivityLogAsync(new TaskActivityLog
                {
                    TaskId = task.TaskId,
                    UserId = currentUserId,
                    Action = "Progress Updated",
                    OldValue = $"{task.ProgressPercentage}%",
                    NewValue = $"{model.ProgressPercentage}%",
                    CreatedAt = DateTime.UtcNow
                });
            }

            if (task.AssignedTo != model.AssignedTo)
            {
                await _taskRepository.AddActivityLogAsync(new TaskActivityLog
                {
                    TaskId = task.TaskId,
                    UserId = currentUserId,
                    Action = "Reassigned",
                    OldValue = task.AssignedTo.ToString(),
                    NewValue = model.AssignedTo.ToString(),
                    CreatedAt = DateTime.UtcNow
                });

                // Notify newly assigned user
                await _notificationRepository.AddAsync(new Notification
                {
                    UserId = model.AssignedTo,
                    Title = "Task Reassigned",
                    Message = $"Task '{task.TaskTitle}' has been reassigned to you.",
                    CreatedAt = DateTime.UtcNow
                });
            }

            // Apply Business Rules
            if (model.Status == "Completed")
            {
                task.ProgressPercentage = 100;
                task.CompletedDate = DateTime.Today;

                // Notify creator
                await _notificationRepository.AddAsync(new Notification
                {
                    UserId = task.CreatedBy,
                    Title = "Task Completed",
                    Message = $"Task '{task.TaskTitle}' was marked as completed.",
                    CreatedAt = DateTime.UtcNow
                });
            }
            else
            {
                task.ProgressPercentage = model.ProgressPercentage;
                if (task.Status == "Completed" && model.Status != "Completed")
                {
                    task.CompletedDate = null;
                }
            }

            task.ProjectId = model.ProjectId;
            task.TaskTitle = model.TaskTitle;
            task.Description = model.Description;
            task.AssignedTo = model.AssignedTo;
            task.Priority = model.Priority;
            task.Status = model.Status;
            task.StartDate = model.StartDate;
            task.DueDate = model.DueDate;
            task.EstimatedHours = model.EstimatedHours;
            task.ActualHours = model.ActualHours;
            task.UpdatedAt = DateTime.UtcNow;

            _taskRepository.Update(task);
            await _taskRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateProgressAsync(TaskUpdateProgressViewModel model, int currentUserId)
        {
            var task = await _taskRepository.GetByIdAsync(model.TaskId);
            if (task == null) return false;

            // Business Rules
            if (model.Status == "Completed" || model.ProgressPercentage == 100)
            {
                model.Status = "Completed";
                model.ProgressPercentage = 100;
                task.CompletedDate = DateTime.Today;

                // Notify creator
                await _notificationRepository.AddAsync(new Notification
                {
                    UserId = task.CreatedBy,
                    Title = "Task Completed",
                    Message = $"Task '{task.TaskTitle}' has been completed.",
                    CreatedAt = DateTime.UtcNow
                });
            }
            else
            {
                if (task.Status == "Completed" && model.Status != "Completed")
                {
                    task.CompletedDate = null;
                }
            }

            if (task.Status != model.Status)
            {
                await _taskRepository.AddActivityLogAsync(new TaskActivityLog
                {
                    TaskId = task.TaskId,
                    UserId = currentUserId,
                    Action = "Status Changed",
                    OldValue = task.Status,
                    NewValue = model.Status,
                    CreatedAt = DateTime.UtcNow
                });
            }

            if (task.ProgressPercentage != model.ProgressPercentage)
            {
                await _taskRepository.AddActivityLogAsync(new TaskActivityLog
                {
                    TaskId = task.TaskId,
                    UserId = currentUserId,
                    Action = "Progress Updated",
                    OldValue = $"{task.ProgressPercentage}%",
                    NewValue = $"{model.ProgressPercentage}%",
                    CreatedAt = DateTime.UtcNow
                });
            }

            task.Status = model.Status;
            task.ProgressPercentage = model.ProgressPercentage;
            task.ActualHours = model.ActualHours;
            task.UpdatedAt = DateTime.UtcNow;

            if (!string.IsNullOrWhiteSpace(model.Comment))
            {
                await _taskRepository.AddCommentAsync(new TaskComment
                {
                    TaskId = task.TaskId,
                    UserId = currentUserId,
                    CommentText = model.Comment.Trim(),
                    CreatedAt = DateTime.UtcNow
                });
            }

            _taskRepository.Update(task);
            await _taskRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteTaskAsync(int taskId)
        {
            var task = await _taskRepository.GetByIdAsync(taskId);
            if (task == null) return false;

            _taskRepository.Remove(task);
            await _taskRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AddCommentAsync(int taskId, int userId, string commentText)
        {
            if (string.IsNullOrWhiteSpace(commentText)) return false;

            var comment = new TaskComment
            {
                TaskId = taskId,
                UserId = userId,
                CommentText = commentText.Trim(),
                CreatedAt = DateTime.UtcNow
            };

            await _taskRepository.AddCommentAsync(comment);
            await _taskRepository.SaveChangesAsync();
            return true;
        }

        public async Task<(bool Success, string Message)> MoveTaskStatusAsync(int taskId, string newStatus, int currentUserId)
        {
            var task = await _taskRepository.GetByIdAsync(taskId);
            if (task == null) return (false, "Task not found.");

            var validStatuses = new[] { "Pending", "In Progress", "Completed", "On Hold", "Cancelled" };
            var matchedStatus = validStatuses.FirstOrDefault(s => s.Equals(newStatus, StringComparison.OrdinalIgnoreCase));
            if (matchedStatus == null) return (false, "Invalid target status.");

            var oldStatus = task.Status;
            if (oldStatus.Equals(matchedStatus, StringComparison.OrdinalIgnoreCase))
            {
                return (true, "Status unchanged.");
            }

            // Workflow Step Ordering:
            // 1: Pending, 2: In Progress, 3: Completed
            int GetStep(string status) => status switch
            {
                "Pending" => 1,
                "In Progress" => 2,
                "Completed" => 3,
                _ => 0
            };

            int oldStep = GetStep(oldStatus);
            int newStep = GetStep(matchedStatus);

            var user = await _userRepository.GetByIdAsync(currentUserId);
            bool isEmployee = user != null && user.RoleId == 3; // 3 = Employee

            // Business Rule: No employee can move a task backward after forwarding to another state
            if (isEmployee && newStep > 0 && oldStep > 0 && newStep < oldStep)
            {
                return (false, "Employees cannot move tasks backward after forwarding to another state.");
            }

            // Apply Business Rules
            if (matchedStatus == "Completed")
            {
                task.Status = "Completed";
                task.ProgressPercentage = 100;
                task.CompletedDate = DateTime.Today;

                if (task.CreatedBy != currentUserId)
                {
                    await _notificationRepository.AddAsync(new Notification
                    {
                        UserId = task.CreatedBy,
                        Title = "Task Completed",
                        Message = $"Task '{task.TaskTitle}' was completed.",
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }
            else if (matchedStatus == "In Progress")
            {
                task.Status = "In Progress";
                if (oldStatus == "Completed")
                {
                    task.CompletedDate = null;
                    if (task.ProgressPercentage >= 100) task.ProgressPercentage = 80;
                }
                else if (oldStatus == "Pending" && task.ProgressPercentage == 0)
                {
                    task.ProgressPercentage = 25;
                }
            }
            else if (matchedStatus == "Pending")
            {
                task.Status = "Pending";
                if (oldStatus == "Completed")
                {
                    task.CompletedDate = null;
                }
                if (task.ProgressPercentage == 100)
                {
                    task.ProgressPercentage = 0;
                }
            }
            else
            {
                task.Status = matchedStatus;
            }

            task.UpdatedAt = DateTime.UtcNow;

            // Activity Log
            await _taskRepository.AddActivityLogAsync(new TaskActivityLog
            {
                TaskId = task.TaskId,
                UserId = currentUserId,
                Action = "Status Changed (Drag & Drop)",
                OldValue = oldStatus,
                NewValue = task.Status,
                CreatedAt = DateTime.UtcNow
            });

            _taskRepository.Update(task);
            await _taskRepository.SaveChangesAsync();
            return (true, $"Task moved from {oldStatus} to {task.Status}.");
        }
    }
}
