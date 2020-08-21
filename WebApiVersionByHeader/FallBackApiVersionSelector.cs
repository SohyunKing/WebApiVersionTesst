using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiVersionByHeader
{
    public class FallBackApiVersionSelector : IApiVersionSelector
    {
        private readonly ApiVersioningOptions options;

        public FallBackApiVersionSelector(ApiVersioningOptions options)
        {
            this.options = options;
        }

        public ApiVersion SelectVersion(HttpRequest request, ApiVersionModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }

            switch (model.ImplementedApiVersions.Count)
            {
                case 0:
                    return options.DefaultApiVersion;
                case 1:
                    {
                        ApiVersion apiVersion = model.ImplementedApiVersions[0];
                        if (apiVersion.Status != null)
                        {
                            return options.DefaultApiVersion;
                        }

                        return apiVersion;
                    }
                default:
                    return ((IEnumerable<ApiVersion>)model.ImplementedApiVersions).Where(((ApiVersion v) => v.Status == null)).Max((Func<ApiVersion, ApiVersion>)((ApiVersion v) => v)) ?? options.DefaultApiVersion;
            }
        }
    }
}
