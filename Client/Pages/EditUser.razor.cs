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
    public partial class EditUser
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
        public Guid UserID { get; set; }

        protected override async Task OnInitializedAsync()
        {
            user = await TaskManagerDbService.GetUserByUserId(userId:UserID);
        }
        protected bool errorVisible;
        protected TaskManager.Server.Models.TaskManagerDb.User user;

        protected IEnumerable<TaskManager.Server.Models.TaskManagerDb.Role> rolesForRoleID;


        protected int rolesForRoleIDCount;
        protected TaskManager.Server.Models.TaskManagerDb.Role rolesForRoleIDValue;

        [Inject]
        protected SecurityService Security { get; set; }
        protected async Task rolesForRoleIDLoadData(LoadDataArgs args)
        {
            try
            {
                var result = await TaskManagerDbService.GetRoles(top: args.Top, skip: args.Skip, count:args.Top != null && args.Skip != null, filter: $"contains(RoleName, '{(!string.IsNullOrEmpty(args.Filter) ? args.Filter : "")}')", orderby: $"{args.OrderBy}");
                rolesForRoleID = result.Value.AsODataEnumerable();
                rolesForRoleIDCount = result.Count;

                if (!object.Equals(user.RoleID, null))
                {
                    var valueResult = await TaskManagerDbService.GetRoles(filter: $"RoleID eq {user.RoleID}");
                    var firstItem = valueResult.Value.FirstOrDefault();
                    if (firstItem != null)
                    {
                        rolesForRoleIDValue = firstItem;
                    }
                }

            }
            catch (System.Exception ex)
            {
                NotificationService.Notify(new NotificationMessage(){ Severity = NotificationSeverity.Error, Summary = $"Error", Detail = $"Unable to load Role" });
            }
        }
        protected async Task FormSubmit()
        {
            try
            {
                await TaskManagerDbService.UpdateUser(userId:UserID, user);
                DialogService.Close(user);
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