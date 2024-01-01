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
    [Route("odata/TaskManagerDb/TimeLogs")]
    public partial class TimeLogsController : ODataController
    {
        private TaskManager.Server.Data.TaskManagerDbContext context;

        public TimeLogsController(TaskManager.Server.Data.TaskManagerDbContext context)
        {
            this.context = context;
        }

    
        [HttpGet]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IEnumerable<TaskManager.Server.Models.TaskManagerDb.TimeLog> GetTimeLogs()
        {
            var items = this.context.TimeLogs.AsQueryable<TaskManager.Server.Models.TaskManagerDb.TimeLog>();
            this.OnTimeLogsRead(ref items);

            return items;
        }

        partial void OnTimeLogsRead(ref IQueryable<TaskManager.Server.Models.TaskManagerDb.TimeLog> items);

        partial void OnTimeLogGet(ref SingleResult<TaskManager.Server.Models.TaskManagerDb.TimeLog> item);

        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        [HttpGet("/odata/TaskManagerDb/TimeLogs(TimeLogID={TimeLogID})")]
        public SingleResult<TaskManager.Server.Models.TaskManagerDb.TimeLog> GetTimeLog(Guid key)
        {
            var items = this.context.TimeLogs.Where(i => i.TimeLogID == key);
            var result = SingleResult.Create(items);

            OnTimeLogGet(ref result);

            return result;
        }
        partial void OnTimeLogDeleted(TaskManager.Server.Models.TaskManagerDb.TimeLog item);
        partial void OnAfterTimeLogDeleted(TaskManager.Server.Models.TaskManagerDb.TimeLog item);

        [HttpDelete("/odata/TaskManagerDb/TimeLogs(TimeLogID={TimeLogID})")]
        public IActionResult DeleteTimeLog(Guid key)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }


                var item = this.context.TimeLogs
                    .Where(i => i.TimeLogID == key)
                    .FirstOrDefault();

                if (item == null)
                {
                    return BadRequest();
                }
                this.OnTimeLogDeleted(item);
                this.context.TimeLogs.Remove(item);
                this.context.SaveChanges();
                this.OnAfterTimeLogDeleted(item);

                return new NoContentResult();

            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        partial void OnTimeLogUpdated(TaskManager.Server.Models.TaskManagerDb.TimeLog item);
        partial void OnAfterTimeLogUpdated(TaskManager.Server.Models.TaskManagerDb.TimeLog item);

        [HttpPut("/odata/TaskManagerDb/TimeLogs(TimeLogID={TimeLogID})")]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult PutTimeLog(Guid key, [FromBody]TaskManager.Server.Models.TaskManagerDb.TimeLog item)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (item == null || (item.TimeLogID != key))
                {
                    return BadRequest();
                }
                this.OnTimeLogUpdated(item);
                this.context.TimeLogs.Update(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.TimeLogs.Where(i => i.TimeLogID == key);
                Request.QueryString = Request.QueryString.Add("$expand", "Task,User");
                this.OnAfterTimeLogUpdated(item);
                return new ObjectResult(SingleResult.Create(itemToReturn));
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        [HttpPatch("/odata/TaskManagerDb/TimeLogs(TimeLogID={TimeLogID})")]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult PatchTimeLog(Guid key, [FromBody]Delta<TaskManager.Server.Models.TaskManagerDb.TimeLog> patch)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var item = this.context.TimeLogs.Where(i => i.TimeLogID == key).FirstOrDefault();

                if (item == null)
                {
                    return BadRequest();
                }
                patch.Patch(item);

                this.OnTimeLogUpdated(item);
                this.context.TimeLogs.Update(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.TimeLogs.Where(i => i.TimeLogID == key);
                Request.QueryString = Request.QueryString.Add("$expand", "Task,User");
                return new ObjectResult(SingleResult.Create(itemToReturn));
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        partial void OnTimeLogCreated(TaskManager.Server.Models.TaskManagerDb.TimeLog item);
        partial void OnAfterTimeLogCreated(TaskManager.Server.Models.TaskManagerDb.TimeLog item);

        [HttpPost]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult Post([FromBody] TaskManager.Server.Models.TaskManagerDb.TimeLog item)
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

                this.OnTimeLogCreated(item);
                this.context.TimeLogs.Add(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.TimeLogs.Where(i => i.TimeLogID == item.TimeLogID);

                Request.QueryString = Request.QueryString.Add("$expand", "Task,User");

                this.OnAfterTimeLogCreated(item);

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
