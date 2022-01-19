using System;
using FRB.Cleveland.AutomatedTests.Lib;
using OpenQA.Selenium;

namespace FRB.Cleveland.AutomatedTests.Selenium
{
    public static class FindByExtensions
    {
        public static By ToBy(this FindBy findBy)
        {
            switch (findBy.FindByType)
            {
                case FindByType.ClassName:
                    return By.ClassName(findBy.Value);
                case FindByType.CssSelector:
                    return By.CssSelector(findBy.Value);
                case FindByType.Id:
                    return By.Id(findBy.Value);
                case FindByType.LinkText:
                    return By.LinkText(findBy.Value);
                case FindByType.Name:
                    return By.Name(findBy.Value);
                case FindByType.PartialLinkText:
                    return By.PartialLinkText(findBy.Value);
                case FindByType.TagName:
                    return By.TagName(findBy.Value);
                case FindByType.XPath:
                    return By.XPath(findBy.Value);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}