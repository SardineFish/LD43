﻿using System;
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
        public object POST(string id)
        {
        }
    }
}
