using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MaginusLunch.Authentication.IdentityServer.Attributes
{
    public sealed class ResponseSecurityHeadersAttribute : ActionFilterAttribute
    {
        public const string DefaultXContentTypeOptions = "nosniff";
        public const string DefaultXFrameOptions = "SAMEORIGIN";
        public const string DefaultContentSecurityPolicy = "default-src 'self'";
        public const string XContentTypeOptionsHeader = "X-Content-Type-Options";
        public const string XFrameOptionsHeader = "X-Frame-Options";
        public const string ContentsSecurityPolicyHeader = "Content-Security-Policy";
        public const string XContentsSecurityPolicyHeader = "X-Content-Security-Policy";

        public string XContentTypeOptions;
        public string XFrameOptions;
        public string ContentSecurityPolicy;

        public ResponseSecurityHeadersAttribute(string xContentTypeOptions = DefaultXContentTypeOptions,
            string xFrameOptions = DefaultXFrameOptions,
            string contentSecurityPolicy = DefaultContentSecurityPolicy)
        {
            XContentTypeOptions = xContentTypeOptions;
            XFrameOptions = xFrameOptions;
            ContentSecurityPolicy = contentSecurityPolicy;
        }

        public override void OnResultExecuting(ResultExecutingContext context)
        {
            var result = context.Result;
            if (result is ViewResult)
            {
                if (!context.HttpContext.Response.Headers.ContainsKey(XContentTypeOptionsHeader))
                {
                    context.HttpContext.Response.Headers.Add(XContentTypeOptionsHeader, XContentTypeOptions);
                }
                if (!context.HttpContext.Response.Headers.ContainsKey(XFrameOptionsHeader))
                {
                    context.HttpContext.Response.Headers.Add(XFrameOptionsHeader, XFrameOptions);
                }
                // once for standards compliant browsers
                if (!context.HttpContext.Response.Headers.ContainsKey(ContentsSecurityPolicyHeader))
                {
                    context.HttpContext.Response.Headers.Add(ContentsSecurityPolicyHeader, ContentSecurityPolicy);
                }
                // and once again for IE
                if (!context.HttpContext.Response.Headers.ContainsKey(XContentsSecurityPolicyHeader))
                {
                    context.HttpContext.Response.Headers.Add(XContentsSecurityPolicyHeader, ContentSecurityPolicy);
                }
            }
        }
    }
}
