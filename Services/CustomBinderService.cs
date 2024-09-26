using Library.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Library.Services
{
    public class CustomBinderService : IModelBinder
    {
        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext != null)
            {
                var form = bindingContext.HttpContext.Request.Form;

                //Bind the Book object
                var bookJSON = form["newBook"];
                var book = System.Text.Json.JsonSerializer.Deserialize<Book>(bookJSON);

                //Bind the IFormFile
                var file = form.Files.GetFile("newCover");

                using (var memoryStream = new MemoryStream())
                {
                    await file.CopyToAsync(memoryStream);
                    book.Cover = memoryStream.ToArray();
                }

                bindingContext.Result = ModelBindingResult.Success(book);
            }
        }
    }
}
 