using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Library.Services
{
    public class CustomBinderService : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            throw new NotImplementedException();
        }
    }
}
