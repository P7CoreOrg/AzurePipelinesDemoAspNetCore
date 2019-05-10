using Microsoft.Extensions.Configuration;
using TheWebApp;
using XUnitHelpers;

namespace xUnit_TheWebAppTests
{

    public class MyTestServerFixture : TestServerFixture<Startup>
    {
        protected override string RelativePathToHostProject => @"..\..\..\..\TheWebApp";

        protected override void LoadConfigurations(IConfigurationBuilder config, string environmentName)
        {
            Program.LoadConfigurations(config, environmentName);
        }
    }
}
