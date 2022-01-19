using System;
using FRB.Cleveland.AutomatedTests.Lib;
using OpenQA.Selenium;

namespace FRB.Cleveland.AutomatedTests.Selenium
{
    public class SeleniumElement : IElement
    {
        private readonly IWebElement _element;

        public SeleniumElement(IWebElement element) => _element = element;

        public string Id => _element.GetAttribute("id");
        public string Name => _element.GetAttribute("name");
        public string CssClass => _element.GetAttribute("class");
        public string InnerHtml => _element.GetAttribute("innerHTML");
        public string OuterHtml => _element.GetAttribute("outerHTML");
        public bool IsEnabled => _element.Enabled;
        public bool IsVisible => _element.Displayed;
        public string TabIndex => _element.GetAttribute("tabIndex");
        public string Title => _element.GetAttribute("title");
        public string Value => _element.GetAttribute("value");
        public string MaxLength => _element.GetAttribute("maxLength");
        public string HtmlFor => _element.GetAttribute("htmlFor");
        public string Href => _element.GetAttribute("href");
        public bool Checked => Convert.ToBoolean(_element.GetAttribute("checked"));

        public string Text
        {
            get => _element.Text;
            set
            {
                Clear();
                _element.SendKeys(value);
            }
        }

        public void Click() => _element.Click();

        public string GetAttribute(string name) => _element.GetAttribute(name);

        public bool AttributeExists(string name)
        {
            var x = _element.GetAttribute(name);
            return string.IsNullOrWhiteSpace(x) || x.ToLower() == "null";
        }

        public void SendEnter() => _element.SendKeys(Keys.Enter);

        public void SendKeys(string keys) => _element.SendKeys(keys);

        public void Clear() => _element.Clear();
    }
}