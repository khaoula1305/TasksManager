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
    [Route("odata/TaskManagerDb/TaskSubPoints")]
    public partial class TaskSubPointsController : ODataController
    {
        private TaskManager.Server.Data.TaskManagerDbContext context;

        public TaskSubPointsController(TaskManager.Server.Data.TaskManagerDbContext context)
        {
            this.context = context;
        }

    
        [HttpGet]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IEnumerable<TaskManager.Server.Models.TaskManagerDb.TaskSubPoint> GetTaskSubPoints()
        {
            var items = this.context.TaskSubPoints.AsQueryable<TaskManager.Server.Models.TaskManagerDb.TaskSubPoint>();
            this.OnTaskSubPointsRead(ref items);

            return items;
        }

        partial void OnTaskSubPointsRead(ref IQueryable<TaskManager.Server.Models.TaskManagerDb.TaskSubPoint> items);

        partial void OnTaskSubPointGet(ref SingleResult<TaskManager.Server.Models.TaskManagerDb.TaskSubPoint> item);

        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        [HttpGet("/odata/TaskManagerDb/TaskSubPoints(SubPointID={SubPointID})")]
        public SingleResult<TaskManager.Server.Models.TaskManagerDb.TaskSubPoint> GetTaskSubPoint(Guid key)
        {
            var items = this.context.TaskSubPoints.Where(i => i.SubPointID == key);
            var result = SingleResult.Create(items);

            OnTaskSubPointGet(ref result);

            return result;
        }
        partial void OnTaskSubPointDeleted(TaskManager.Server.Models.TaskManagerDb.TaskSubPoint item);
        partial void OnAfterTaskSubPointDeleted(TaskManager.Server.Models.TaskManagerDb.TaskSubPoint item);

        [HttpDelete("/odata/TaskManagerDb/TaskSubPoints(SubPointID={SubPointID})")]
        public IActionResult DeleteTaskSubPoint(Guid key)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }


                var item = this.context.TaskSubPoints
                    .Where(i => i.SubPointID == key)
                    .FirstOrDefault();

                if (item == null)
                {
                    return BadRequest();
                }
                this.OnTaskSubPointDeleted(item);
                this.context.TaskSubPoints.Remove(item);
                this.context.SaveChanges();
                this.OnAfterTaskSubPointDeleted(item);

                return new NoContentResult();

            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        partial void OnTaskSubPointUpdated(TaskManager.Server.Models.TaskManagerDb.TaskSubPoint item);
        partial void OnAfterTaskSubPointUpdated(TaskManager.Server.Models.TaskManagerDb.TaskSubPoint item);

        [HttpPut("/odata/TaskManagerDb/TaskSubPoints(SubPointID={SubPointID})")]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult PutTaskSubPoint(Guid key, [FromBody]TaskManager.Server.Models.TaskManagerDb.TaskSubPoint item)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (item == null || (item.SubPointID != key))
                {
                    return BadRequest();
                }
                this.OnTaskSubPointUpdated(item);
                this.context.TaskSubPoints.Update(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.TaskSubPoints.Where(i => i.SubPointID == key);
                Request.QueryString = Request.QueryString.Add("$expand", "Task");
                this.OnAfterTaskSubPointUpdated(item);
                return new ObjectResult(SingleResult.Create(itemToReturn));
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        [HttpPatch("/odata/TaskManagerDb/TaskSubPoints(SubPointID={SubPointID})")]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult PatchTaskSubPoint(Guid key, [FromBody]Delta<TaskManager.Server.Models.TaskManagerDb.TaskSubPoint> patch)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var item = this.context.TaskSubPoints.Where(i => i.SubPointID == key).FirstOrDefault();

                if (item == null)
                {
                    return BadRequest();
                }
                patch.Patch(item);

                this.OnTaskSubPointUpdated(item);
                this.context.TaskSubPoints.Update(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.TaskSubPoints.Where(i => i.SubPointID == key);
                Request.QueryString = Request.QueryString.Add("$expand", "Task");
                return new ObjectResult(SingleResult.Create(itemToReturn));
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        partial void OnTaskSubPointCreated(TaskManager.Server.Models.TaskManagerDb.TaskSubPoint item);
        partial void OnAfterTaskSubPointCreated(TaskManager.Server.Models.TaskManagerDb.TaskSubPoint item);

        [HttpPost]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult Post([FromBody] TaskManager.Server.Models.TaskManagerDb.TaskSubPoint item)
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

                this.OnTaskSubPointCreated(item);
                this.context.TaskSubPoints.Add(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.TaskSubPoints.Where(i => i.SubPointID == item.SubPointID);

                Request.QueryString = Request.QueryString.Add("$expand", "Task");

                this.OnAfterTaskSubPointCreated(item);

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
