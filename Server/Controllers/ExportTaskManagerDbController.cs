using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

using TaskManager.Server.Data;

namespace TaskManager.Server.Controllers
{
    public partial class ExportTaskManagerDbController : ExportController
    {
        private readonly TaskManagerDbContext context;
        private readonly TaskManagerDbService service;

        public ExportTaskManagerDbController(TaskManagerDbContext context, TaskManagerDbService service)
        {
            this.service = service;
            this.context = context;
        }

        [HttpGet("/export/TaskManagerDb/notifications/csv")]
        [HttpGet("/export/TaskManagerDb/notifications/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportNotificationsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetNotifications(), Request.Query, false), fileName);
        }

        [HttpGet("/export/TaskManagerDb/notifications/excel")]
        [HttpGet("/export/TaskManagerDb/notifications/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportNotificationsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetNotifications(), Request.Query, false), fileName);
        }

        [HttpGet("/export/TaskManagerDb/roles/csv")]
        [HttpGet("/export/TaskManagerDb/roles/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportRolesToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetRoles(), Request.Query, false), fileName);
        }

        [HttpGet("/export/TaskManagerDb/roles/excel")]
        [HttpGet("/export/TaskManagerDb/roles/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportRolesToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetRoles(), Request.Query, false), fileName);
        }

        [HttpGet("/export/TaskManagerDb/taskhistories/csv")]
        [HttpGet("/export/TaskManagerDb/taskhistories/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportTaskHistoriesToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetTaskHistories(), Request.Query, false), fileName);
        }

        [HttpGet("/export/TaskManagerDb/taskhistories/excel")]
        [HttpGet("/export/TaskManagerDb/taskhistories/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportTaskHistoriesToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetTaskHistories(), Request.Query, false), fileName);
        }

        [HttpGet("/export/TaskManagerDb/tasks/csv")]
        [HttpGet("/export/TaskManagerDb/tasks/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportTasksToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetTasks(), Request.Query, false), fileName);
        }

        [HttpGet("/export/TaskManagerDb/tasks/excel")]
        [HttpGet("/export/TaskManagerDb/tasks/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportTasksToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetTasks(), Request.Query, false), fileName);
        }

        [HttpGet("/export/TaskManagerDb/tasksubpoints/csv")]
        [HttpGet("/export/TaskManagerDb/tasksubpoints/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportTaskSubPointsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetTaskSubPoints(), Request.Query, false), fileName);
        }

        [HttpGet("/export/TaskManagerDb/tasksubpoints/excel")]
        [HttpGet("/export/TaskManagerDb/tasksubpoints/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportTaskSubPointsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetTaskSubPoints(), Request.Query, false), fileName);
        }

        [HttpGet("/export/TaskManagerDb/timelogs/csv")]
        [HttpGet("/export/TaskManagerDb/timelogs/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportTimeLogsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetTimeLogs(), Request.Query, false), fileName);
        }

        [HttpGet("/export/TaskManagerDb/timelogs/excel")]
        [HttpGet("/export/TaskManagerDb/timelogs/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportTimeLogsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetTimeLogs(), Request.Query, false), fileName);
        }

        [HttpGet("/export/TaskManagerDb/users/csv")]
        [HttpGet("/export/TaskManagerDb/users/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportUsersToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetUsers(), Request.Query, false), fileName);
        }

        [HttpGet("/export/TaskManagerDb/users/excel")]
        [HttpGet("/export/TaskManagerDb/users/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportUsersToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetUsers(), Request.Query, false), fileName);
        }
    }
}
