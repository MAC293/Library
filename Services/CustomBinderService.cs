using Library.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Serilog;
using System.Text.Json;

namespace Library.Services
{
    public class CustomBinderService : IModelBinder
    {
        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext != null)
            {
                var form = bindingContext.HttpContext.Request.Form;
                Log.Information("form: {@form}", form);

                var bookJSON = form["newBook"];
                Log.Information("bookJSON: {@bookJSON}", bookJSON);

                var book = JsonSerializer.Deserialize<BookBorrowService>(bookJSON);
                Log.Information("book: {@book}", book);

                var file = form.Files.GetFile("newCover");
                Log.Information("file: {@file}", file);

                if (file != null)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await file.CopyToAsync(memoryStream);
                        book.Cover = memoryStream.ToArray();
                    }
                }

                bindingContext.Result = ModelBindingResult.Success(book);
            }
        }
    }
}
