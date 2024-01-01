using System;
using System.Data;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Components;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Radzen;

using TaskManager.Server.Data;

namespace TaskManager.Server
{
    public partial class TaskManagerDbService
    {
        TaskManagerDbContext Context
        {
           get
           {
             return this.context;
           }
        }

        private readonly TaskManagerDbContext context;
        private readonly NavigationManager navigationManager;

        public TaskManagerDbService(TaskManagerDbContext context, NavigationManager navigationManager)
        {
            this.context = context;
            this.navigationManager = navigationManager;
        }

        public void Reset() => Context.ChangeTracker.Entries().Where(e => e.Entity != null).ToList().ForEach(e => e.State = EntityState.Detached);

        public void ApplyQuery<T>(ref IQueryable<T> items, Query query = null)
        {
            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Filter))
                {
                    if (query.FilterParameters != null)
                    {
                        items = items.Where(query.Filter, query.FilterParameters);
                    }
                    else
                    {
                        items = items.Where(query.Filter);
                    }
                }

                if (!string.IsNullOrEmpty(query.OrderBy))
                {
                    items = items.OrderBy(query.OrderBy);
                }

                if (query.Skip.HasValue)
                {
                    items = items.Skip(query.Skip.Value);
                }

                if (query.Top.HasValue)
                {
                    items = items.Take(query.Top.Value);
                }
            }
        }


        public async Task ExportNotificationsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/taskmanagerdb/notifications/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/taskmanagerdb/notifications/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportNotificationsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/taskmanagerdb/notifications/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/taskmanagerdb/notifications/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnNotificationsRead(ref IQueryable<TaskManager.Server.Models.TaskManagerDb.Notification> items);

        public async Task<IQueryable<TaskManager.Server.Models.TaskManagerDb.Notification>> GetNotifications(Query query = null)
        {
            var items = Context.Notifications.AsQueryable();

            items = items.Include(i => i.Task);
            items = items.Include(i => i.User);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnNotificationsRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnNotificationGet(TaskManager.Server.Models.TaskManagerDb.Notification item);
        partial void OnGetNotificationByNotificationId(ref IQueryable<TaskManager.Server.Models.TaskManagerDb.Notification> items);


        public async Task<TaskManager.Server.Models.TaskManagerDb.Notification> GetNotificationByNotificationId(Guid notificationid)
        {
            var items = Context.Notifications
                              .AsNoTracking()
                              .Where(i => i.NotificationID == notificationid);

            items = items.Include(i => i.Task);
            items = items.Include(i => i.User);
 
            OnGetNotificationByNotificationId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnNotificationGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnNotificationCreated(TaskManager.Server.Models.TaskManagerDb.Notification item);
        partial void OnAfterNotificationCreated(TaskManager.Server.Models.TaskManagerDb.Notification item);

        public async Task<TaskManager.Server.Models.TaskManagerDb.Notification> CreateNotification(TaskManager.Server.Models.TaskManagerDb.Notification notification)
        {
            OnNotificationCreated(notification);

            var existingItem = Context.Notifications
                              .Where(i => i.NotificationID == notification.NotificationID)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.Notifications.Add(notification);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(notification).State = EntityState.Detached;
                throw;
            }

            OnAfterNotificationCreated(notification);

            return notification;
        }

        public async Task<TaskManager.Server.Models.TaskManagerDb.Notification> CancelNotificationChanges(TaskManager.Server.Models.TaskManagerDb.Notification item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnNotificationUpdated(TaskManager.Server.Models.TaskManagerDb.Notification item);
        partial void OnAfterNotificationUpdated(TaskManager.Server.Models.TaskManagerDb.Notification item);

        public async Task<TaskManager.Server.Models.TaskManagerDb.Notification> UpdateNotification(Guid notificationid, TaskManager.Server.Models.TaskManagerDb.Notification notification)
        {
            OnNotificationUpdated(notification);

            var itemToUpdate = Context.Notifications
                              .Where(i => i.NotificationID == notification.NotificationID)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(notification);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterNotificationUpdated(notification);

            return notification;
        }

        partial void OnNotificationDeleted(TaskManager.Server.Models.TaskManagerDb.Notification item);
        partial void OnAfterNotificationDeleted(TaskManager.Server.Models.TaskManagerDb.Notification item);

        public async Task<TaskManager.Server.Models.TaskManagerDb.Notification> DeleteNotification(Guid notificationid)
        {
            var itemToDelete = Context.Notifications
                              .Where(i => i.NotificationID == notificationid)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnNotificationDeleted(itemToDelete);


            Context.Notifications.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterNotificationDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportRolesToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/taskmanagerdb/roles/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/taskmanagerdb/roles/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportRolesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/taskmanagerdb/roles/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/taskmanagerdb/roles/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnRolesRead(ref IQueryable<TaskManager.Server.Models.TaskManagerDb.Role> items);

        public async Task<IQueryable<TaskManager.Server.Models.TaskManagerDb.Role>> GetRoles(Query query = null)
        {
            var items = Context.Roles.AsQueryable();


            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnRolesRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnRoleGet(TaskManager.Server.Models.TaskManagerDb.Role item);
        partial void OnGetRoleByRoleId(ref IQueryable<TaskManager.Server.Models.TaskManagerDb.Role> items);


        public async Task<TaskManager.Server.Models.TaskManagerDb.Role> GetRoleByRoleId(Guid roleid)
        {
            var items = Context.Roles
                              .AsNoTracking()
                              .Where(i => i.RoleID == roleid);

 
            OnGetRoleByRoleId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnRoleGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnRoleCreated(TaskManager.Server.Models.TaskManagerDb.Role item);
        partial void OnAfterRoleCreated(TaskManager.Server.Models.TaskManagerDb.Role item);

        public async Task<TaskManager.Server.Models.TaskManagerDb.Role> CreateRole(TaskManager.Server.Models.TaskManagerDb.Role role)
        {
            OnRoleCreated(role);

            var existingItem = Context.Roles
                              .Where(i => i.RoleID == role.RoleID)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.Roles.Add(role);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(role).State = EntityState.Detached;
                throw;
            }

            OnAfterRoleCreated(role);

            return role;
        }

        public async Task<TaskManager.Server.Models.TaskManagerDb.Role> CancelRoleChanges(TaskManager.Server.Models.TaskManagerDb.Role item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnRoleUpdated(TaskManager.Server.Models.TaskManagerDb.Role item);
        partial void OnAfterRoleUpdated(TaskManager.Server.Models.TaskManagerDb.Role item);

        public async Task<TaskManager.Server.Models.TaskManagerDb.Role> UpdateRole(Guid roleid, TaskManager.Server.Models.TaskManagerDb.Role role)
        {
            OnRoleUpdated(role);

            var itemToUpdate = Context.Roles
                              .Where(i => i.RoleID == role.RoleID)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(role);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterRoleUpdated(role);

            return role;
        }

        partial void OnRoleDeleted(TaskManager.Server.Models.TaskManagerDb.Role item);
        partial void OnAfterRoleDeleted(TaskManager.Server.Models.TaskManagerDb.Role item);

        public async Task<TaskManager.Server.Models.TaskManagerDb.Role> DeleteRole(Guid roleid)
        {
            var itemToDelete = Context.Roles
                              .Where(i => i.RoleID == roleid)
                              .Include(i => i.Users)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnRoleDeleted(itemToDelete);


            Context.Roles.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterRoleDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportTaskHistoriesToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/taskmanagerdb/taskhistories/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/taskmanagerdb/taskhistories/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportTaskHistoriesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/taskmanagerdb/taskhistories/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/taskmanagerdb/taskhistories/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnTaskHistoriesRead(ref IQueryable<TaskManager.Server.Models.TaskManagerDb.TaskHistory> items);

        public async Task<IQueryable<TaskManager.Server.Models.TaskManagerDb.TaskHistory>> GetTaskHistories(Query query = null)
        {
            var items = Context.TaskHistories.AsQueryable();

            items = items.Include(i => i.Task);
            items = items.Include(i => i.User);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnTaskHistoriesRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnTaskHistoryGet(TaskManager.Server.Models.TaskManagerDb.TaskHistory item);
        partial void OnGetTaskHistoryByHistoryId(ref IQueryable<TaskManager.Server.Models.TaskManagerDb.TaskHistory> items);


        public async Task<TaskManager.Server.Models.TaskManagerDb.TaskHistory> GetTaskHistoryByHistoryId(Guid historyid)
        {
            var items = Context.TaskHistories
                              .AsNoTracking()
                              .Where(i => i.HistoryID == historyid);

            items = items.Include(i => i.Task);
            items = items.Include(i => i.User);
 
            OnGetTaskHistoryByHistoryId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnTaskHistoryGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnTaskHistoryCreated(TaskManager.Server.Models.TaskManagerDb.TaskHistory item);
        partial void OnAfterTaskHistoryCreated(TaskManager.Server.Models.TaskManagerDb.TaskHistory item);

        public async Task<TaskManager.Server.Models.TaskManagerDb.TaskHistory> CreateTaskHistory(TaskManager.Server.Models.TaskManagerDb.TaskHistory taskhistory)
        {
            OnTaskHistoryCreated(taskhistory);

            var existingItem = Context.TaskHistories
                              .Where(i => i.HistoryID == taskhistory.HistoryID)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.TaskHistories.Add(taskhistory);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(taskhistory).State = EntityState.Detached;
                throw;
            }

            OnAfterTaskHistoryCreated(taskhistory);

            return taskhistory;
        }

        public async Task<TaskManager.Server.Models.TaskManagerDb.TaskHistory> CancelTaskHistoryChanges(TaskManager.Server.Models.TaskManagerDb.TaskHistory item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnTaskHistoryUpdated(TaskManager.Server.Models.TaskManagerDb.TaskHistory item);
        partial void OnAfterTaskHistoryUpdated(TaskManager.Server.Models.TaskManagerDb.TaskHistory item);

        public async Task<TaskManager.Server.Models.TaskManagerDb.TaskHistory> UpdateTaskHistory(Guid historyid, TaskManager.Server.Models.TaskManagerDb.TaskHistory taskhistory)
        {
            OnTaskHistoryUpdated(taskhistory);

            var itemToUpdate = Context.TaskHistories
                              .Where(i => i.HistoryID == taskhistory.HistoryID)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(taskhistory);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterTaskHistoryUpdated(taskhistory);

            return taskhistory;
        }

        partial void OnTaskHistoryDeleted(TaskManager.Server.Models.TaskManagerDb.TaskHistory item);
        partial void OnAfterTaskHistoryDeleted(TaskManager.Server.Models.TaskManagerDb.TaskHistory item);

        public async Task<TaskManager.Server.Models.TaskManagerDb.TaskHistory> DeleteTaskHistory(Guid historyid)
        {
            var itemToDelete = Context.TaskHistories
                              .Where(i => i.HistoryID == historyid)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnTaskHistoryDeleted(itemToDelete);


            Context.TaskHistories.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterTaskHistoryDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportTasksToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/taskmanagerdb/tasks/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/taskmanagerdb/tasks/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportTasksToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/taskmanagerdb/tasks/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/taskmanagerdb/tasks/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnTasksRead(ref IQueryable<TaskManager.Server.Models.TaskManagerDb.Task> items);

        public async Task<IQueryable<TaskManager.Server.Models.TaskManagerDb.Task>> GetTasks(Query query = null)
        {
            var items = Context.Tasks.AsQueryable();

            items = items.Include(i => i.User);
            items = items.Include(i => i.User1);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnTasksRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnTaskGet(TaskManager.Server.Models.TaskManagerDb.Task item);
        partial void OnGetTaskByTaskId(ref IQueryable<TaskManager.Server.Models.TaskManagerDb.Task> items);


        public async Task<TaskManager.Server.Models.TaskManagerDb.Task> GetTaskByTaskId(Guid taskid)
        {
            var items = Context.Tasks
                              .AsNoTracking()
                              .Where(i => i.TaskID == taskid);

            items = items.Include(i => i.User);
            items = items.Include(i => i.User1);
 
            OnGetTaskByTaskId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnTaskGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnTaskCreated(TaskManager.Server.Models.TaskManagerDb.Task item);
        partial void OnAfterTaskCreated(TaskManager.Server.Models.TaskManagerDb.Task item);

        public async Task<TaskManager.Server.Models.TaskManagerDb.Task> CreateTask(TaskManager.Server.Models.TaskManagerDb.Task task)
        {
            OnTaskCreated(task);

            var existingItem = Context.Tasks
                              .Where(i => i.TaskID == task.TaskID)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.Tasks.Add(task);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(task).State = EntityState.Detached;
                throw;
            }

            OnAfterTaskCreated(task);

            return task;
        }

        public async Task<TaskManager.Server.Models.TaskManagerDb.Task> CancelTaskChanges(TaskManager.Server.Models.TaskManagerDb.Task item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnTaskUpdated(TaskManager.Server.Models.TaskManagerDb.Task item);
        partial void OnAfterTaskUpdated(TaskManager.Server.Models.TaskManagerDb.Task item);

        public async Task<TaskManager.Server.Models.TaskManagerDb.Task> UpdateTask(Guid taskid, TaskManager.Server.Models.TaskManagerDb.Task task)
        {
            OnTaskUpdated(task);

            var itemToUpdate = Context.Tasks
                              .Where(i => i.TaskID == task.TaskID)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(task);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterTaskUpdated(task);

            return task;
        }

        partial void OnTaskDeleted(TaskManager.Server.Models.TaskManagerDb.Task item);
        partial void OnAfterTaskDeleted(TaskManager.Server.Models.TaskManagerDb.Task item);

        public async Task<TaskManager.Server.Models.TaskManagerDb.Task> DeleteTask(Guid taskid)
        {
            var itemToDelete = Context.Tasks
                              .Where(i => i.TaskID == taskid)
                              .Include(i => i.Notifications)
                              .Include(i => i.TaskHistories)
                              .Include(i => i.TaskSubPoints)
                              .Include(i => i.TimeLogs)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnTaskDeleted(itemToDelete);


            Context.Tasks.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterTaskDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportTaskSubPointsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/taskmanagerdb/tasksubpoints/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/taskmanagerdb/tasksubpoints/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportTaskSubPointsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/taskmanagerdb/tasksubpoints/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/taskmanagerdb/tasksubpoints/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnTaskSubPointsRead(ref IQueryable<TaskManager.Server.Models.TaskManagerDb.TaskSubPoint> items);

        public async Task<IQueryable<TaskManager.Server.Models.TaskManagerDb.TaskSubPoint>> GetTaskSubPoints(Query query = null)
        {
            var items = Context.TaskSubPoints.AsQueryable();

            items = items.Include(i => i.Task);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnTaskSubPointsRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnTaskSubPointGet(TaskManager.Server.Models.TaskManagerDb.TaskSubPoint item);
        partial void OnGetTaskSubPointBySubPointId(ref IQueryable<TaskManager.Server.Models.TaskManagerDb.TaskSubPoint> items);


        public async Task<TaskManager.Server.Models.TaskManagerDb.TaskSubPoint> GetTaskSubPointBySubPointId(Guid subpointid)
        {
            var items = Context.TaskSubPoints
                              .AsNoTracking()
                              .Where(i => i.SubPointID == subpointid);

            items = items.Include(i => i.Task);
 
            OnGetTaskSubPointBySubPointId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnTaskSubPointGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnTaskSubPointCreated(TaskManager.Server.Models.TaskManagerDb.TaskSubPoint item);
        partial void OnAfterTaskSubPointCreated(TaskManager.Server.Models.TaskManagerDb.TaskSubPoint item);

        public async Task<TaskManager.Server.Models.TaskManagerDb.TaskSubPoint> CreateTaskSubPoint(TaskManager.Server.Models.TaskManagerDb.TaskSubPoint tasksubpoint)
        {
            OnTaskSubPointCreated(tasksubpoint);

            var existingItem = Context.TaskSubPoints
                              .Where(i => i.SubPointID == tasksubpoint.SubPointID)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.TaskSubPoints.Add(tasksubpoint);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(tasksubpoint).State = EntityState.Detached;
                throw;
            }

            OnAfterTaskSubPointCreated(tasksubpoint);

            return tasksubpoint;
        }

        public async Task<TaskManager.Server.Models.TaskManagerDb.TaskSubPoint> CancelTaskSubPointChanges(TaskManager.Server.Models.TaskManagerDb.TaskSubPoint item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnTaskSubPointUpdated(TaskManager.Server.Models.TaskManagerDb.TaskSubPoint item);
        partial void OnAfterTaskSubPointUpdated(TaskManager.Server.Models.TaskManagerDb.TaskSubPoint item);

        public async Task<TaskManager.Server.Models.TaskManagerDb.TaskSubPoint> UpdateTaskSubPoint(Guid subpointid, TaskManager.Server.Models.TaskManagerDb.TaskSubPoint tasksubpoint)
        {
            OnTaskSubPointUpdated(tasksubpoint);

            var itemToUpdate = Context.TaskSubPoints
                              .Where(i => i.SubPointID == tasksubpoint.SubPointID)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(tasksubpoint);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterTaskSubPointUpdated(tasksubpoint);

            return tasksubpoint;
        }

        partial void OnTaskSubPointDeleted(TaskManager.Server.Models.TaskManagerDb.TaskSubPoint item);
        partial void OnAfterTaskSubPointDeleted(TaskManager.Server.Models.TaskManagerDb.TaskSubPoint item);

        public async Task<TaskManager.Server.Models.TaskManagerDb.TaskSubPoint> DeleteTaskSubPoint(Guid subpointid)
        {
            var itemToDelete = Context.TaskSubPoints
                              .Where(i => i.SubPointID == subpointid)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnTaskSubPointDeleted(itemToDelete);


            Context.TaskSubPoints.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterTaskSubPointDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportTimeLogsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/taskmanagerdb/timelogs/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/taskmanagerdb/timelogs/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportTimeLogsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/taskmanagerdb/timelogs/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/taskmanagerdb/timelogs/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnTimeLogsRead(ref IQueryable<TaskManager.Server.Models.TaskManagerDb.TimeLog> items);

        public async Task<IQueryable<TaskManager.Server.Models.TaskManagerDb.TimeLog>> GetTimeLogs(Query query = null)
        {
            var items = Context.TimeLogs.AsQueryable();

            items = items.Include(i => i.Task);
            items = items.Include(i => i.User);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnTimeLogsRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnTimeLogGet(TaskManager.Server.Models.TaskManagerDb.TimeLog item);
        partial void OnGetTimeLogByTimeLogId(ref IQueryable<TaskManager.Server.Models.TaskManagerDb.TimeLog> items);


        public async Task<TaskManager.Server.Models.TaskManagerDb.TimeLog> GetTimeLogByTimeLogId(Guid timelogid)
        {
            var items = Context.TimeLogs
                              .AsNoTracking()
                              .Where(i => i.TimeLogID == timelogid);

            items = items.Include(i => i.Task);
            items = items.Include(i => i.User);
 
            OnGetTimeLogByTimeLogId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnTimeLogGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnTimeLogCreated(TaskManager.Server.Models.TaskManagerDb.TimeLog item);
        partial void OnAfterTimeLogCreated(TaskManager.Server.Models.TaskManagerDb.TimeLog item);

        public async Task<TaskManager.Server.Models.TaskManagerDb.TimeLog> CreateTimeLog(TaskManager.Server.Models.TaskManagerDb.TimeLog timelog)
        {
            OnTimeLogCreated(timelog);

            var existingItem = Context.TimeLogs
                              .Where(i => i.TimeLogID == timelog.TimeLogID)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.TimeLogs.Add(timelog);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(timelog).State = EntityState.Detached;
                throw;
            }

            OnAfterTimeLogCreated(timelog);

            return timelog;
        }

        public async Task<TaskManager.Server.Models.TaskManagerDb.TimeLog> CancelTimeLogChanges(TaskManager.Server.Models.TaskManagerDb.TimeLog item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnTimeLogUpdated(TaskManager.Server.Models.TaskManagerDb.TimeLog item);
        partial void OnAfterTimeLogUpdated(TaskManager.Server.Models.TaskManagerDb.TimeLog item);

        public async Task<TaskManager.Server.Models.TaskManagerDb.TimeLog> UpdateTimeLog(Guid timelogid, TaskManager.Server.Models.TaskManagerDb.TimeLog timelog)
        {
            OnTimeLogUpdated(timelog);

            var itemToUpdate = Context.TimeLogs
                              .Where(i => i.TimeLogID == timelog.TimeLogID)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(timelog);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterTimeLogUpdated(timelog);

            return timelog;
        }

        partial void OnTimeLogDeleted(TaskManager.Server.Models.TaskManagerDb.TimeLog item);
        partial void OnAfterTimeLogDeleted(TaskManager.Server.Models.TaskManagerDb.TimeLog item);

        public async Task<TaskManager.Server.Models.TaskManagerDb.TimeLog> DeleteTimeLog(Guid timelogid)
        {
            var itemToDelete = Context.TimeLogs
                              .Where(i => i.TimeLogID == timelogid)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnTimeLogDeleted(itemToDelete);


            Context.TimeLogs.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterTimeLogDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportUsersToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/taskmanagerdb/users/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/taskmanagerdb/users/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportUsersToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/taskmanagerdb/users/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/taskmanagerdb/users/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnUsersRead(ref IQueryable<TaskManager.Server.Models.TaskManagerDb.User> items);

        public async Task<IQueryable<TaskManager.Server.Models.TaskManagerDb.User>> GetUsers(Query query = null)
        {
            var items = Context.Users.AsQueryable();

            items = items.Include(i => i.Role);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnUsersRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnUserGet(TaskManager.Server.Models.TaskManagerDb.User item);
        partial void OnGetUserByUserId(ref IQueryable<TaskManager.Server.Models.TaskManagerDb.User> items);


        public async Task<TaskManager.Server.Models.TaskManagerDb.User> GetUserByUserId(Guid userid)
        {
            var items = Context.Users
                              .AsNoTracking()
                              .Where(i => i.UserID == userid);

            items = items.Include(i => i.Role);
 
            OnGetUserByUserId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnUserGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnUserCreated(TaskManager.Server.Models.TaskManagerDb.User item);
        partial void OnAfterUserCreated(TaskManager.Server.Models.TaskManagerDb.User item);

        public async Task<TaskManager.Server.Models.TaskManagerDb.User> CreateUser(TaskManager.Server.Models.TaskManagerDb.User user)
        {
            OnUserCreated(user);

            var existingItem = Context.Users
                              .Where(i => i.UserID == user.UserID)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.Users.Add(user);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(user).State = EntityState.Detached;
                throw;
            }

            OnAfterUserCreated(user);

            return user;
        }

        public async Task<TaskManager.Server.Models.TaskManagerDb.User> CancelUserChanges(TaskManager.Server.Models.TaskManagerDb.User item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnUserUpdated(TaskManager.Server.Models.TaskManagerDb.User item);
        partial void OnAfterUserUpdated(TaskManager.Server.Models.TaskManagerDb.User item);

        public async Task<TaskManager.Server.Models.TaskManagerDb.User> UpdateUser(Guid userid, TaskManager.Server.Models.TaskManagerDb.User user)
        {
            OnUserUpdated(user);

            var itemToUpdate = Context.Users
                              .Where(i => i.UserID == user.UserID)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(user);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterUserUpdated(user);

            return user;
        }

        partial void OnUserDeleted(TaskManager.Server.Models.TaskManagerDb.User item);
        partial void OnAfterUserDeleted(TaskManager.Server.Models.TaskManagerDb.User item);

        public async Task<TaskManager.Server.Models.TaskManagerDb.User> DeleteUser(Guid userid)
        {
            var itemToDelete = Context.Users
                              .Where(i => i.UserID == userid)
                              .Include(i => i.Notifications)
                              .Include(i => i.TaskHistories)
                              .Include(i => i.Tasks)
                              .Include(i => i.Tasks1)
                              .Include(i => i.TimeLogs)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnUserDeleted(itemToDelete);


            Context.Users.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterUserDeleted(itemToDelete);

            return itemToDelete;
        }
        }
}