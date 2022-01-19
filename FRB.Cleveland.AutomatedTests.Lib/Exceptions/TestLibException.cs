using System;

namespace FRB.Cleveland.AutomatedTests.Lib.Exceptions
{
    public class TestLibException : Exception
    {
        public TestLibException(string message) : base(message) {}
    }
}