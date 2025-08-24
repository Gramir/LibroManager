using System;
using Xunit;

namespace LibroManager.Tests.E2E.Helpers
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class UseSnapshotAttribute(string snapshotName) : Xunit.Sdk.BeforeAfterTestAttribute
    {
        private readonly string _snapshotName = snapshotName;

        public override void Before(System.Reflection.MethodInfo methodUnderTest)
        {
            Environment.SetEnvironmentVariable("E2E_SNAPSHOT", _snapshotName);
        }
        public override void After(System.Reflection.MethodInfo methodUnderTest)
        {
            Environment.SetEnvironmentVariable("E2E_SNAPSHOT", null);
        }
    }
}
