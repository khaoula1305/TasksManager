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
    public partial class AddTaskHistory
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

        protected override async Task OnInitializedAsync()
        {
            taskHistory = new TaskManager.Server.Models.TaskManagerDb.TaskHistory();
        }
        protected bool errorVisible;
        protected TaskManager.Server.Models.TaskManagerDb.TaskHistory taskHistory;

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

                if (!object.Equals(taskHistory.TaskID, null))
                {
                    var valueResult = await TaskManagerDbService.GetTasks(filter: $"TaskID eq {taskHistory.TaskID}");
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

                if (!object.Equals(taskHistory.UserID, null))
                {
                    var valueResult = await TaskManagerDbService.GetUsers(filter: $"UserID eq {taskHistory.UserID}");
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
                await TaskManagerDbService.CreateTaskHistory(taskHistory);
                DialogService.Close(taskHistory);
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