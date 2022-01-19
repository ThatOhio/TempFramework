using System;
using FluentAssertions;
using FRB.Cleveland.AutomatedTests.Lib;
using FRB.Cleveland.AutomatedTests.Lib.Exceptions;
using FRB.Cleveland.AutomatedTests.Lib.Utilities.Wait;
using Moq;
using Xunit;

namespace FRB.Cleveland.AutomatedTests.CoreLibTests
{
    public class WaitTests
    {
        protected class TestWait : Wait<IBrowser>
        {
            /// <inheritdoc />
            public TestWait(IBrowser context, TimeSpan timeout, TimeSpan pollingInterval)
                : base(context, timeout, pollingInterval)
            {
            }

            /// <inheritdoc />
            protected override void ThrowTimeoutException(IBrowser context, Exception lastException)
            {
                throw new NotImplementedException();
            }
        }

        [Fact]
        public void Context_Null_Check()
        {
            Assert.Throws<ArgumentNullException>(() => { new TestWait(null, TimeSpan.MaxValue, TimeSpan.Zero); });
        }

        [Fact]
        public void Constructor_Happy_Path()
        {
            var timeout = TimeSpan.MaxValue;
            var polling = TimeSpan.MinValue;

            var test = new TestWait(new Mock<IBrowser>().Object, timeout, polling);

            test.Timeout.Should().Be(timeout);
            test.PollingInterval.Should().Be(polling);
        }

        [Fact]
        public void IgnoreExceptionTypes_Null_Check()
        {
            var test = new TestWait(new Mock<IBrowser>().Object, TimeSpan.MaxValue, TimeSpan.MaxValue);

            Assert.Throws<ArgumentNullException>(() => test.IgnoreExceptionTypes(null));
        }

        [Fact]
        public void IgnoreExceptionTypes_Must_Be_Exception()
        {
            var test = new TestWait(new Mock<IBrowser>().Object, TimeSpan.MaxValue, TimeSpan.MaxValue);

            Assert.Throws<ArgumentException>(() => test.IgnoreExceptionTypes(new object().GetType()));
        }

        [Fact]
        public void IgnoreExceptionTypes_Happy_Path()
        {
            var test = new TestWait(new Mock<IBrowser>().Object, TimeSpan.MaxValue, TimeSpan.MaxValue);
            test.IgnoreExceptionTypes(new Exception().GetType());
        }

        [Fact]
        public void Until_Null_Condition()
        {
            var test = new TestWait(new Mock<IBrowser>().Object, TimeSpan.MaxValue, TimeSpan.MaxValue);

            Assert.Throws<ArgumentNullException>(() => test.Until<TestWait>(null));
        }

        [Fact]
        public void Until_With_Bad_Return_Type()
        {
            var test = new TestWait(new Mock<IBrowser>().Object, TimeSpan.MaxValue, TimeSpan.MaxValue);

            Assert.Throws<ArgumentException>(() => test.Until<int>(x => 7));
        }

        [Fact]
        public void Until_As_Boolean()
        {
            var test = new TestWait(new Mock<IBrowser>().Object, TimeSpan.FromMilliseconds(20), TimeSpan.FromMilliseconds(5));

            test.Until(x => true).Should().BeTrue();
        }

        [Fact]
        public void Until_IgnoredException()
        {
            var test = new TestWait(new Mock<IBrowser>().Object, TimeSpan.FromMilliseconds(20), TimeSpan.FromMilliseconds(5));
            test.IgnoreExceptionTypes(new TestLibException("").GetType());

            // NotImplementedException is the TimeoutException for TestWait,
            // if this passes, it means NexusException is being ignored.
            Assert.Throws<NotImplementedException>(() => test.Until<bool>(x => throw new TestLibException("")));
        }

    }
}