using System.CodeDom.Compiler;
using System.Threading;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using WinRT;

namespace CyreneAuth;

#if DISABLE_XAML_GENERATED_MAIN

public static class Program
{
    [GeneratedCode("Microsoft.UI.Xaml.Markup.Compiler", " 3.0.0.2411")]
    public static void Main()
    {
        ComWrappersSupport.InitializeComWrappers();
        Application.Start((_) =>
        {
            var context = new DispatcherQueueSynchronizationContext(DispatcherQueue.GetForCurrentThread());
            SynchronizationContext.SetSynchronizationContext(context);
            new App();
        });
    }
}

#endif