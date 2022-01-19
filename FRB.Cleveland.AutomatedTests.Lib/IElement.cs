using FRB.Cleveland.AutomatedTests.Lib.Attributes;

namespace FRB.Cleveland.AutomatedTests.Lib
{
    [Log]
    public interface IElement
    {
        string Id { get; }
        string Name { get; }
        string CssClass { get; }
        string InnerHtml { get; }
        string OuterHtml { get; }
        bool IsEnabled { get; }
        bool IsVisible { get; }
        string TabIndex { get; }
        string Title { get; }
        string Value { get; }
        string MaxLength { get; }
        string HtmlFor { get; }
        string Href { get; }
        bool Checked { get; }

        string Text { get; set; }

        void Click();

        string GetAttribute(string name);

        bool AttributeExists(string name);

        void SendEnter();

        void SendKeys(string keys);

        void Clear();
    }
}