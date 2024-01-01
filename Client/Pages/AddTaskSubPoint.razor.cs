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
    public partial class AddTaskSubPoint
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
            taskSubPoint = new TaskManager.Server.Models.TaskManagerDb.TaskSubPoint();
        }
        protected bool errorVisible;
        protected TaskManager.Server.Models.TaskManagerDb.TaskSubPoint taskSubPoint;

        protected IEnumerable<TaskManager.Server.Models.TaskManagerDb.Task> tasksForTaskID;


        protected int tasksForTaskIDCount;
        protected TaskManager.Server.Models.TaskManagerDb.Task tasksForTaskIDValue;

        [Inject]
        protected SecurityService Security { get; set; }
        protected async Task tasksForTaskIDLoadData(LoadDataArgs args)
        {
            try
            {
                var result = await TaskManagerDbService.GetTasks(top: args.Top, skip: args.Skip, count:args.Top != null && args.Skip != null, filter: $"contains(Title, '{(!string.IsNullOrEmpty(args.Filter) ? args.Filter : "")}')", orderby: $"{args.OrderBy}");
                tasksForTaskID = result.Value.AsODataEnumerable();
                tasksForTaskIDCount = result.Count;

                if (!object.Equals(taskSubPoint.TaskID, null))
                {
                    var valueResult = await TaskManagerDbService.GetTasks(filter: $"TaskID eq {taskSubPoint.TaskID}");
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
        protected async Task FormSubmit()
        {
            try
            {
                await TaskManagerDbService.CreateTaskSubPoint(taskSubPoint);
                DialogService.Close(taskSubPoint);
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