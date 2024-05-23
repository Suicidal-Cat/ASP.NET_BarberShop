﻿using Microsoft.AspNetCore.Mvc.Formatters;
using System.Text;
using System.Text.Json;

namespace BarberShopWeb.Hateoas
{
    public class HateoasJsonOutputFormmater:TextOutputFormatter
    {
        public HateoasJsonOutputFormmater()
        {
            SupportedMediaTypes.Add(Microsoft.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/vnd.barber.hateoas+json"));
            SupportedEncodings.Add(Encoding.UTF8);
            SupportedEncodings.Add(Encoding.Unicode);
        }

        public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
        {
            var json = JsonSerializer.Serialize(context.Object);
            var httpContext = context.HttpContext;

            httpContext.Response.ContentType = "application/vnd.barber.hateoas+json; charset=utf-8";

            await httpContext.Response.WriteAsync(json, selectedEncoding);

        }

        protected override bool CanWriteType(Type? type)
        {
            if (typeof(LinkCollectionWrapper<>) == (type?.BaseType) || typeof(LinkCollectionWrapper<>).Name == type?.Name) return true;

            return false;
        }

    }
}
/*type?.AssemblyQualifiedName != null &&
                type.AssemblyQualifiedName.Contains("LinkCollectionWrapper")) return true;*/
