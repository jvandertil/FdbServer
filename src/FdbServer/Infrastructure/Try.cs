using System;
using System.Threading.Tasks;

namespace FdbServer.Infrastructure
{
    internal static class Try
    {
        public static IResult Wrap(Action action)
        {
            try
            {
                action();

                return new SuccessResult();
            }
            catch (Exception e)
            {
                return new ExceptionResult(e);
            }
        }

        public static IResult Wrap(Func<IResult> func)
        {
            try
            {
                return func();
            }
            catch (Exception e)
            {
                return new ExceptionResult(e);
            }
        }

        public static async Task<IResult> Wrap(Func<Task> actionAsync)
        {
            try
            {
                await actionAsync().ConfigureAwait(false);

                return new SuccessResult();
            }
            catch (Exception e)
            {
                return new ExceptionResult(e);
            }
        }
    }
}
