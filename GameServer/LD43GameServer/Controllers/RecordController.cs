using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IO;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LD43GameServer
{
    [Route("/record")]
    public class RecordController : Controller
    {
        [HttpPost("{id}")]
        public void POST(string id)
        {
            PlayerRecord record;
            using (StreamReader sr = new StreamReader(Request.Body)) 
            using (JsonTextReader jtr = new JsonTextReader(sr))
            {
                record = JsonSerializer.Create().Deserialize<PlayerRecord>(jtr);   
            }
            if (!GameServer.Instance.CheckPlayer(Guid.Parse(record.ID)))
            {
                Response.StatusCode = 403;
                return;
            }

        }
    }
}
