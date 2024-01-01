using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;

namespace TaskManager.Client.Pages
{
    public partial class EditTimeLog
    {
        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected TooltipService TooltipService { get; set; }

        [Inject]
        protected ContextMenuService ContextMenuService { get; set; }

        [Inject]
        protected NotificationService NotificationService { get; set; }
        [Inject]
        public TaskManagerDbService TaskManagerDbService { get; set; }

        [Parameter]
        public Guid TimeLogID { get; set; }

        protected override async Task OnInitializedAsync()
        {
            timeLog = await TaskManagerDbService.GetTimeLogByTimeLogId(timeLogId:TimeLogID);
        }
        protected bool errorVisible;
        protected TaskManager.Server.Models.TaskManagerDb.TimeLog timeLog;

        protected IEnumerable<TaskManager.Server.Models.TaskManagerDb.Task> tasksForTaskID;

        protected IEnumerable<TaskManager.Server.Models.TaskManagerDb.User> usersForUserID;


        protected int tasksForTaskIDCount;
        protected TaskManager.Server.Models.TaskManagerDb.Task tasksForTaskIDValue;
        protected async Task tasksForTaskIDLoadData(LoadDataArgs args)
        {
            try
            {
                var result = await TaskManagerDbService.GetTasks(top: args.Top, skip: args.Skip, count:args.Top != null && args.Skip != null, filter: $"contains(Title, '{(!string.IsNullOrEmpty(args.Filter) ? args.Filter : "")}')", orderby: $"{args.OrderBy}");
                tasksForTaskID = result.Value.AsODataEnumerable();
                tasksForTaskIDCount = result.Count;

                if (!object.Equals(timeLog.TaskID, null))
                {
                    var valueResult = await TaskManagerDbService.GetTasks(filter: $"TaskID eq {timeLog.TaskID}");
                    var firstItem = valueResult.Value.FirstOrDefault();
                    if (firstItem != null)
                    {
                        tasksForTaskIDValue = firstItem;
                    }
                }

            }
            catch (System.Exception ex)
            {
                NotificationService.Notify(new NotificationMessage(){ Severity = NotificationSeverity.Error, Summary = $"Error", Detail = $"Unable to load Task" });
            }
        }

        protected int usersForUserIDCount;
        protected TaskManager.Server.Models.TaskManagerDb.User usersForUserIDValue;

        [Inject]
        protected SecurityService Security { get; set; }
        protected async Task usersForUserIDLoadData(LoadDataArgs args)
        {
            try
            {
                var result = await TaskManagerDbService.GetUsers(top: args.Top, skip: args.Skip, count:args.Top != null && args.Skip != null, filter: $"contains(Username, '{(!string.IsNullOrEmpty(args.Filter) ? args.Filter : "")}')", orderby: $"{args.OrderBy}");
                usersForUserID = result.Value.AsODataEnumerable();
                usersForUserIDCount = result.Count;

                if (!object.Equals(timeLog.UserID, null))
                {
                    var valueResult = await TaskManagerDbService.GetUsers(filter: $"UserID eq {timeLog.UserID}");
                    var firstItem = valueResult.Value.FirstOrDefault();
                    if (firstItem != null)
                    {
                        usersForUserIDValue = firstItem;
                    }
                }

            }
            catch (System.Exception ex)
            {
                NotificationService.Notify(new NotificationMessage(){ Severity = NotificationSeverity.Error, Summary = $"Error", Detail = $"Unable to load User" });
            }
        }
        protected async Task FormSubmit()
        {
            try
            {
                await TaskManagerDbService.UpdateTimeLog(timeLogId:TimeLogID, timeLog);
                DialogService.Close(timeLog);
            }
            catch (Exception ex)
            {
                errorVisible = true;
            }
        }

        protected async Task CancelButtonClick(MouseEventArgs args)
        {
            DialogService.Close(null);
        }
    }
}