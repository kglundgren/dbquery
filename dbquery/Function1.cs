using System;
using System.Data.SqlClient;
using System.Configuration;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;

namespace dbquery
{
    public static class Function1
    {
        [FunctionName("Function1")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log, ExecutionContext context)
        {

            /* Works, but very cumbersome.
            var config = new ConfigurationBuilder()
            .SetBasePath(context.FunctionAppDirectory)
            .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();
            var appSettingValue = config["localdb"];
            var connection = config.GetConnectionString("localdb");
            */

            /* Iterate through environment variables (just for fun).
            var envs = Environment.GetEnvironmentVariables();
            foreach (var key in envs.Keys) {
                var value = envs[key];
                Console.WriteLine(value);
            }
            */

            // Works, finally! Need to have connection string in "Values" object in local.settings.json.
            //var connstr = Environment.GetEnvironmentVariable("localdb");
            //Console.WriteLine($"localdb connection string: {connstr}");

            log.LogInformation("C# HTTP trigger function processed a request.");

            var dbconn = new DatabaseConnection();
            await dbconn.TestConnection();
            await dbconn.Query("select * from Customers");

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            string responseMessage = string.IsNullOrEmpty(name)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {name}. This HTTP triggered function executed successfully.";

            return new OkObjectResult(responseMessage);
        }
    }
}
