﻿using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(GamexApi.Startup))]

namespace GamexApi
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
