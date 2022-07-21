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
using JsonLogic.Net;


namespace AzureAssignment
{
    public static class Function1
    {
        [FunctionName("Function1")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            string connectionString = @"BlobEndpoint=https://assignmentyoda.blob.core.windows.net/;SharedAccessSignature=sv=2021-06-08&ss=bfqt&srt=sco&sp=rwdlacupiytfx&se=2022-07-21T19:33:45Z&st=2022-07-21T11:33:45Z&spr=https&sig=asT1v2ypPLxXUduSyG5bKtfwBJ7pVRBXQPrBq3PztQA%3D";

            string blobName = "output";

            string containerName = "output";

            // Get a reference to a container
            BlobContainerClient container = new BlobContainerClient(connectionString, containerName);

            // Get a reference to a blob
            BlobClient blob = container.GetBlobClient(blobName);

            // Download file to a given path from Azure storage
            string downloadPath = Path.Combine(Path.GetTempPath(), blobName);
            var response = await blob.DownloadToAsync(downloadPath);

            string line;
            List<Employee> employeeList = new List<Employee>();
            var evaluator = new JsonLogicEvaluator(EvaluateOperators.Default);
            string jsonText = """{ "if" : [{">" : [ { "var" : "SALARY" }, 4000 ]} ,true, false] } """;
            var rule = JObject.Parse(jsonText);
            StreamReader sr = new StreamReader(downloadPath);
            while ((line = sr.ReadLine()) != null) {
                //string[] words = line.Split(':');
                Employee employee = JsonConvert.DeserializeObject<Employee>(line);
                string data  =JsonConvert.SerializeObject(employee);
                var finalData = JObject.Parse(data);
                object result = evaluator.Apply(rule, finalData);
                if (result.ToString()=="True") {
                    employeeList.Add(employee);
                }
            }
            sr.Close();
            //foreach (Employee e in employeeList)
            //{
            //    string ser = JsonConvert.SerializeObject(e);
            //}
            using FileStream fileStream = File.Create(downloadPath + "_final.json");
            await System.Text.Json.JsonSerializer.SerializeAsync(fileStream,employeeList);
            fileStream.Close();

            
            BlobContainerClient outputContainer = new BlobContainerClient(connectionString, containerName);
            BlobClient outputBlob = outputContainer.GetBlobClient("output_final.json");
            using FileStream uploadFileStream = File.OpenRead(downloadPath + "_final.json");

            await outputBlob.UploadAsync(uploadFileStream, true);
            uploadFileStream.Close();


            return new OkObjectResult("ok");


        }
    }
}
