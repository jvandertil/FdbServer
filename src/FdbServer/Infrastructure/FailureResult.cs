using System;

namespace FdbServer.Infrastructure
{
    internal class FailureResult : IResult
    {
        public string Message { get; }

        public FailureResult(string message)
        {
            Message = message ?? string.Empty;
        }
    }

    internal class ExceptionResult : FailureResult
    {
        public ExceptionResult(Exception ex)
            : base(ex?.Message)
        {
            Exception = ex;
        }

        public Exception Exception { get; }
    }
}
