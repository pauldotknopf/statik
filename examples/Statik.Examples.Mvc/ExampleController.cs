using System;
using Microsoft.AspNetCore.Mvc;

namespace Statik.Examples.Mvc
{
    public class ExampleController : Controller
    {
        public ActionResult Index()
        {
            return Content($"The time is {DateTime.Now.ToLongTimeString()}");
        }
    }
}