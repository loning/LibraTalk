﻿using System;
using System.Threading.Tasks;
using System.Web.Http;
using LibraProgramming.Grains.Interfaces;
using Orleans;

namespace WebHost.Controllers
{
    public class MeController : ApiController
    {
        public Task<string> Get(Guid id)
        {
            var user = GrainClient.GrainFactory.GetGrain<IChatUser>(id);
            return user.GetName();
        }

        public Task Post(Guid id, [FromBody] string userName)
        {
            var user = GrainClient.GrainFactory.GetGrain<IChatUser>(id);
            return user.SetName(userName);
        }
    }
}