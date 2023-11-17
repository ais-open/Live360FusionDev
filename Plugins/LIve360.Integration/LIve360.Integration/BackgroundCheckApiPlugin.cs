using Microsoft.Xrm.Sdk;
using System;

namespace Live360.Plugins
{
  public class BackgroundCheckApiPlugin : IPlugin
  {
    public void Execute(IServiceProvider serviceProvider)
    {
      var tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

      // Obtain the execution context from the service provider.
      var context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

      var orgFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));

      // create without the user uses the system user
      var systemUserService = orgFactory.CreateOrganizationService(null);

      // get inputs

      var idString = context.InputParameters["BackgroundCheckID"]?.ToString();
      var safeToHireString = context.InputParameters["IsSafeToHire"]?.ToString();

      tracingService.Trace($"ID String: {idString}, SafeToHire? {safeToHireString}");

      var id = Guid.Parse(idString);
      bool isSafeToHire = safeToHireString.ToLower() == "true";

      // create an "update" entity
      var updateEntity = new Entity("ais_backgroundcheck")
      {
        Id = id
      };

      updateEntity.Attributes.Add("ais_issafe", isSafeToHire);

      // save results to the DB
      systemUserService.Update(updateEntity);
    }
  }
}
