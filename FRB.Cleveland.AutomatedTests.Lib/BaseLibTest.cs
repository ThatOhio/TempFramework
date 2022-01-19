using System;
using Serilog;
using Xunit;

namespace FRB.Cleveland.AutomatedTests.Lib
{
    public class LibFixture : IDisposable
    {
        public void Dispose()
        {
            Log.CloseAndFlush();
        }
    }

    [Collection("Cle Test Collection")]
    public abstract class BaseLibTest : IDisposable
    {
        public LibFixture Fixture { get; }

        protected BaseLibTest(LibFixture fixture)
        {
            Fixture = fixture;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
        }
    }
}