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
    [Route("odata/TaskManagerDb/Users")]
    public partial class UsersController : ODataController
    {
        private TaskManager.Server.Data.TaskManagerDbContext context;

        public UsersController(TaskManager.Server.Data.TaskManagerDbContext context)
        {
            this.context = context;
        }

    
        [HttpGet]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IEnumerable<TaskManager.Server.Models.TaskManagerDb.User> GetUsers()
        {
            var items = this.context.Users.AsQueryable<TaskManager.Server.Models.TaskManagerDb.User>();
            this.OnUsersRead(ref items);

            return items;
        }

        partial void OnUsersRead(ref IQueryable<TaskManager.Server.Models.TaskManagerDb.User> items);

        partial void OnUserGet(ref SingleResult<TaskManager.Server.Models.TaskManagerDb.User> item);

        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        [HttpGet("/odata/TaskManagerDb/Users(UserID={UserID})")]
        public SingleResult<TaskManager.Server.Models.TaskManagerDb.User> GetUser(Guid key)
        {
            var items = this.context.Users.Where(i => i.UserID == key);
            var result = SingleResult.Create(items);

            OnUserGet(ref result);

            return result;
        }
        partial void OnUserDeleted(TaskManager.Server.Models.TaskManagerDb.User item);
        partial void OnAfterUserDeleted(TaskManager.Server.Models.TaskManagerDb.User item);

        [HttpDelete("/odata/TaskManagerDb/Users(UserID={UserID})")]
        public IActionResult DeleteUser(Guid key)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }


                var item = this.context.Users
                    .Where(i => i.UserID == key)
                    .FirstOrDefault();

                if (item == null)
                {
                    return BadRequest();
                }
                this.OnUserDeleted(item);
                this.context.Users.Remove(item);
                this.context.SaveChanges();
                this.OnAfterUserDeleted(item);

                return new NoContentResult();

            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        partial void OnUserUpdated(TaskManager.Server.Models.TaskManagerDb.User item);
        partial void OnAfterUserUpdated(TaskManager.Server.Models.TaskManagerDb.User item);

        [HttpPut("/odata/TaskManagerDb/Users(UserID={UserID})")]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult PutUser(Guid key, [FromBody]TaskManager.Server.Models.TaskManagerDb.User item)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (item == null || (item.UserID != key))
                {
                    return BadRequest();
                }
                this.OnUserUpdated(item);
                this.context.Users.Update(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.Users.Where(i => i.UserID == key);
                Request.QueryString = Request.QueryString.Add("$expand", "Role");
                this.OnAfterUserUpdated(item);
                return new ObjectResult(SingleResult.Create(itemToReturn));
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        [HttpPatch("/odata/TaskManagerDb/Users(UserID={UserID})")]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult PatchUser(Guid key, [FromBody]Delta<TaskManager.Server.Models.TaskManagerDb.User> patch)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var item = this.context.Users.Where(i => i.UserID == key).FirstOrDefault();

                if (item == null)
                {
                    return BadRequest();
                }
                patch.Patch(item);

                this.OnUserUpdated(item);
                this.context.Users.Update(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.Users.Where(i => i.UserID == key);
                Request.QueryString = Request.QueryString.Add("$expand", "Role");
                return new ObjectResult(SingleResult.Create(itemToReturn));
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        partial void OnUserCreated(TaskManager.Server.Models.TaskManagerDb.User item);
        partial void OnAfterUserCreated(TaskManager.Server.Models.TaskManagerDb.User item);

        [HttpPost]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult Post([FromBody] TaskManager.Server.Models.TaskManagerDb.User item)
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

                this.OnUserCreated(item);
                this.context.Users.Add(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.Users.Where(i => i.UserID == item.UserID);

                Request.QueryString = Request.QueryString.Add("$expand", "Role");

                this.OnAfterUserCreated(item);

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
