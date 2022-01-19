using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using FRB.Cleveland.AutomatedTests.Lib;
using FRB.Cleveland.AutomatedTests.Lib.Exceptions;
using Moq;
using Xunit;

namespace FRB.Cleveland.AutomatedTests.CoreLibTests
{
    public class ElementFinderTests
    {
        [Fact]
        public void FindBy_Cannot_Be_Null()
        {
            Assert.Throws<ArgumentNullException>(() => { new ElementFinder(null, new Mock<IBrowser>().Object); });
        }

        [Fact]
        public void IBrowser_Cannot_Be_Null()
        {
            Assert.Throws<ArgumentNullException>(() => { new ElementFinder(FindBy.Id("Test"), null); });
        }

        [Fact]
        public void Default_Values()
        {
            var findBy = FindBy.Id("Test");
            var browser = new Mock<IBrowser>().Object;

            var test = new ElementFinder(findBy, browser);

            test.FindBy.Should().Be(findBy);
            test.DoWait.Should().BeTrue();
            test.CheckVisibility.Should().BeTrue();
            test.CheckEnabled.Should().BeTrue();
            test.Timeout.Should().Be(TimeSpan.FromSeconds(EnvironmentVariables.DefaultTimeoutInSeconds));
            test.IgnoredExceptions.Count.Should().Be(0);
            test.Conditions.Count.Should().Be(0);
        }

        [Fact]
        public void SkipWait_Sets_DoWait()
        {
            var findBy = FindBy.Id("Test");
            var browser = new Mock<IBrowser>().Object;

            var test = new ElementFinder(findBy, browser);
            test.SkipWait();

            test.DoWait.Should().BeFalse();
        }

        [Fact]
        public void SkipVisibility_Sets_CheckVisibility()
        {
            var findBy = FindBy.Id("Test");
            var browser = new Mock<IBrowser>().Object;

            var test = new ElementFinder(findBy, browser);
            test.SkipVisibility();

            test.CheckVisibility.Should().BeFalse();
        }

        [Fact]
        public void SkipEnabledCheck_Sets_CheckEnabled()
        {
            var findBy = FindBy.Id("Test");
            var browser = new Mock<IBrowser>().Object;

            var test = new ElementFinder(findBy, browser);
            test.SkipEnableCheck();

            test.CheckEnabled.Should().BeFalse();
        }

        [Fact]
        public void SetTime_Sets_Timeout()
        {
            var findBy = FindBy.Id("Test");
            var browser = new Mock<IBrowser>().Object;

            var test = new ElementFinder(findBy, browser);
            test.SetTimeout(TimeSpan.MaxValue);

            test.Timeout.Should().Be(TimeSpan.MaxValue);
        }

        [Fact]
        public void IgnoreException_WithObject_Throws()
        {
            var findBy = FindBy.Id("Test");
            var browser = new Mock<IBrowser>().Object;

            var test = new ElementFinder(findBy, browser);
            Assert.Throws<ArgumentException>(() => test.IgnoreException(new object().GetType()));
        }

        [Fact]
        public void IgnoreExceptions_Adds_To_Collection_Only_Once()
        {
            var findBy = FindBy.Id("Test");
            var browser = new Mock<IBrowser>().Object;
            var exceptionToAdd = new TestLibException("Testing").GetType();

            var test = new ElementFinder(findBy, browser);
            test.IgnoreException(exceptionToAdd);

            test.IgnoredExceptions.Count.Should().Be(1);
            test.IgnoredExceptions.Should().Contain(exceptionToAdd);

            test.IgnoreException(exceptionToAdd);
            test.IgnoredExceptions.Count.Should().Be(1);
        }


        [Fact]
        public void WithCondition_Null_Check()
        {
            var findBy = FindBy.Id("Test");
            var browser = new Mock<IBrowser>().Object;

            var test = new ElementFinder(findBy, browser);
            Assert.Throws<ArgumentNullException>(() => test.WithCondition(null));
        }

        [Fact]
        public void WithCondition_Adds_To_Collection()
        {
            var findBy = FindBy.Id("Test");
            var browser = new Mock<IBrowser>().Object;

            var test = new ElementFinder(findBy, browser);
            test.WithCondition(x => true);

            test.Conditions.Count.Should().Be(1);
        }

        [Fact]
        public void ExpectCount_Range()
        {
            var findBy = FindBy.Id("Test");
            var browser = new Mock<IBrowser>().Object;

            var test = new ElementFinder(findBy, browser);

            // 0 and any negative number are invalid, with the exception of -1.
            Assert.Throws<ArgumentOutOfRangeException>(() => test.ExpectCount(0));
            Assert.Throws<ArgumentOutOfRangeException>(() => test.ExpectCount(-2));
            test.ExpectCount(-1);
            test.ExpectCount(100);
        }

        [Fact]
        public void IsElementValid_Null_Is_False()
        {
            var findBy = FindBy.Id("Test");
            var browser = new Mock<IBrowser>().Object;

            var test = new ElementFinder(findBy, browser);
            test.IsElementValid(null).Should().BeFalse();
        }

        [Fact]
        public void IsElementValid_HappyPath()
        {
            var findBy = FindBy.Id("Test");
            var browser = new Mock<IBrowser>().Object;
            var elementMock = new Mock<IElement>();
            elementMock.Setup(x => x.IsEnabled).Returns(true);
            elementMock.Setup(x => x.IsVisible).Returns(true);
            elementMock.Setup(x => x.Checked).Returns(true);

            var test = new ElementFinder(findBy, browser);
            test.WithCondition(x => x.Checked);

            test.IsElementValid(elementMock.Object).Should().BeTrue();
        }

        [Fact]
        public void IsElementValid_When_Not_Enabled()
        {
            var findBy = FindBy.Id("Test");
            var browser = new Mock<IBrowser>().Object;
            var elementMock = new Mock<IElement>();
            elementMock.Setup(x => x.IsEnabled).Returns(false);
            elementMock.Setup(x => x.IsVisible).Returns(true);
            elementMock.Setup(x => x.Checked).Returns(true);

            var test = new ElementFinder(findBy, browser);
            test.WithCondition(x => x.Checked);

            test.IsElementValid(elementMock.Object).Should().BeFalse();
        }

        [Fact]
        public void IsElementValid_When_Not_Visibile()
        {
            var findBy = FindBy.Id("Test");
            var browser = new Mock<IBrowser>().Object;
            var elementMock = new Mock<IElement>();
            elementMock.Setup(x => x.IsEnabled).Returns(true);
            elementMock.Setup(x => x.IsVisible).Returns(false);
            elementMock.Setup(x => x.Checked).Returns(true);

            var test = new ElementFinder(findBy, browser);
            test.WithCondition(x => x.Checked);

            test.IsElementValid(elementMock.Object).Should().BeFalse();
        }

        [Fact]
        public void IsElementValid_When_Condition_Not_True()
        {
            var findBy = FindBy.Id("Test");
            var browser = new Mock<IBrowser>().Object;
            var elementMock = new Mock<IElement>();
            elementMock.Setup(x => x.IsEnabled).Returns(true);
            elementMock.Setup(x => x.IsVisible).Returns(true);
            elementMock.Setup(x => x.Checked).Returns(false);

            var test = new ElementFinder(findBy, browser);
            test.WithCondition(x => x.Checked);

            test.IsElementValid(elementMock.Object).Should().BeFalse();
        }

        [Fact]
        public void GetElement_HappyPath()
        {
            var findBy = FindBy.Id("Test");
            var browserMock = new Mock<IBrowser>();
            browserMock.Setup(x => x.ElementNotFoundException).Returns(new TestLibException("").GetType);
            var elementMock = new Mock<IElement>();
            elementMock.Setup(x => x.IsEnabled).Returns(true);
            elementMock.Setup(x => x.IsVisible).Returns(true);
            browserMock.Setup(x => x.GetElements(It.IsAny<FindBy>())).Returns(new List<IElement>() { elementMock.Object });

            var test = new ElementFinder(findBy, browserMock.Object);
            test.SetTimeout(TimeSpan.MinValue);

            test.GetElement().Should().Be(elementMock.Object);
        }

        [Fact]
        public void GetElement_Propagates_Exceptions()
        {
            var findBy = FindBy.Id("Test");
            var browserMock = new Mock<IBrowser>();
            browserMock.Setup(x => x.ElementNotFoundException).Returns(new TestLibException("").GetType);
            browserMock.Setup(x => x.GetElements(It.IsAny<FindBy>())).Throws<ArgumentException>();

            var test = new ElementFinder(findBy, browserMock.Object);

            Assert.Throws<ArgumentException>(() => test.GetElement());
        }

        [Fact]
        public void GetElement_ThrowsTimeout()
        {
            var findBy = FindBy.Id("Test");
            var browserMock = new Mock<IBrowser>();
            browserMock.Setup(x => x.ElementNotFoundException).Returns(new TestLibException("").GetType);

            var test = new ElementFinder(findBy, browserMock.Object);
            test.SetTimeout(TimeSpan.Zero);

            Assert.Throws<ElementFinderTimeoutException>(() => test.GetElement());
        }

        [Fact]
        public void GetElements_Propagates_Exceptions()
        {
            var findBy = FindBy.Id("Test");
            var browserMock = new Mock<IBrowser>();
            browserMock.Setup(x => x.ElementNotFoundException).Returns(new TestLibException("").GetType);
            browserMock.Setup(x => x.GetElements(It.IsAny<FindBy>())).Throws<ArgumentException>();

            var test = new ElementFinder(findBy, browserMock.Object);

            Assert.Throws<ArgumentException>(() => test.GetElements());
        }

        [Fact]
        public void GetElements_ExpectedCount()
        {
            var findBy = FindBy.Id("Test");
            var browserMock = new Mock<IBrowser>();
            browserMock.Setup(x => x.ElementNotFoundException).Returns(new TestLibException("").GetType);
            var elementMock1 = new Mock<IElement>();
            elementMock1.Setup(x => x.IsEnabled).Returns(true);
            elementMock1.Setup(x => x.IsVisible).Returns(true);
            var elementMock2 = new Mock<IElement>();
            elementMock2.Setup(x => x.IsEnabled).Returns(true);
            elementMock2.Setup(x => x.IsVisible).Returns(true);
            var elementMock3Invalid = new Mock<IElement>();
            elementMock3Invalid.Setup(x => x.IsEnabled).Returns(false);
            elementMock3Invalid.Setup(x => x.IsVisible).Returns(true);
            browserMock.Setup(x => x.GetElements(It.IsAny<FindBy>())).Returns(new List<IElement>()
                { elementMock1.Object, elementMock2.Object, elementMock3Invalid.Object });

            var test = new ElementFinder(findBy, browserMock.Object);
            test.SetTimeout(TimeSpan.MinValue);

            var eles = test.GetElements();
            eles.Count().Should().Be(2);
            eles.Should().Contain(elementMock1.Object);
            eles.Should().Contain(elementMock2.Object);
            eles.Should().NotContain(elementMock3Invalid.Object);
        }

        [Fact]
        public void GetElements_ExpectedCount_WrongCount()
        {
            var findBy = FindBy.Id("Test");
            var browserMock = new Mock<IBrowser>();
            browserMock.Setup(x => x.ElementNotFoundException).Returns(new TestLibException("").GetType);
            var elementMock1 = new Mock<IElement>();
            elementMock1.Setup(x => x.IsEnabled).Returns(true);
            elementMock1.Setup(x => x.IsVisible).Returns(true);
            var elementMock2 = new Mock<IElement>();
            elementMock2.Setup(x => x.IsEnabled).Returns(true);
            elementMock2.Setup(x => x.IsVisible).Returns(true);
            var elementMock3Invalid = new Mock<IElement>();
            elementMock3Invalid.Setup(x => x.IsEnabled).Returns(false);
            elementMock3Invalid.Setup(x => x.IsVisible).Returns(true);
            browserMock.Setup(x => x.GetElements(It.IsAny<FindBy>())).Returns(new List<IElement>()
                { elementMock1.Object, elementMock2.Object, elementMock3Invalid.Object });

            var test = new ElementFinder(findBy, browserMock.Object);
            test.SetTimeout(TimeSpan.MinValue);
            test.ExpectCount(3);

            Assert.Throws<ElementFinderTimeoutException>(() => test.GetElements());
        }

    }
}