using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Statik.Mvc
{
    public class FromRouteDataAttribute : Attribute, IBinderTypeProviderMetadata, IBindingSourceMetadata
    {
        public FromRouteDataAttribute()
        {
            BinderType = typeof(Binder);
        }
        
        public BindingSource BindingSource => BindingSource.Custom;
        
        public Type BinderType { get; }

        private class Binder : IModelBinder
        {
            public Task BindModelAsync(ModelBindingContext bindingContext)
            {
                var routeData = bindingContext.ActionContext.RouteData.Values;
                bindingContext.Result = ModelBindingResult.Success(routeData[bindingContext.FieldName]);
                return Task.CompletedTask;
            }
        }
    }
}