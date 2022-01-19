namespace FRB.Cleveland.AutomatedTests.Lib
{
    public enum FindByType
    {
        ClassName,
        CssSelector,
        Id,
        LinkText,
        Name,
        PartialLinkText,
        TagName,
        XPath
    }

    /// <summary>
    /// Mimics the "By" functionality of the popular Selenium library.
    /// </summary>
    public sealed class FindBy
    {
        public string Value { get; }
        public FindByType FindByType { get; }

        private FindBy(string value, FindByType findByType)
        {
            Value = value;
            FindByType = findByType;
        }

        public static FindBy ClassName(string className) => new FindBy(className, FindByType.ClassName);
        public static FindBy CssSelector(string cssSelector) => new FindBy(cssSelector, FindByType.CssSelector);
        public static FindBy Id(string id) => new FindBy(id, FindByType.Id);
        public static FindBy LinkText(string linkText) => new FindBy(linkText, FindByType.LinkText);
        public static FindBy Name(string name) => new FindBy(name, FindByType.Name);
        public static FindBy PartialLinkText(string partialLinkText) => new FindBy(partialLinkText, FindByType.PartialLinkText);
        public static FindBy TagName(string tagName) => new FindBy(tagName, FindByType.TagName);
        public static FindBy XPath(string xPath) => new FindBy(xPath, FindByType.XPath);
    }
}