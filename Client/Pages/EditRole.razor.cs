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
    public partial class EditRole
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
        public Guid RoleID { get; set; }

        protected override async Task OnInitializedAsync()
        {
            role = await TaskManagerDbService.GetRoleByRoleId(roleId:RoleID);
        }
        protected bool errorVisible;
        protected TaskManager.Server.Models.TaskManagerDb.Role role;

        [Inject]
        protected SecurityService Security { get; set; }

        protected async Task FormSubmit()
        {
            try
            {
                await TaskManagerDbService.UpdateRole(roleId:RoleID, role);
                DialogService.Close(role);
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