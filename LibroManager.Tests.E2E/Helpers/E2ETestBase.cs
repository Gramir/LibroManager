using System;
using System.Threading.Tasks;
using Microsoft.Playwright;

namespace LibroManager.Tests.E2E.Helpers
{
    public abstract class E2ETestBase
    {
        protected async Task RunWithReportAsync(
            IBrowserContext context,
            IPage page,
            string testName,
            Func<Task> testBody)
        {
            Exception? testException = null;
            try
            {
                await testBody();
            }
            catch (Exception ex)
            {
                testException = ex;
            }
            finally
            {
                if (testException != null)
                {
                    await PlaywrightReportHelper.SaveFailureReportAsync(context, page, testName, testException);
                    await context.CloseAsync();
                    throw testException;
                }
                else
                {
                    await context.CloseAsync();
                }
            }
        }
    }
}
