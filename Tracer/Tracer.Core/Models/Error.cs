namespace Tracer.Models;

public class Error
{
    public Error(string errorMessage, Exception exception)
    {
        ErrorMessage=errorMessage;
        Exception=exception;
    }

    public string ErrorMessage { get; set; }
    public Exception Exception { get; set; }
}