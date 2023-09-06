using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Tracer.Models;

namespace Tracer.Processors.TraceToFileProcessor;

internal class TraceToFileProcessor : ITraceToFileProcessor, IAsyncDisposable
{

    private readonly string _traceFilePath = @".\Traces\";
    private MemoryStream? responseBodyMemoryStream; 
    private Stream? originalResponseBody;
    private HttpTrace Trace;

    public TraceToFileProcessor()
    {
        EnsureTraceFolderExist();
        Trace = new HttpTrace();
    }

    #region Public Methods

    public async Task ProcessRequestAsync(HttpContext httpContext)
    {
        try
        {
            ReplaceResponseBodyStream(httpContext);

            var request = httpContext.Request;
            var reader = new StreamReader(request.Body);
            var requestString = await reader.ReadToEndAsync();
            var requestObject = JsonConvert.DeserializeObject(requestString);

            Trace.RequestPath = request.Path;
            Trace.RequestBody = requestObject;
        }
        catch (Exception ex)
        {
            UpsertTracerException(ex, Trace);
        }
    }

    public async Task ProcessResponseAsync(HttpContext httpContext)
    {
        try
        {
            var response = httpContext.Response;

            if(responseBodyMemoryStream is null) throw new NullReferenceException(nameof(responseBodyMemoryStream));

            using var responseBodyStream = new StreamReader(responseBodyMemoryStream);
            responseBodyMemoryStream.Position = 0;
            var responseBodyString = await responseBodyStream.ReadToEndAsync();

            Trace.ResponseBody = JsonConvert.DeserializeObject(responseBodyString);
            Trace.ResponseStatusCode = (short)httpContext.Response.StatusCode;

            await ReassignResponseBodyStream();

            await CreateTraceFile(Trace, httpContext.Connection.Id);
        }
        catch (Exception ex)
        {
            UpsertTracerException(ex, Trace);
            await CreateTraceFile(Trace, httpContext.Connection.Id);
        }
    }

    public async Task ProcessUnhandledException(HttpContext httpContext, Exception exception)
    {
        try
        {
            if (responseBodyMemoryStream is null) throw new NullReferenceException(nameof(responseBodyMemoryStream));

            using var responseBodyStream = new StreamReader(responseBodyMemoryStream);
            responseBodyMemoryStream.Position = 0;
            var responseBody = await responseBodyStream.ReadToEndAsync();

            Trace.ResponseBody = responseBody;
            Trace.ApplicationError = new Error(exception.Message, exception);
            Trace.ResponseStatusCode = 500;

            await CreateTraceFile(Trace, httpContext.Connection.Id);
        }
        catch (Exception ex)
        {
            UpsertTracerException(ex, Trace);
            await CreateTraceFile(Trace, httpContext.Connection.Id);
        }
    }

    public void AddTraceEvent(TraceEvent traceEvent)
    {
        Trace.TraceEvents ??= new List<TraceEvent>();

        Trace.TraceEvents.Add(traceEvent);
    }

    #endregion

    #region Private Methods

    private void UpsertTracerException(Exception exception, HttpTrace? httpTrace)
    {
        if (httpTrace is not null)
        {
            httpTrace.TracerError = new Error(exception.Message, exception);
        }
        else
        {
            httpTrace = new HttpTrace
            {
                TracerError = new Error(exception.Message, exception)
            };
        }
    }

    private async Task CreateTraceFile(HttpTrace trace, string connectionId)
    {
        string json = JsonConvert.SerializeObject(trace, Formatting.Indented);
        await File.WriteAllTextAsync(GetFilePath(connectionId), json);
    }

    private string GetFilePath(string connectionId)
    {
        if (connectionId == null) throw new ArgumentNullException(nameof(connectionId));

        return Path.Combine(_traceFilePath, $"trace-{connectionId}.json");
    }

    private void ReplaceResponseBodyStream(HttpContext httpContext)
    {
        this.originalResponseBody = httpContext.Response.Body;
        this.responseBodyMemoryStream = new MemoryStream();
        httpContext.Response.Body = responseBodyMemoryStream;
    }

    private async Task ReassignResponseBodyStream()
    {
        if(responseBodyMemoryStream is null || originalResponseBody is null)
        {
            throw new ArgumentNullException($"{nameof(responseBodyMemoryStream)} OR {nameof(originalResponseBody)}");
        }
        else
        {
            responseBodyMemoryStream.Position = 0;
            await responseBodyMemoryStream.CopyToAsync(originalResponseBody);
        }
    }

    private void EnsureTraceFolderExist()
    {
        if (Directory.Exists(_traceFilePath)) return;

        Directory.CreateDirectory(_traceFilePath);
    }

    #region Dispose
    private bool isDisposing = false;

    public async ValueTask DisposeAsync()
    {
        if(!isDisposing)
        {
            Console.WriteLine($"Disposing TraceToFileProcessor");
            isDisposing = true;
            await DisposeResourcesAsync();
        }
    }

    private async Task DisposeResourcesAsync()
    {
        if(this.responseBodyMemoryStream is not null)
        {
            await this.responseBodyMemoryStream.FlushAsync();
            await this.responseBodyMemoryStream.DisposeAsync();
        }
    }
    #endregion

    #endregion
}
