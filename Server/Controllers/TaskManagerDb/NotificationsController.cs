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
    [Route("odata/TaskManagerDb/Notifications")]
    public partial class NotificationsController : ODataController
    {
        private TaskManager.Server.Data.TaskManagerDbContext context;

        public NotificationsController(TaskManager.Server.Data.TaskManagerDbContext context)
        {
            this.context = context;
        }

    
        [HttpGet]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IEnumerable<TaskManager.Server.Models.TaskManagerDb.Notification> GetNotifications()
        {
            var items = this.context.Notifications.AsQueryable<TaskManager.Server.Models.TaskManagerDb.Notification>();
            this.OnNotificationsRead(ref items);

            return items;
        }

        partial void OnNotificationsRead(ref IQueryable<TaskManager.Server.Models.TaskManagerDb.Notification> items);

        partial void OnNotificationGet(ref SingleResult<TaskManager.Server.Models.TaskManagerDb.Notification> item);

        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        [HttpGet("/odata/TaskManagerDb/Notifications(NotificationID={NotificationID})")]
        public SingleResult<TaskManager.Server.Models.TaskManagerDb.Notification> GetNotification(Guid key)
        {
            var items = this.context.Notifications.Where(i => i.NotificationID == key);
            var result = SingleResult.Create(items);

            OnNotificationGet(ref result);

            return result;
        }
        partial void OnNotificationDeleted(TaskManager.Server.Models.TaskManagerDb.Notification item);
        partial void OnAfterNotificationDeleted(TaskManager.Server.Models.TaskManagerDb.Notification item);

        [HttpDelete("/odata/TaskManagerDb/Notifications(NotificationID={NotificationID})")]
        public IActionResult DeleteNotification(Guid key)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }


                var item = this.context.Notifications
                    .Where(i => i.NotificationID == key)
                    .FirstOrDefault();

                if (item == null)
                {
                    return BadRequest();
                }
                this.OnNotificationDeleted(item);
                this.context.Notifications.Remove(item);
                this.context.SaveChanges();
                this.OnAfterNotificationDeleted(item);

                return new NoContentResult();

            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        partial void OnNotificationUpdated(TaskManager.Server.Models.TaskManagerDb.Notification item);
        partial void OnAfterNotificationUpdated(TaskManager.Server.Models.TaskManagerDb.Notification item);

        [HttpPut("/odata/TaskManagerDb/Notifications(NotificationID={NotificationID})")]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult PutNotification(Guid key, [FromBody]TaskManager.Server.Models.TaskManagerDb.Notification item)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (item == null || (item.NotificationID != key))
                {
                    return BadRequest();
                }
                this.OnNotificationUpdated(item);
                this.context.Notifications.Update(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.Notifications.Where(i => i.NotificationID == key);
                Request.QueryString = Request.QueryString.Add("$expand", "Task,User");
                this.OnAfterNotificationUpdated(item);
                return new ObjectResult(SingleResult.Create(itemToReturn));
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        [HttpPatch("/odata/TaskManagerDb/Notifications(NotificationID={NotificationID})")]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult PatchNotification(Guid key, [FromBody]Delta<TaskManager.Server.Models.TaskManagerDb.Notification> patch)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var item = this.context.Notifications.Where(i => i.NotificationID == key).FirstOrDefault();

                if (item == null)
                {
                    return BadRequest();
                }
                patch.Patch(item);

                this.OnNotificationUpdated(item);
                this.context.Notifications.Update(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.Notifications.Where(i => i.NotificationID == key);
                Request.QueryString = Request.QueryString.Add("$expand", "Task,User");
                return new ObjectResult(SingleResult.Create(itemToReturn));
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        partial void OnNotificationCreated(TaskManager.Server.Models.TaskManagerDb.Notification item);
        partial void OnAfterNotificationCreated(TaskManager.Server.Models.TaskManagerDb.Notification item);

        [HttpPost]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult Post([FromBody] TaskManager.Server.Models.TaskManagerDb.Notification item)
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

                this.OnNotificationCreated(item);
                this.context.Notifications.Add(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.Notifications.Where(i => i.NotificationID == item.NotificationID);

                Request.QueryString = Request.QueryString.Add("$expand", "Task,User");

                this.OnAfterNotificationCreated(item);

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
