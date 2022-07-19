using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Rest;
using Microsoft.Azure.Management.DataFactory;
using Microsoft.Azure.Management.DataFactory.Models;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using Azure.Storage.Blobs;

namespace AzureAssignment
{
    public static class Function1
    {
        [FunctionName("Function1")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            string connectionString = @"DefaultEndpointsProtocol=https;AccountName=assignmentyoda;AccountKey=A1TGgaPfmigb0mZZIBg1jNPcDmfJPNdN9j9Q4+VgVMWA7H5sJMJrUtRyTBUJCzZnfC4xKJsv6PHJ+AStMUVz7Q==;EndpointSuffix=core.windows.net";

            string blobName = "output";

            string containerName = "output";

            // Get a reference to a container
            BlobContainerClient container = new BlobContainerClient(connectionString, containerName);

            // Get a reference to a blob
            BlobClient blob = container.GetBlobClient(blobName);

            // Download file to a given path from Azure storage
            string downloadPath = @"C:\Users\Divyam Jalota\source\repos\AzureAssignment\AzureAssignment";
            var response = await blob.DownloadToAsync(downloadPath);


            return new OkObjectResult("ok");


        }
    }
}
