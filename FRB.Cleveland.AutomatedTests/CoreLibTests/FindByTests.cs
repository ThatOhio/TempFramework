using FluentAssertions;
using FRB.Cleveland.AutomatedTests.Lib;
using Xunit;

namespace FRB.Cleveland.AutomatedTests.CoreLibTests
{
    public class FindByTests
    {
        [Fact]
        public void FindBy_Correct_Type_Happy_Path()
        {
            var testVal = "Test";

            var className = FindBy.ClassName(testVal);
            className.FindByType.Should().Be(FindByType.ClassName);
            className.Value.Should().Be(testVal);

            var cssSelector = FindBy.CssSelector(testVal);
            cssSelector.FindByType.Should().Be(FindByType.CssSelector);
            cssSelector.Value.Should().Be(testVal);

            var id = FindBy.Id(testVal);
            id.FindByType.Should().Be(FindByType.Id);
            id.Value.Should().Be(testVal);

            var linkText = FindBy.LinkText(testVal);
            linkText.FindByType.Should().Be(FindByType.LinkText);
            linkText.Value.Should().Be(testVal);

            var name = FindBy.Name(testVal);
            name.FindByType.Should().Be(FindByType.Name);
            name.Value.Should().Be(testVal);

            var partialLinkText = FindBy.PartialLinkText(testVal);
            partialLinkText.FindByType.Should().Be(FindByType.PartialLinkText);
            partialLinkText.Value.Should().Be(testVal);

            var tagName = FindBy.TagName(testVal);
            tagName.FindByType.Should().Be(FindByType.TagName);
            tagName.Value.Should().Be(testVal);

            var xPath = FindBy.XPath(testVal);
            xPath.FindByType.Should().Be(FindByType.XPath);
            xPath.Value.Should().Be(testVal);
        }
    }
}