using Library.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Serilog;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace Library.Services
{
    public class CustomBinderService : IModelBinder
    {
        //public async Task BindModelAsync(ModelBindingContext bindingContext)
        //{
        //    if (bindingContext == null) return;

        //    var form = bindingContext.HttpContext.Request.Form;

        //    var bookJSON = form["newBook"];

        //    var options = new JsonSerializerOptions
        //    {
        //        PropertyNameCaseInsensitive = true
        //    };

        //    var book = System.Text.Json.JsonSerializer.Deserialize<Book>(bookJSON, options);

        //    var file = form.Files.GetFile("newCover");

        //    if (file != null)
        //    {
        //        using (var memoryStream = new MemoryStream())
        //        {
        //            await file.CopyToAsync(memoryStream);
        //            book.Cover = memoryStream.ToArray();
        //        }
        //    }

        //    bindingContext.Result = ModelBindingResult.Success(book);
        //}

        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null) return;

            var form = bindingContext.HttpContext.Request.Form;
            var bookJSON = form["newBook"];

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var book = System.Text.Json.JsonSerializer.Deserialize<BookCoverService>(bookJSON, options);
            var file = form.Files.GetFile("newCover");

            if (file != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await file.CopyToAsync(memoryStream);
                    book.Cover = memoryStream.ToArray();
                    book.CoverFileName = file.FileName; 
                }
            }

            bindingContext.Result = ModelBindingResult.Success(book);
        }
    }

}
