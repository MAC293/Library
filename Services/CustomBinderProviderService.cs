using Library.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Library.Services
{
    public class CustomBinderProviderService : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (context.Metadata.ModelType == typeof(Book))
            {
                return new BinderTypeModelBinder(typeof(CustomBinderService));
            }

            return null;
        }
    }
}
