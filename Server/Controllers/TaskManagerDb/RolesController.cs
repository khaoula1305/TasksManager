using System;
using System.Net;
using System.Data;
using System.Linq;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Formatter;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace TaskManager.Server.Controllers.TaskManagerDb
{
    [Route("odata/TaskManagerDb/Roles")]
    public partial class RolesController : ODataController
    {
        private TaskManager.Server.Data.TaskManagerDbContext context;

        public RolesController(TaskManager.Server.Data.TaskManagerDbContext context)
        {
            this.context = context;
        }

    
        [HttpGet]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IEnumerable<TaskManager.Server.Models.TaskManagerDb.Role> GetRoles()
        {
            var items = this.context.Roles.AsQueryable<TaskManager.Server.Models.TaskManagerDb.Role>();
            this.OnRolesRead(ref items);

            return items;
        }

        partial void OnRolesRead(ref IQueryable<TaskManager.Server.Models.TaskManagerDb.Role> items);

        partial void OnRoleGet(ref SingleResult<TaskManager.Server.Models.TaskManagerDb.Role> item);

        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        [HttpGet("/odata/TaskManagerDb/Roles(RoleID={RoleID})")]
        public SingleResult<TaskManager.Server.Models.TaskManagerDb.Role> GetRole(Guid key)
        {
            var items = this.context.Roles.Where(i => i.RoleID == key);
            var result = SingleResult.Create(items);

            OnRoleGet(ref result);

            return result;
        }
        partial void OnRoleDeleted(TaskManager.Server.Models.TaskManagerDb.Role item);
        partial void OnAfterRoleDeleted(TaskManager.Server.Models.TaskManagerDb.Role item);

        [HttpDelete("/odata/TaskManagerDb/Roles(RoleID={RoleID})")]
        public IActionResult DeleteRole(Guid key)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }


                var item = this.context.Roles
                    .Where(i => i.RoleID == key)
                    .FirstOrDefault();

                if (item == null)
                {
                    return BadRequest();
                }
                this.OnRoleDeleted(item);
                this.context.Roles.Remove(item);
                this.context.SaveChanges();
                this.OnAfterRoleDeleted(item);

                return new NoContentResult();

            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        partial void OnRoleUpdated(TaskManager.Server.Models.TaskManagerDb.Role item);
        partial void OnAfterRoleUpdated(TaskManager.Server.Models.TaskManagerDb.Role item);

        [HttpPut("/odata/TaskManagerDb/Roles(RoleID={RoleID})")]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult PutRole(Guid key, [FromBody]TaskManager.Server.Models.TaskManagerDb.Role item)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (item == null || (item.RoleID != key))
                {
                    return BadRequest();
                }
                this.OnRoleUpdated(item);
                this.context.Roles.Update(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.Roles.Where(i => i.RoleID == key);
                
                this.OnAfterRoleUpdated(item);
                return new ObjectResult(SingleResult.Create(itemToReturn));
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        [HttpPatch("/odata/TaskManagerDb/Roles(RoleID={RoleID})")]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult PatchRole(Guid key, [FromBody]Delta<TaskManager.Server.Models.TaskManagerDb.Role> patch)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var item = this.context.Roles.Where(i => i.RoleID == key).FirstOrDefault();

                if (item == null)
                {
                    return BadRequest();
                }
                patch.Patch(item);

                this.OnRoleUpdated(item);
                this.context.Roles.Update(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.Roles.Where(i => i.RoleID == key);
                
                return new ObjectResult(SingleResult.Create(itemToReturn));
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        partial void OnRoleCreated(TaskManager.Server.Models.TaskManagerDb.Role item);
        partial void OnAfterRoleCreated(TaskManager.Server.Models.TaskManagerDb.Role item);

        [HttpPost]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult Post([FromBody] TaskManager.Server.Models.TaskManagerDb.Role item)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (item == null)
                {
                    return BadRequest();
                }

                this.OnRoleCreated(item);
                this.context.Roles.Add(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.Roles.Where(i => i.RoleID == item.RoleID);

                

                this.OnAfterRoleCreated(item);

                return new ObjectResult(SingleResult.Create(itemToReturn))
                {
                    StatusCode = 201
                };
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }
    }
}
