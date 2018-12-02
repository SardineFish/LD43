using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace LD43GameServer.Controllers
{
    [Route("/get-records")]
    public class GetRecordsController : Controller
    {
        [HttpPost("{room}")]
        public string Post(string room)
        {
            var records = GameServer.Instance.GetRecordsFromRoom(Guid.Parse(room));
            if(records==null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return JsonConvert.SerializeObject(records);
        }
    }
}