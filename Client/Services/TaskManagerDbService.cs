
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Web;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;
using Radzen;

namespace TaskManager.Client
{
    public partial class TaskManagerDbService
    {
        private readonly HttpClient httpClient;
        private readonly Uri baseUri;
        private readonly NavigationManager navigationManager;

        public TaskManagerDbService(NavigationManager navigationManager, HttpClient httpClient, IConfiguration configuration)
        {
            this.httpClient = httpClient;

            this.navigationManager = navigationManager;
            this.baseUri = new Uri($"{navigationManager.BaseUri}odata/TaskManagerDb/");
        }


        public async System.Threading.Tasks.Task ExportNotificationsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/taskmanagerdb/notifications/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/taskmanagerdb/notifications/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async System.Threading.Tasks.Task ExportNotificationsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/taskmanagerdb/notifications/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/taskmanagerdb/notifications/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnGetNotifications(HttpRequestMessage requestMessage);

        public async Task<Radzen.ODataServiceResult<TaskManager.Server.Models.TaskManagerDb.Notification>> GetNotifications(Query query)
        {
            return await GetNotifications(filter:$"{query.Filter}", orderby:$"{query.OrderBy}", top:query.Top, skip:query.Skip, count:query.Top != null && query.Skip != null);
        }

        public async Task<Radzen.ODataServiceResult<TaskManager.Server.Models.TaskManagerDb.Notification>> GetNotifications(string filter = default(string), string orderby = default(string), string expand = default(string), int? top = default(int?), int? skip = default(int?), bool? count = default(bool?), string format = default(string), string select = default(string))
        {
            var uri = new Uri(baseUri, $"Notifications");
            uri = Radzen.ODataExtensions.GetODataUri(uri: uri, filter:filter, top:top, skip:skip, orderby:orderby, expand:expand, select:select, count:count);

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            OnGetNotifications(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<Radzen.ODataServiceResult<TaskManager.Server.Models.TaskManagerDb.Notification>>(response);
        }

        partial void OnCreateNotification(HttpRequestMessage requestMessage);

        public async Task<TaskManager.Server.Models.TaskManagerDb.Notification> CreateNotification(TaskManager.Server.Models.TaskManagerDb.Notification notification = default(TaskManager.Server.Models.TaskManagerDb.Notification))
        {
            var uri = new Uri(baseUri, $"Notifications");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, uri);

            httpRequestMessage.Content = new StringContent(Radzen.ODataJsonSerializer.Serialize(notification), Encoding.UTF8, "application/json");

            OnCreateNotification(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<TaskManager.Server.Models.TaskManagerDb.Notification>(response);
        }

        partial void OnDeleteNotification(HttpRequestMessage requestMessage);

        public async Task<HttpResponseMessage> DeleteNotification(Guid notificationId = default(Guid))
        {
            var uri = new Uri(baseUri, $"Notifications({notificationId})");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Delete, uri);

            OnDeleteNotification(httpRequestMessage);

            return await httpClient.SendAsync(httpRequestMessage);
        }

        partial void OnGetNotificationByNotificationId(HttpRequestMessage requestMessage);

        public async Task<TaskManager.Server.Models.TaskManagerDb.Notification> GetNotificationByNotificationId(string expand = default(string), Guid notificationId = default(Guid))
        {
            var uri = new Uri(baseUri, $"Notifications({notificationId})");

            uri = Radzen.ODataExtensions.GetODataUri(uri: uri, filter:null, top:null, skip:null, orderby:null, expand:expand, select:null, count:null);

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            OnGetNotificationByNotificationId(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<TaskManager.Server.Models.TaskManagerDb.Notification>(response);
        }

        partial void OnUpdateNotification(HttpRequestMessage requestMessage);
        
        public async Task<HttpResponseMessage> UpdateNotification(Guid notificationId = default(Guid), TaskManager.Server.Models.TaskManagerDb.Notification notification = default(TaskManager.Server.Models.TaskManagerDb.Notification))
        {
            var uri = new Uri(baseUri, $"Notifications({notificationId})");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Patch, uri);


            httpRequestMessage.Content = new StringContent(Radzen.ODataJsonSerializer.Serialize(notification), Encoding.UTF8, "application/json");

            OnUpdateNotification(httpRequestMessage);

            return await httpClient.SendAsync(httpRequestMessage);
        }

        public async System.Threading.Tasks.Task ExportRolesToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/taskmanagerdb/roles/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/taskmanagerdb/roles/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async System.Threading.Tasks.Task ExportRolesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/taskmanagerdb/roles/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/taskmanagerdb/roles/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnGetRoles(HttpRequestMessage requestMessage);

        public async Task<Radzen.ODataServiceResult<TaskManager.Server.Models.TaskManagerDb.Role>> GetRoles(Query query)
        {
            return await GetRoles(filter:$"{query.Filter}", orderby:$"{query.OrderBy}", top:query.Top, skip:query.Skip, count:query.Top != null && query.Skip != null);
        }

        public async Task<Radzen.ODataServiceResult<TaskManager.Server.Models.TaskManagerDb.Role>> GetRoles(string filter = default(string), string orderby = default(string), string expand = default(string), int? top = default(int?), int? skip = default(int?), bool? count = default(bool?), string format = default(string), string select = default(string))
        {
            var uri = new Uri(baseUri, $"Roles");
            uri = Radzen.ODataExtensions.GetODataUri(uri: uri, filter:filter, top:top, skip:skip, orderby:orderby, expand:expand, select:select, count:count);

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            OnGetRoles(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<Radzen.ODataServiceResult<TaskManager.Server.Models.TaskManagerDb.Role>>(response);
        }

        partial void OnCreateRole(HttpRequestMessage requestMessage);

        public async Task<TaskManager.Server.Models.TaskManagerDb.Role> CreateRole(TaskManager.Server.Models.TaskManagerDb.Role role = default(TaskManager.Server.Models.TaskManagerDb.Role))
        {
            var uri = new Uri(baseUri, $"Roles");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, uri);

            httpRequestMessage.Content = new StringContent(Radzen.ODataJsonSerializer.Serialize(role), Encoding.UTF8, "application/json");

            OnCreateRole(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<TaskManager.Server.Models.TaskManagerDb.Role>(response);
        }

        partial void OnDeleteRole(HttpRequestMessage requestMessage);

        public async Task<HttpResponseMessage> DeleteRole(Guid roleId = default(Guid))
        {
            var uri = new Uri(baseUri, $"Roles({roleId})");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Delete, uri);

            OnDeleteRole(httpRequestMessage);

            return await httpClient.SendAsync(httpRequestMessage);
        }

        partial void OnGetRoleByRoleId(HttpRequestMessage requestMessage);

        public async Task<TaskManager.Server.Models.TaskManagerDb.Role> GetRoleByRoleId(string expand = default(string), Guid roleId = default(Guid))
        {
            var uri = new Uri(baseUri, $"Roles({roleId})");

            uri = Radzen.ODataExtensions.GetODataUri(uri: uri, filter:null, top:null, skip:null, orderby:null, expand:expand, select:null, count:null);

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            OnGetRoleByRoleId(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<TaskManager.Server.Models.TaskManagerDb.Role>(response);
        }

        partial void OnUpdateRole(HttpRequestMessage requestMessage);
        
        public async Task<HttpResponseMessage> UpdateRole(Guid roleId = default(Guid), TaskManager.Server.Models.TaskManagerDb.Role role = default(TaskManager.Server.Models.TaskManagerDb.Role))
        {
            var uri = new Uri(baseUri, $"Roles({roleId})");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Patch, uri);


            httpRequestMessage.Content = new StringContent(Radzen.ODataJsonSerializer.Serialize(role), Encoding.UTF8, "application/json");

            OnUpdateRole(httpRequestMessage);

            return await httpClient.SendAsync(httpRequestMessage);
        }

        public async System.Threading.Tasks.Task ExportTaskHistoriesToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/taskmanagerdb/taskhistories/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/taskmanagerdb/taskhistories/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async System.Threading.Tasks.Task ExportTaskHistoriesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/taskmanagerdb/taskhistories/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/taskmanagerdb/taskhistories/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnGetTaskHistories(HttpRequestMessage requestMessage);

        public async Task<Radzen.ODataServiceResult<TaskManager.Server.Models.TaskManagerDb.TaskHistory>> GetTaskHistories(Query query)
        {
            return await GetTaskHistories(filter:$"{query.Filter}", orderby:$"{query.OrderBy}", top:query.Top, skip:query.Skip, count:query.Top != null && query.Skip != null);
        }

        public async Task<Radzen.ODataServiceResult<TaskManager.Server.Models.TaskManagerDb.TaskHistory>> GetTaskHistories(string filter = default(string), string orderby = default(string), string expand = default(string), int? top = default(int?), int? skip = default(int?), bool? count = default(bool?), string format = default(string), string select = default(string))
        {
            var uri = new Uri(baseUri, $"TaskHistories");
            uri = Radzen.ODataExtensions.GetODataUri(uri: uri, filter:filter, top:top, skip:skip, orderby:orderby, expand:expand, select:select, count:count);

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            OnGetTaskHistories(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<Radzen.ODataServiceResult<TaskManager.Server.Models.TaskManagerDb.TaskHistory>>(response);
        }

        partial void OnCreateTaskHistory(HttpRequestMessage requestMessage);

        public async Task<TaskManager.Server.Models.TaskManagerDb.TaskHistory> CreateTaskHistory(TaskManager.Server.Models.TaskManagerDb.TaskHistory taskHistory = default(TaskManager.Server.Models.TaskManagerDb.TaskHistory))
        {
            var uri = new Uri(baseUri, $"TaskHistories");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, uri);

            httpRequestMessage.Content = new StringContent(Radzen.ODataJsonSerializer.Serialize(taskHistory), Encoding.UTF8, "application/json");

            OnCreateTaskHistory(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<TaskManager.Server.Models.TaskManagerDb.TaskHistory>(response);
        }

        partial void OnDeleteTaskHistory(HttpRequestMessage requestMessage);

        public async Task<HttpResponseMessage> DeleteTaskHistory(Guid historyId = default(Guid))
        {
            var uri = new Uri(baseUri, $"TaskHistories({historyId})");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Delete, uri);

            OnDeleteTaskHistory(httpRequestMessage);

            return await httpClient.SendAsync(httpRequestMessage);
        }

        partial void OnGetTaskHistoryByHistoryId(HttpRequestMessage requestMessage);

        public async Task<TaskManager.Server.Models.TaskManagerDb.TaskHistory> GetTaskHistoryByHistoryId(string expand = default(string), Guid historyId = default(Guid))
        {
            var uri = new Uri(baseUri, $"TaskHistories({historyId})");

            uri = Radzen.ODataExtensions.GetODataUri(uri: uri, filter:null, top:null, skip:null, orderby:null, expand:expand, select:null, count:null);

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            OnGetTaskHistoryByHistoryId(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<TaskManager.Server.Models.TaskManagerDb.TaskHistory>(response);
        }

        partial void OnUpdateTaskHistory(HttpRequestMessage requestMessage);
        
        public async Task<HttpResponseMessage> UpdateTaskHistory(Guid historyId = default(Guid), TaskManager.Server.Models.TaskManagerDb.TaskHistory taskHistory = default(TaskManager.Server.Models.TaskManagerDb.TaskHistory))
        {
            var uri = new Uri(baseUri, $"TaskHistories({historyId})");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Patch, uri);


            httpRequestMessage.Content = new StringContent(Radzen.ODataJsonSerializer.Serialize(taskHistory), Encoding.UTF8, "application/json");

            OnUpdateTaskHistory(httpRequestMessage);

            return await httpClient.SendAsync(httpRequestMessage);
        }

        public async System.Threading.Tasks.Task ExportTasksToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/taskmanagerdb/tasks/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/taskmanagerdb/tasks/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async System.Threading.Tasks.Task ExportTasksToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/taskmanagerdb/tasks/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/taskmanagerdb/tasks/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnGetTasks(HttpRequestMessage requestMessage);

        public async Task<Radzen.ODataServiceResult<TaskManager.Server.Models.TaskManagerDb.Task>> GetTasks(Query query)
        {
            return await GetTasks(filter:$"{query.Filter}", orderby:$"{query.OrderBy}", top:query.Top, skip:query.Skip, count:query.Top != null && query.Skip != null);
        }

        public async Task<Radzen.ODataServiceResult<TaskManager.Server.Models.TaskManagerDb.Task>> GetTasks(string filter = default(string), string orderby = default(string), string expand = default(string), int? top = default(int?), int? skip = default(int?), bool? count = default(bool?), string format = default(string), string select = default(string))
        {
            var uri = new Uri(baseUri, $"Tasks");
            uri = Radzen.ODataExtensions.GetODataUri(uri: uri, filter:filter, top:top, skip:skip, orderby:orderby, expand:expand, select:select, count:count);

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            OnGetTasks(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<Radzen.ODataServiceResult<TaskManager.Server.Models.TaskManagerDb.Task>>(response);
        }

        partial void OnCreateTask(HttpRequestMessage requestMessage);

        public async Task<TaskManager.Server.Models.TaskManagerDb.Task> CreateTask(TaskManager.Server.Models.TaskManagerDb.Task task = default(TaskManager.Server.Models.TaskManagerDb.Task))
        {
            var uri = new Uri(baseUri, $"Tasks");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, uri);

            httpRequestMessage.Content = new StringContent(Radzen.ODataJsonSerializer.Serialize(task), Encoding.UTF8, "application/json");

            OnCreateTask(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<TaskManager.Server.Models.TaskManagerDb.Task>(response);
        }

        partial void OnDeleteTask(HttpRequestMessage requestMessage);

        public async Task<HttpResponseMessage> DeleteTask(Guid taskId = default(Guid))
        {
            var uri = new Uri(baseUri, $"Tasks({taskId})");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Delete, uri);

            OnDeleteTask(httpRequestMessage);

            return await httpClient.SendAsync(httpRequestMessage);
        }

        partial void OnGetTaskByTaskId(HttpRequestMessage requestMessage);

        public async Task<TaskManager.Server.Models.TaskManagerDb.Task> GetTaskByTaskId(string expand = default(string), Guid taskId = default(Guid))
        {
            var uri = new Uri(baseUri, $"Tasks({taskId})");

            uri = Radzen.ODataExtensions.GetODataUri(uri: uri, filter:null, top:null, skip:null, orderby:null, expand:expand, select:null, count:null);

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            OnGetTaskByTaskId(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<TaskManager.Server.Models.TaskManagerDb.Task>(response);
        }

        partial void OnUpdateTask(HttpRequestMessage requestMessage);
        
        public async Task<HttpResponseMessage> UpdateTask(Guid taskId = default(Guid), TaskManager.Server.Models.TaskManagerDb.Task task = default(TaskManager.Server.Models.TaskManagerDb.Task))
        {
            var uri = new Uri(baseUri, $"Tasks({taskId})");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Patch, uri);


            httpRequestMessage.Content = new StringContent(Radzen.ODataJsonSerializer.Serialize(task), Encoding.UTF8, "application/json");

            OnUpdateTask(httpRequestMessage);

            return await httpClient.SendAsync(httpRequestMessage);
        }

        public async System.Threading.Tasks.Task ExportTaskSubPointsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/taskmanagerdb/tasksubpoints/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/taskmanagerdb/tasksubpoints/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async System.Threading.Tasks.Task ExportTaskSubPointsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/taskmanagerdb/tasksubpoints/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/taskmanagerdb/tasksubpoints/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnGetTaskSubPoints(HttpRequestMessage requestMessage);

        public async Task<Radzen.ODataServiceResult<TaskManager.Server.Models.TaskManagerDb.TaskSubPoint>> GetTaskSubPoints(Query query)
        {
            return await GetTaskSubPoints(filter:$"{query.Filter}", orderby:$"{query.OrderBy}", top:query.Top, skip:query.Skip, count:query.Top != null && query.Skip != null);
        }

        public async Task<Radzen.ODataServiceResult<TaskManager.Server.Models.TaskManagerDb.TaskSubPoint>> GetTaskSubPoints(string filter = default(string), string orderby = default(string), string expand = default(string), int? top = default(int?), int? skip = default(int?), bool? count = default(bool?), string format = default(string), string select = default(string))
        {
            var uri = new Uri(baseUri, $"TaskSubPoints");
            uri = Radzen.ODataExtensions.GetODataUri(uri: uri, filter:filter, top:top, skip:skip, orderby:orderby, expand:expand, select:select, count:count);

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            OnGetTaskSubPoints(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<Radzen.ODataServiceResult<TaskManager.Server.Models.TaskManagerDb.TaskSubPoint>>(response);
        }

        partial void OnCreateTaskSubPoint(HttpRequestMessage requestMessage);

        public async Task<TaskManager.Server.Models.TaskManagerDb.TaskSubPoint> CreateTaskSubPoint(TaskManager.Server.Models.TaskManagerDb.TaskSubPoint taskSubPoint = default(TaskManager.Server.Models.TaskManagerDb.TaskSubPoint))
        {
            var uri = new Uri(baseUri, $"TaskSubPoints");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, uri);

            httpRequestMessage.Content = new StringContent(Radzen.ODataJsonSerializer.Serialize(taskSubPoint), Encoding.UTF8, "application/json");

            OnCreateTaskSubPoint(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<TaskManager.Server.Models.TaskManagerDb.TaskSubPoint>(response);
        }

        partial void OnDeleteTaskSubPoint(HttpRequestMessage requestMessage);

        public async Task<HttpResponseMessage> DeleteTaskSubPoint(Guid subPointId = default(Guid))
        {
            var uri = new Uri(baseUri, $"TaskSubPoints({subPointId})");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Delete, uri);

            OnDeleteTaskSubPoint(httpRequestMessage);

            return await httpClient.SendAsync(httpRequestMessage);
        }

        partial void OnGetTaskSubPointBySubPointId(HttpRequestMessage requestMessage);

        public async Task<TaskManager.Server.Models.TaskManagerDb.TaskSubPoint> GetTaskSubPointBySubPointId(string expand = default(string), Guid subPointId = default(Guid))
        {
            var uri = new Uri(baseUri, $"TaskSubPoints({subPointId})");

            uri = Radzen.ODataExtensions.GetODataUri(uri: uri, filter:null, top:null, skip:null, orderby:null, expand:expand, select:null, count:null);

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            OnGetTaskSubPointBySubPointId(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<TaskManager.Server.Models.TaskManagerDb.TaskSubPoint>(response);
        }

        partial void OnUpdateTaskSubPoint(HttpRequestMessage requestMessage);
        
        public async Task<HttpResponseMessage> UpdateTaskSubPoint(Guid subPointId = default(Guid), TaskManager.Server.Models.TaskManagerDb.TaskSubPoint taskSubPoint = default(TaskManager.Server.Models.TaskManagerDb.TaskSubPoint))
        {
            var uri = new Uri(baseUri, $"TaskSubPoints({subPointId})");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Patch, uri);


            httpRequestMessage.Content = new StringContent(Radzen.ODataJsonSerializer.Serialize(taskSubPoint), Encoding.UTF8, "application/json");

            OnUpdateTaskSubPoint(httpRequestMessage);

            return await httpClient.SendAsync(httpRequestMessage);
        }

        public async System.Threading.Tasks.Task ExportTimeLogsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/taskmanagerdb/timelogs/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/taskmanagerdb/timelogs/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async System.Threading.Tasks.Task ExportTimeLogsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/taskmanagerdb/timelogs/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/taskmanagerdb/timelogs/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnGetTimeLogs(HttpRequestMessage requestMessage);

        public async Task<Radzen.ODataServiceResult<TaskManager.Server.Models.TaskManagerDb.TimeLog>> GetTimeLogs(Query query)
        {
            return await GetTimeLogs(filter:$"{query.Filter}", orderby:$"{query.OrderBy}", top:query.Top, skip:query.Skip, count:query.Top != null && query.Skip != null);
        }

        public async Task<Radzen.ODataServiceResult<TaskManager.Server.Models.TaskManagerDb.TimeLog>> GetTimeLogs(string filter = default(string), string orderby = default(string), string expand = default(string), int? top = default(int?), int? skip = default(int?), bool? count = default(bool?), string format = default(string), string select = default(string))
        {
            var uri = new Uri(baseUri, $"TimeLogs");
            uri = Radzen.ODataExtensions.GetODataUri(uri: uri, filter:filter, top:top, skip:skip, orderby:orderby, expand:expand, select:select, count:count);

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            OnGetTimeLogs(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<Radzen.ODataServiceResult<TaskManager.Server.Models.TaskManagerDb.TimeLog>>(response);
        }

        partial void OnCreateTimeLog(HttpRequestMessage requestMessage);

        public async Task<TaskManager.Server.Models.TaskManagerDb.TimeLog> CreateTimeLog(TaskManager.Server.Models.TaskManagerDb.TimeLog timeLog = default(TaskManager.Server.Models.TaskManagerDb.TimeLog))
        {
            var uri = new Uri(baseUri, $"TimeLogs");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, uri);

            httpRequestMessage.Content = new StringContent(Radzen.ODataJsonSerializer.Serialize(timeLog), Encoding.UTF8, "application/json");

            OnCreateTimeLog(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<TaskManager.Server.Models.TaskManagerDb.TimeLog>(response);
        }

        partial void OnDeleteTimeLog(HttpRequestMessage requestMessage);

        public async Task<HttpResponseMessage> DeleteTimeLog(Guid timeLogId = default(Guid))
        {
            var uri = new Uri(baseUri, $"TimeLogs({timeLogId})");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Delete, uri);

            OnDeleteTimeLog(httpRequestMessage);

            return await httpClient.SendAsync(httpRequestMessage);
        }

        partial void OnGetTimeLogByTimeLogId(HttpRequestMessage requestMessage);

        public async Task<TaskManager.Server.Models.TaskManagerDb.TimeLog> GetTimeLogByTimeLogId(string expand = default(string), Guid timeLogId = default(Guid))
        {
            var uri = new Uri(baseUri, $"TimeLogs({timeLogId})");

            uri = Radzen.ODataExtensions.GetODataUri(uri: uri, filter:null, top:null, skip:null, orderby:null, expand:expand, select:null, count:null);

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            OnGetTimeLogByTimeLogId(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<TaskManager.Server.Models.TaskManagerDb.TimeLog>(response);
        }

        partial void OnUpdateTimeLog(HttpRequestMessage requestMessage);
        
        public async Task<HttpResponseMessage> UpdateTimeLog(Guid timeLogId = default(Guid), TaskManager.Server.Models.TaskManagerDb.TimeLog timeLog = default(TaskManager.Server.Models.TaskManagerDb.TimeLog))
        {
            var uri = new Uri(baseUri, $"TimeLogs({timeLogId})");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Patch, uri);


            httpRequestMessage.Content = new StringContent(Radzen.ODataJsonSerializer.Serialize(timeLog), Encoding.UTF8, "application/json");

            OnUpdateTimeLog(httpRequestMessage);

            return await httpClient.SendAsync(httpRequestMessage);
        }

        public async System.Threading.Tasks.Task ExportUsersToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/taskmanagerdb/users/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/taskmanagerdb/users/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async System.Threading.Tasks.Task ExportUsersToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/taskmanagerdb/users/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/taskmanagerdb/users/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnGetUsers(HttpRequestMessage requestMessage);

        public async Task<Radzen.ODataServiceResult<TaskManager.Server.Models.TaskManagerDb.User>> GetUsers(Query query)
        {
            return await GetUsers(filter:$"{query.Filter}", orderby:$"{query.OrderBy}", top:query.Top, skip:query.Skip, count:query.Top != null && query.Skip != null);
        }

        public async Task<Radzen.ODataServiceResult<TaskManager.Server.Models.TaskManagerDb.User>> GetUsers(string filter = default(string), string orderby = default(string), string expand = default(string), int? top = default(int?), int? skip = default(int?), bool? count = default(bool?), string format = default(string), string select = default(string))
        {
            var uri = new Uri(baseUri, $"Users");
            uri = Radzen.ODataExtensions.GetODataUri(uri: uri, filter:filter, top:top, skip:skip, orderby:orderby, expand:expand, select:select, count:count);

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            OnGetUsers(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<Radzen.ODataServiceResult<TaskManager.Server.Models.TaskManagerDb.User>>(response);
        }

        partial void OnCreateUser(HttpRequestMessage requestMessage);

        public async Task<TaskManager.Server.Models.TaskManagerDb.User> CreateUser(TaskManager.Server.Models.TaskManagerDb.User user = default(TaskManager.Server.Models.TaskManagerDb.User))
        {
            var uri = new Uri(baseUri, $"Users");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, uri);

            httpRequestMessage.Content = new StringContent(Radzen.ODataJsonSerializer.Serialize(user), Encoding.UTF8, "application/json");

            OnCreateUser(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<TaskManager.Server.Models.TaskManagerDb.User>(response);
        }

        partial void OnDeleteUser(HttpRequestMessage requestMessage);

        public async Task<HttpResponseMessage> DeleteUser(Guid userId = default(Guid))
        {
            var uri = new Uri(baseUri, $"Users({userId})");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Delete, uri);

            OnDeleteUser(httpRequestMessage);

            return await httpClient.SendAsync(httpRequestMessage);
        }

        partial void OnGetUserByUserId(HttpRequestMessage requestMessage);

        public async Task<TaskManager.Server.Models.TaskManagerDb.User> GetUserByUserId(string expand = default(string), Guid userId = default(Guid))
        {
            var uri = new Uri(baseUri, $"Users({userId})");

            uri = Radzen.ODataExtensions.GetODataUri(uri: uri, filter:null, top:null, skip:null, orderby:null, expand:expand, select:null, count:null);

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            OnGetUserByUserId(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<TaskManager.Server.Models.TaskManagerDb.User>(response);
        }

        partial void OnUpdateUser(HttpRequestMessage requestMessage);
        
        public async Task<HttpResponseMessage> UpdateUser(Guid userId = default(Guid), TaskManager.Server.Models.TaskManagerDb.User user = default(TaskManager.Server.Models.TaskManagerDb.User))
        {
            var uri = new Uri(baseUri, $"Users({userId})");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Patch, uri);


            httpRequestMessage.Content = new StringContent(Radzen.ODataJsonSerializer.Serialize(user), Encoding.UTF8, "application/json");

            OnUpdateUser(httpRequestMessage);

            return await httpClient.SendAsync(httpRequestMessage);
        }
    }
}