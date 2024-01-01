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
    [Route("odata/TaskManagerDb/Tasks")]
    public partial class TasksController : ODataController
    {
        private TaskManager.Server.Data.TaskManagerDbContext context;

        public TasksController(TaskManager.Server.Data.TaskManagerDbContext context)
        {
            this.context = context;
        }

    
        [HttpGet]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IEnumerable<TaskManager.Server.Models.TaskManagerDb.Task> GetTasks()
        {
            var items = this.context.Tasks.AsQueryable<TaskManager.Server.Models.TaskManagerDb.Task>();
            this.OnTasksRead(ref items);

            return items;
        }

        partial void OnTasksRead(ref IQueryable<TaskManager.Server.Models.TaskManagerDb.Task> items);

        partial void OnTaskGet(ref SingleResult<TaskManager.Server.Models.TaskManagerDb.Task> item);

        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        [HttpGet("/odata/TaskManagerDb/Tasks(TaskID={TaskID})")]
        public SingleResult<TaskManager.Server.Models.TaskManagerDb.Task> GetTask(Guid key)
        {
            var items = this.context.Tasks.Where(i => i.TaskID == key);
            var result = SingleResult.Create(items);

            OnTaskGet(ref result);

            return result;
        }
        partial void OnTaskDeleted(TaskManager.Server.Models.TaskManagerDb.Task item);
        partial void OnAfterTaskDeleted(TaskManager.Server.Models.TaskManagerDb.Task item);

        [HttpDelete("/odata/TaskManagerDb/Tasks(TaskID={TaskID})")]
        public IActionResult DeleteTask(Guid key)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }


                var item = this.context.Tasks
                    .Where(i => i.TaskID == key)
                    .FirstOrDefault();

                if (item == null)
                {
                    return BadRequest();
                }
                this.OnTaskDeleted(item);
                this.context.Tasks.Remove(item);
                this.context.SaveChanges();
                this.OnAfterTaskDeleted(item);

                return new NoContentResult();

            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        partial void OnTaskUpdated(TaskManager.Server.Models.TaskManagerDb.Task item);
        partial void OnAfterTaskUpdated(TaskManager.Server.Models.TaskManagerDb.Task item);

        [HttpPut("/odata/TaskManagerDb/Tasks(TaskID={TaskID})")]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult PutTask(Guid key, [FromBody]TaskManager.Server.Models.TaskManagerDb.Task item)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (item == null || (item.TaskID != key))
                {
                    return BadRequest();
                }
                this.OnTaskUpdated(item);
                this.context.Tasks.Update(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.Tasks.Where(i => i.TaskID == key);
                Request.QueryString = Request.QueryString.Add("$expand", "User,User1");
                this.OnAfterTaskUpdated(item);
                return new ObjectResult(SingleResult.Create(itemToReturn));
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        [HttpPatch("/odata/TaskManagerDb/Tasks(TaskID={TaskID})")]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult PatchTask(Guid key, [FromBody]Delta<TaskManager.Server.Models.TaskManagerDb.Task> patch)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var item = this.context.Tasks.Where(i => i.TaskID == key).FirstOrDefault();

                if (item == null)
                {
                    return BadRequest();
                }
                patch.Patch(item);

                this.OnTaskUpdated(item);
                this.context.Tasks.Update(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.Tasks.Where(i => i.TaskID == key);
                Request.QueryString = Request.QueryString.Add("$expand", "User,User1");
                return new ObjectResult(SingleResult.Create(itemToReturn));
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        partial void OnTaskCreated(TaskManager.Server.Models.TaskManagerDb.Task item);
        partial void OnAfterTaskCreated(TaskManager.Server.Models.TaskManagerDb.Task item);

        [HttpPost]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult Post([FromBody] TaskManager.Server.Models.TaskManagerDb.Task item)
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

                this.OnTaskCreated(item);
                this.context.Tasks.Add(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.Tasks.Where(i => i.TaskID == item.TaskID);

                Request.QueryString = Request.QueryString.Add("$expand", "User,User1");

                this.OnAfterTaskCreated(item);

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
