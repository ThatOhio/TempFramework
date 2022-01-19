using System;
using System.Collections.Generic;
using FRB.Cleveland.AutomatedTests.Lib.Attributes;

namespace FRB.Cleveland.AutomatedTests.Lib
{
    [Log]
    public interface IBrowser : IDisposable
    {
        string CurrentUrl { get; }
        string PageSource { get; }
        string BrowserType { get; }
        Type ElementNotFoundException { get; }

        void Initialize();

        TimeSpan Navigate(string url);

        void Close();

        [LogIgnore]
        IElement GetElement(FindBy findBy);

        [LogIgnore]
        IEnumerable<IElement> GetElements(FindBy findBy);

        [LogIgnore]
        ElementFinder ElementFinder(FindBy findBy);
    }
}