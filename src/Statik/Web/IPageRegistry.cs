using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Statik.Web
{
    public interface IPageRegistry
    {
        IReadOnlyCollection<string> GetPaths();

        Page GetPage(string path);

        Page FindOne(PageMatchDelegate match);
        
        Task<Page> FindOne(PageMatchDelegateAsync match);

        List<Page> FindMany(PageMatchDelegate match);
        
        Task<List<Page>> FindMany(PageMatchDelegateAsync match);

        void ForEach(PageActionDelegate action);
        
        Task ForEach(PageActionDelegateAsync action);
    }

    public delegate bool PageMatchDelegate(Page page);

    public delegate Task<bool> PageMatchDelegateAsync(Page page);

    public delegate void PageActionDelegate(Page page);

    public delegate Task PageActionDelegateAsync(Page page);
}