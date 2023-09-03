using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Net.Http;
using Tracer.Models;

namespace Tracer.Processors.TraceToFileProcessor;

internal class TraceToFileProcessor : ITraceToFileProcessor
{
    public TraceToFileProcessor()
    {
        EnsureTraceFolderExist();
    }

    private readonly string _traceFilePath = @".\Traces\";
    private Stream originalResponseBody;
    private MemoryStream responseBodyMemoryStream;

    public async Task ProcessRequestAsync(HttpContext httpContext)
    {
        var trace = null as HttpTrace;

        try
        {
            this.originalResponseBody = httpContext.Response.Body;
            this.responseBodyMemoryStream = new MemoryStream();
            httpContext.Response.Body = responseBodyMemoryStream;

            var request = httpContext.Request;

            var reader = new StreamReader(request.Body);
            var rawMessage = await reader.ReadToEndAsync();

            trace = new HttpTrace
            {
                RequestPath = request.Path,
                RequestBody = rawMessage
            };

            await CreateTraceFile(trace, httpContext.Connection.Id);
        }
        catch (Exception ex)
        {
            UpsertException(ex, trace);
            await CreateTraceFile(trace, httpContext.Connection.Id);
        }
    }

    public async Task ProcessResponseAsync(HttpContext httpContext)
    {
        var trace = null as HttpTrace;

        try
        {
            trace = await GetTrace(httpContext.Connection.Id);

            var response = httpContext.Response;

            using (var responseBodyStream = new StreamReader(responseBodyMemoryStream))
            {
                responseBodyMemoryStream.Position = 0;
                var responseBody = await responseBodyStream.ReadToEndAsync();

                trace.ResponseBody = responseBody;
                trace.ResponseStatusCode = (short) httpContext.Response.StatusCode;

                responseBodyMemoryStream.Position = 0;
                await responseBodyMemoryStream.CopyToAsync(originalResponseBody);

                await UpdateTraceFile(trace, httpContext.Connection.Id);
            }            
        }
        catch (Exception ex)
        {
            UpsertException(ex, trace);
            await UpdateTraceFile(trace, httpContext.Connection.Id);
        }
        finally
        {
            await responseBodyMemoryStream.DisposeAsync();
            await originalResponseBody.DisposeAsync();
        }
    }

    public Task ProcessUnhandledException(HttpContext httpContext, Exception exception)
    {
        throw new NotImplementedException();
    }

    private void UpsertException(Exception exception, HttpTrace? httpTrace)
    {
        if (httpTrace != null)
        {
            httpTrace.TracerError = new Error
            {
                ErrorMessage = exception.Message,
                Exception = exception,
            };
        }
        else
        {
            httpTrace = new HttpTrace
            {
                TracerError = new Error
                {
                    ErrorMessage = exception.Message,
                    Exception = exception,
                }
            };
        }
    }

    private async Task CreateTraceFile(HttpTrace trace, string connectionId)
    {
        string json = JsonConvert.SerializeObject(trace);
        await File.WriteAllTextAsync(GetFilePath(connectionId), json);
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
