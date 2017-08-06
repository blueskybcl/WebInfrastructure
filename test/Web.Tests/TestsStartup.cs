﻿namespace Web.Tests
{
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Skeleton.Web.Configuration;
    using Skeleton.Web.Testing.Extensions;

    public class TestsStartup : Startup
    {
        protected override IConfigurationBuilder AddAdditionalConfigurations(IHostingEnvironment env, IConfigurationBuilder configurationBuilder)
        {
            return configurationBuilder.AddCiDependentSettings(env.EnvironmentName);
        }

        protected override void AddLoggerProviders(ILoggerFactory loggerFactory)
        {
        }

        public TestsStartup(IHostingEnvironment env, CommandLineArgumentsProvider commandLineArgumentsProvider)
            : base(env, commandLineArgumentsProvider)
        {
        }
    }
}