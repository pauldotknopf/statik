using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using Statik.Examples.Pages.Models;
using Statik.Pages;

namespace Statik.Examples.Pages.Controllers
{
    public class PagesController : Controller
    {
        public async Task<ActionResult> Index()
        {
            var treeItem = RouteData.Values["treeItem"] as PageTreeItem<IFileInfo>;
            if(treeItem == null) throw new InvalidOperationException();
            
            string content = null;
            if (!treeItem.Data.IsDirectory)
            {
                using (var stream = treeItem.Data.CreateReadStream())
                using (var streamReader = new StreamReader(stream))
                    content = await streamReader.ReadToEndAsync();
                content = Markdig.Markdown.ToHtml(content);
            }

            var model = new PageModel(treeItem, content);
            
            return View(model);
        }
    }
}