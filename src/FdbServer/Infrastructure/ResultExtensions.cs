namespace FdbServer.Infrastructure
{
    using System;
    using System.Threading.Tasks;

    internal static class ResultExtensions
    {
        public static IResult OnFailure(this IResult result, Func<IResult, IResult> onFail)
        {
            if (result is FailureResult)
            {
                return onFail(result);
            }

            return result;
        }

        public static Task<IResult> OnFailureAsync(this IResult result, Func<IResult, Task<IResult>> onFail)
        {
            if (result is FailureResult)
            {
                return onFail(result);
            }

            return Task.FromResult(result);
        }

        public static IResult Then(this IResult result, Func<IResult, IResult> onSuccess)
        {
            if (result is SuccessResult)
            {
                return onSuccess(result);
            }

            return result;
        }

        public static Task<IResult> ThenAsync(this IResult result, Func<IResult, Task<IResult>> onSuccess)
        {
            if (result is SuccessResult)
            {
                return onSuccess(result);
            }

            return Task.FromResult(result);
        }

        public static async Task<IResult> ThenAsync(this Task<IResult> result, Func<IResult, Task<IResult>> onSuccess)
        {
            var actualResult = await result.ConfigureAwait(false);

            if (actualResult is SuccessResult)
            {
                return await onSuccess(actualResult).ConfigureAwait(false);
            }

            return actualResult;
        }

        public static IResult ThenAlways(this IResult result, Action<IResult> action)
        {
            action(result);

            return result;
        }

        public static void EnsureSuccess(this IResult result)
        {
            if (result is ExceptionResult e)
            {
                throw new Exception("Exception encountered", e.Exception);
            }

            if (result is FailureResult)
            {
                throw new Exception();
            }
        }
    }
}
