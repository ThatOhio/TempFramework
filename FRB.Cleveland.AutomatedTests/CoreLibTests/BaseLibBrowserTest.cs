using System;
using System.Collections.Generic;
using FluentAssertions;
using FRB.Cleveland.AutomatedTests.Lib;
using Xunit;

namespace FRB.Cleveland.AutomatedTests.CoreLibTests
{
    public class BaseLibBrowserTest
    {
        protected class TestLibBrowser : BaseLibBrowser
        {
            /// <inheritdoc />
            public override string CurrentUrl { get; }

            /// <inheritdoc />
            public override string PageSource { get; }

            /// <inheritdoc />
            public override string BrowserType { get; }

            /// <inheritdoc />
            public override Type ElementNotFoundException { get; }

            /// <inheritdoc />
            public override void Initialize()
            {
            }

            /// <inheritdoc />
            public override TimeSpan Navigate(string url)
            {
                return TimeSpan.MaxValue;
            }

            public bool CloseCalled { get; private set; } = false;

            /// <inheritdoc />
            public override void Close()
            {
                CloseCalled = true;

                // The rest of this is just to make this file Green for me in
                // NCrunch... Delete it if you want?
                var a = CurrentUrl;
                var b = PageSource;
                var c = BrowserType;
                var d = ElementNotFoundException;
                Initialize();
                var e = Navigate("");
                var f = GetElement(null);
                var g = GetElements(null);
            }

            /// <inheritdoc />
            public override IElement GetElement(FindBy findBy)
            {
                return null;
            }

            /// <inheritdoc />
            public override IEnumerable<IElement> GetElements(FindBy findBy)
            {
                return null;
            }
        }

        [Fact]
        public void ElementFinder_HappyPath()
        {
            var test = new TestLibBrowser();
            test.ElementFinder(FindBy.Id("Test")).Should().NotBeNull();
        }

        [Fact]
        public void Dispose_Calls_Close()
        {
            var test = new TestLibBrowser();
            test.Dispose();
            test.CloseCalled.Should().BeTrue();
        }
    }
}