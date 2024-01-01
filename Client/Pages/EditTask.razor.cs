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
    public partial class EditTask
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
        public Guid TaskID { get; set; }

        protected override async Task OnInitializedAsync()
        {
            task = await TaskManagerDbService.GetTaskByTaskId(taskId:TaskID);
        }
        protected bool errorVisible;
        protected TaskManager.Server.Models.TaskManagerDb.Task task;

        protected IEnumerable<TaskManager.Server.Models.TaskManagerDb.User> usersForAssignedToUserID;

        protected IEnumerable<TaskManager.Server.Models.TaskManagerDb.User> usersForCreatorUserID;


        protected int usersForAssignedToUserIDCount;
        protected TaskManager.Server.Models.TaskManagerDb.User usersForAssignedToUserIDValue;
        protected async Task usersForAssignedToUserIDLoadData(LoadDataArgs args)
        {
            try
            {
                var result = await TaskManagerDbService.GetUsers(top: args.Top, skip: args.Skip, count:args.Top != null && args.Skip != null, filter: $"contains(Username, '{(!string.IsNullOrEmpty(args.Filter) ? args.Filter : "")}')", orderby: $"{args.OrderBy}");
                usersForAssignedToUserID = result.Value.AsODataEnumerable();
                usersForAssignedToUserIDCount = result.Count;

                if (!object.Equals(task.AssignedToUserID, null))
                {
                    var valueResult = await TaskManagerDbService.GetUsers(filter: $"UserID eq {task.AssignedToUserID}");
                    var firstItem = valueResult.Value.FirstOrDefault();
                    if (firstItem != null)
                    {
                        usersForAssignedToUserIDValue = firstItem;
                    }
                }

            }
            catch (System.Exception ex)
            {
                NotificationService.Notify(new NotificationMessage(){ Severity = NotificationSeverity.Error, Summary = $"Error", Detail = $"Unable to load User" });
            }
        }

        protected int usersForCreatorUserIDCount;
        protected TaskManager.Server.Models.TaskManagerDb.User usersForCreatorUserIDValue;

        [Inject]
        protected SecurityService Security { get; set; }
        protected async Task usersForCreatorUserIDLoadData(LoadDataArgs args)
        {
            try
            {
                var result = await TaskManagerDbService.GetUsers(top: args.Top, skip: args.Skip, count:args.Top != null && args.Skip != null, filter: $"contains(Username, '{(!string.IsNullOrEmpty(args.Filter) ? args.Filter : "")}')", orderby: $"{args.OrderBy}");
                usersForCreatorUserID = result.Value.AsODataEnumerable();
                usersForCreatorUserIDCount = result.Count;

                if (!object.Equals(task.CreatorUserID, null))
                {
                    var valueResult = await TaskManagerDbService.GetUsers(filter: $"UserID eq {task.CreatorUserID}");
                    var firstItem = valueResult.Value.FirstOrDefault();
                    if (firstItem != null)
                    {
                        usersForCreatorUserIDValue = firstItem;
                    }
                }

            }
            catch (System.Exception ex)
            {
                NotificationService.Notify(new NotificationMessage(){ Severity = NotificationSeverity.Error, Summary = $"Error", Detail = $"Unable to load User1" });
            }
        }
        protected async Task FormSubmit()
        {
            try
            {
                await TaskManagerDbService.UpdateTask(taskId:TaskID, task);
                DialogService.Close(task);
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