using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Tracer.Models;

namespace Tracer.Processors.TraceToFileProcessor;

internal class TraceToFileProcessor : ITraceToFileProcessor
{
    public TraceToFileProcessor()
    {
        EnsureTraceFolderExist();
    }

    private readonly string _traceFilePath = @".\Traces\";

    public async Task ProcessRequestAsync(HttpContext httpContext)
    {
        var request = httpContext.Request;

        var reader = new StreamReader(request.Body);
        var rawMessage = await reader.ReadToEndAsync();

        var trace = new HttpTrace
        {
            RequestPath = request.Path,
            RequestBody = rawMessage
        };

        string json = JsonConvert.SerializeObject(trace);
        try
        {
            await File.WriteAllTextAsync(GetFilePath(httpContext.Connection.Id), json);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task ProcessResponseAsync(HttpContext httpContext)
    {
        try
        {
            var trace = await GetTrace(httpContext.Connection.Id);

            var response = httpContext.Response;

            var reader = new StreamReader(response.Body);
            var rawMessage = await reader.ReadToEndAsync();

            trace.ResponseBody = rawMessage;

            await UpdateTraceFile(trace, httpContext.Connection.Id);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    private async Task UpdateTraceFile(HttpTrace httpTrace, string connectionId)
    {
        string path = GetFilePath(connectionId);
        string json = JsonConvert.SerializeObject(httpTrace);

        await File.WriteAllTextAsync(path, json);
    }

    private async Task<HttpTrace> GetTrace(string connectionId)
    {
        string TString = "";
        using (StreamReader file = File.OpenText(GetFilePath(connectionId)))
        using (JsonTextReader reader = new JsonTextReader(file))
        {
            var jToken = await JToken.ReadFromAsync(reader);
            TString = jToken.ToString();
        }

        HttpTrace? dataObject = JsonConvert.DeserializeObject<HttpTrace>(TString);

        return dataObject ?? throw new FileNotFoundException();
    }

    private string GetFilePath(string connectionId)
    {
        if (connectionId == null) throw new ArgumentNullException(nameof(connectionId));

        return Path.Combine(_traceFilePath, $"trace-{connectionId}.json");
    }

    private void EnsureTraceFolderExist()
    {
        if (Directory.Exists(_traceFilePath)) return;

        Directory.CreateDirectory(_traceFilePath);
    }
}
