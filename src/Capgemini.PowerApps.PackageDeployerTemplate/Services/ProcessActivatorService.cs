﻿using Capgemini.PowerApps.PackageDeployerTemplate.Adapters;
using Capgemini.PowerApps.PackageDeployerTemplate.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Capgemini.PowerApps.PackageDeployerTemplate.Services
{
    public class ProcessActivatorService
    {
        private readonly ILogger logger;
        private readonly ICrmServiceAdapter crmSvc;

        public ProcessActivatorService(ILogger logger, ICrmServiceAdapter crmSvc)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.crmSvc = crmSvc ?? throw new ArgumentNullException(nameof(crmSvc));
        }

        public void Activate(IEnumerable<string> processesToActivate)
        {
            if (processesToActivate is null || !processesToActivate.Any())
            {
                this.logger.LogInformation("No processes to activate have been configured.");
                return;
            }

            var queryResponse = QueryWorkflowsByName(processesToActivate);
            var executeMultipleResponse = this.crmSvc.SetRecordsStateInBatch(queryResponse, 1, 2);
            if (executeMultipleResponse.IsFaulted)
            {
                this.logger.LogError($"Error activating processes.");
                this.logger.LogExecuteMultipleErrors(executeMultipleResponse);
            }
        }

        public void Deactivate(IEnumerable<string> processesToDeactivate)
        {
            if (processesToDeactivate is null || !processesToDeactivate.Any())
            {
                this.logger.LogInformation("No processes to deactivate have been configured.");
                return;
            }

            var queryResponse = QueryWorkflowsByName(processesToDeactivate);
            var executeMultipleResponse = this.crmSvc.SetRecordsStateInBatch(queryResponse, 0, 1);
            if (executeMultipleResponse.IsFaulted)
            {
                this.logger.LogError($"Error deactivating processes.");
                this.logger.LogExecuteMultipleErrors(executeMultipleResponse);
            }
        }

        public EntityCollection QueryWorkflowsByName(IEnumerable<object> values)
        {
            var query = new QueryByAttribute("workflow")
            {
                Attributes = { "name" },
                ColumnSet = new ColumnSet(false)
            };
            query.Values.AddRange(values);
            query.AddAttributeValue("type", 1);

            var results = crmSvc.RetrieveMultiple(query);
            this.logger.LogInformation($"Found {results.Entities.Count} of {values.Count()} workflows found.");
            return results;
        }
    }
}
