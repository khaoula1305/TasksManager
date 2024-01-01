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
    [Route("odata/TaskManagerDb/TaskHistories")]
    public partial class TaskHistoriesController : ODataController
    {
        private TaskManager.Server.Data.TaskManagerDbContext context;

        public TaskHistoriesController(TaskManager.Server.Data.TaskManagerDbContext context)
        {
            this.context = context;
        }

    
        [HttpGet]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IEnumerable<TaskManager.Server.Models.TaskManagerDb.TaskHistory> GetTaskHistories()
        {
            var items = this.context.TaskHistories.AsQueryable<TaskManager.Server.Models.TaskManagerDb.TaskHistory>();
            this.OnTaskHistoriesRead(ref items);

            return items;
        }

        partial void OnTaskHistoriesRead(ref IQueryable<TaskManager.Server.Models.TaskManagerDb.TaskHistory> items);

        partial void OnTaskHistoryGet(ref SingleResult<TaskManager.Server.Models.TaskManagerDb.TaskHistory> item);

        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        [HttpGet("/odata/TaskManagerDb/TaskHistories(HistoryID={HistoryID})")]
        public SingleResult<TaskManager.Server.Models.TaskManagerDb.TaskHistory> GetTaskHistory(Guid key)
        {
            var items = this.context.TaskHistories.Where(i => i.HistoryID == key);
            var result = SingleResult.Create(items);

            OnTaskHistoryGet(ref result);

            return result;
        }
        partial void OnTaskHistoryDeleted(TaskManager.Server.Models.TaskManagerDb.TaskHistory item);
        partial void OnAfterTaskHistoryDeleted(TaskManager.Server.Models.TaskManagerDb.TaskHistory item);

        [HttpDelete("/odata/TaskManagerDb/TaskHistories(HistoryID={HistoryID})")]
        public IActionResult DeleteTaskHistory(Guid key)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }


                var item = this.context.TaskHistories
                    .Where(i => i.HistoryID == key)
                    .FirstOrDefault();

                if (item == null)
                {
                    return BadRequest();
                }
                this.OnTaskHistoryDeleted(item);
                this.context.TaskHistories.Remove(item);
                this.context.SaveChanges();
                this.OnAfterTaskHistoryDeleted(item);

                return new NoContentResult();

            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        partial void OnTaskHistoryUpdated(TaskManager.Server.Models.TaskManagerDb.TaskHistory item);
        partial void OnAfterTaskHistoryUpdated(TaskManager.Server.Models.TaskManagerDb.TaskHistory item);

        [HttpPut("/odata/TaskManagerDb/TaskHistories(HistoryID={HistoryID})")]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult PutTaskHistory(Guid key, [FromBody]TaskManager.Server.Models.TaskManagerDb.TaskHistory item)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (item == null || (item.HistoryID != key))
                {
                    return BadRequest();
                }
                this.OnTaskHistoryUpdated(item);
                this.context.TaskHistories.Update(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.TaskHistories.Where(i => i.HistoryID == key);
                Request.QueryString = Request.QueryString.Add("$expand", "Task,User");
                this.OnAfterTaskHistoryUpdated(item);
                return new ObjectResult(SingleResult.Create(itemToReturn));
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        [HttpPatch("/odata/TaskManagerDb/TaskHistories(HistoryID={HistoryID})")]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult PatchTaskHistory(Guid key, [FromBody]Delta<TaskManager.Server.Models.TaskManagerDb.TaskHistory> patch)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var item = this.context.TaskHistories.Where(i => i.HistoryID == key).FirstOrDefault();

                if (item == null)
                {
                    return BadRequest();
                }
                patch.Patch(item);

                this.OnTaskHistoryUpdated(item);
                this.context.TaskHistories.Update(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.TaskHistories.Where(i => i.HistoryID == key);
                Request.QueryString = Request.QueryString.Add("$expand", "Task,User");
                return new ObjectResult(SingleResult.Create(itemToReturn));
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        partial void OnTaskHistoryCreated(TaskManager.Server.Models.TaskManagerDb.TaskHistory item);
        partial void OnAfterTaskHistoryCreated(TaskManager.Server.Models.TaskManagerDb.TaskHistory item);

        [HttpPost]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult Post([FromBody] TaskManager.Server.Models.TaskManagerDb.TaskHistory item)
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

                this.OnTaskHistoryCreated(item);
                this.context.TaskHistories.Add(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.TaskHistories.Where(i => i.HistoryID == item.HistoryID);

                Request.QueryString = Request.QueryString.Add("$expand", "Task,User");

                this.OnAfterTaskHistoryCreated(item);

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
