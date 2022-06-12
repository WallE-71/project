using System;
using Microsoft.AspNetCore.Http;

namespace ShoppingStore.Domain.Common
{
    public class LocalStorageManeger
    {
        public Guid GetBrowserId(HttpContext context)
        {
            var browserId = context.Session.GetString("browserId");
            if (browserId == null)
            {
                var value = Guid.NewGuid().ToString();
                Add(context, "browserId", value);
                browserId = value;
            }
            Guid guidBrowser;
            Guid.TryParse(browserId.ToString(), out guidBrowser);
            return guidBrowser;
        }

        public byte[] GetData(HttpContext context, string[] keys, string[] values)
        {
            byte[] data = null;
            foreach (var key in keys)
            {
                data = context.Session.Get(key);
                if (data == null)
                    AddRange(context, keys, values);
            }
            return data;
        }

        public void Add(HttpContext context, string key, string value)
        {
            context.Session.SetString(key, value);
        }

        public void AddRange(HttpContext context, string[] keys, string[] values)
        {
            foreach (var value in values)
                foreach (var key in keys)
                    context.Session.SetString(key, value);
        }

        public void Remove(HttpContext context, string token)
        {
            context.Session.Remove(token);
        }
    }
}
