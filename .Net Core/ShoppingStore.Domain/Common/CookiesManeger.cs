using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ShoppingStore.Domain.Common
{
    public class CookiesManeger
    {
        public void Add(HttpContext context, string token, string value)
        {
            context.Response.Cookies.Append(token, value, GetCookieOptions(context));
        }

        public bool Contains(HttpContext context, string token)
        {
            return context.Request.Cookies.ContainsKey(token);
        }

        public string GetValue(HttpContext context, string token)
        {
            string cookieValue;
            if (!context.Request.Cookies.TryGetValue(token, out cookieValue))
                return null;
            return cookieValue;
        }

        public void Remove(HttpContext context, string token)
        {
            if (context.Request.Cookies.ContainsKey(token))
                context.Response.Cookies.Delete(token);
        }

        public Guid GetBrowserId(HttpContext context)
        {
            var browserId = GetValue(context, "browserId");
            if (browserId == null)
            {
                var value = Guid.NewGuid().ToString();
                Add(context, "browserId", value);
                browserId = value;
            }
            Guid guidBrowser;
            Guid.TryParse(browserId, out guidBrowser);
            return guidBrowser;
        }

        private CookieOptions GetCookieOptions(HttpContext context)
        {
            return new CookieOptions
            {
                HttpOnly = true,
                Path = context.Request.PathBase.HasValue ? context.Request.PathBase.ToString() : "/",
                Secure = context.Request.IsHttps,
                Expires = DateTime.Now.AddDays(100),
            };
        }
    }
}
