using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Matching;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiVersionByUrl
{
    public class FallbackApiVersionMatcherPolicy : MatcherPolicy, IEndpointSelectorPolicy
    {
        private readonly IOptions<ApiVersioningOptions> options;

        public override int Order => 0;

        private ApiVersioningOptions Options => options.Value;

        private IApiVersionSelector ApiVersionSelector => Options.ApiVersionSelector;

        private IReportApiVersions ApiVersionReporter
        {
            get;
        }

        private ILogger Logger
        {
            get;
        }

        private List<List<Endpoint>> test = new List<List<Endpoint>>();
        private List<bool> test1 = new List<bool>();

        //
        // 摘要:
        //     Initializes a new instance of the Microsoft.AspNetCore.Mvc.Routing.ApiVersionMatcherPolicy
        //     class.
        //
        // 参数:
        //   options:
        //     The options associated with the action selector.
        //
        //   reportApiVersions:
        //     The object used to report API versions.
        //
        //   loggerFactory:
        //     The factory used to create loggers.
        public FallbackApiVersionMatcherPolicy(IOptions<ApiVersioningOptions> options, IReportApiVersions reportApiVersions, ILoggerFactory loggerFactory)
        {
            this.options = options;
            ApiVersionReporter = reportApiVersions;
            Logger = loggerFactory.CreateLogger(GetType());
        }

        public bool AppliesToEndpoints(IReadOnlyList<Endpoint> endpoints)
        {
            if (endpoints == null)
            {
                throw new ArgumentNullException("endpoints");
            }
            test.Add(endpoints.ToList());
            for (int i = 0; i < endpoints.Count; i++)
            {
                if ((endpoints[i].Metadata?.GetMetadata<ActionDescriptor>())?.GetProperty<ApiVersionModel>() != null)
                {
                    test1.Add(true);
                    return true;
                }
            }
            test1.Add(false);
            return false;
        }

        public Task ApplyAsync(HttpContext httpContext, CandidateSet candidates)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException("httpContext");
            }

            if (candidates == null)
            {
                throw new ArgumentNullException("candidates");
            }

            if (IsRequestedApiVersionAmbiguous(httpContext, out ApiVersion apiVersion))
            {
                return Task.CompletedTask;
            }

            if (apiVersion == null && Options.AssumeDefaultVersionWhenUnspecified)
            {
                apiVersion = TrySelectApiVersion(httpContext, candidates);
                httpContext.Features.Get<IApiVersioningFeature>().RequestedApiVersion = apiVersion;
            }

            IReadOnlyList<(int, ActionDescriptor, bool)> readOnlyList = EvaluateApiVersion(candidates, apiVersion);
            if (readOnlyList.Count == 0)
            {
                ClientError(httpContext, candidates);
                //httpContext.SetEndpoint((Endpoint)ClientError(httpContext, candidates));
            }
            else
            {
                for (int i = 0; i < readOnlyList.Count; i++)
                {
                    (int, ActionDescriptor, bool) valueTuple = readOnlyList[i];
                    int item = valueTuple.Item1;
                    bool item2 = valueTuple.Item3;
                    candidates.SetValidity(item, item2);
                }
            }

            return Task.CompletedTask;
        }

        private static IReadOnlyList<(int Index, ActionDescriptor Action, bool Valid)> EvaluateApiVersion(CandidateSet candidates, ApiVersion apiVersion)
        {
            List<(int, ActionDescriptor, bool)> list = new List<(int, ActionDescriptor, bool)>();
            List<(int, ActionDescriptor, bool)> list2 = new List<(int, ActionDescriptor, bool)>();
            for (int i = 0; i < candidates.Count; i++)
            {
                ActionDescriptor actionDescriptor = candidates[i].Endpoint.Metadata?.GetMetadata<ActionDescriptor>();
                if (actionDescriptor != null)
                {
                    switch (actionDescriptor.MappingTo(apiVersion))
                    {
                        case ApiVersionMapping.Explicit:
                            list.Add((i, actionDescriptor, candidates.IsValidCandidate(i)));
                            break;
                        case ApiVersionMapping.Implicit:
                            list2.Add((i, actionDescriptor, candidates.IsValidCandidate(i)));
                            break;
                    }

                    candidates.SetValidity(i, value: false);
                }
            }

            switch (list.Count)
            {
                case 0:
                    list.AddRange(list2);
                    break;
                case 1:
                    if (list[0].Item2.GetApiVersionModel().IsApiVersionNeutral)
                    {
                        list.AddRange(list2);
                    }

                    break;
            }

            return list.ToArray();
        }

        private bool IsRequestedApiVersionAmbiguous(HttpContext httpContext, out ApiVersion apiVersion)
        {
            try
            {
                apiVersion = httpContext.GetRequestedApiVersion();
            }
            catch (AmbiguousApiVersionException ex)
            {
                Logger.LogInformation(ex.Message);
                apiVersion = null;
                //RequestHandlerContext context = new RequestHandlerContext(Options.ErrorResponses)
                //{
                //    Code = "AmbiguousApiVersion",
                //    Message = ex.Message
                //};
                //httpContext.SetEndpoint((Endpoint)new BadRequestHandler(context));
                return true;
            }

            return false;
        }

        private ApiVersion TrySelectApiVersion(HttpContext httpContext, CandidateSet candidates)
        {
            List<ApiVersionModel> list = new List<ApiVersionModel>();
            for (int i = 0; i < candidates.Count; i++)
            {
                ApiVersionModel apiVersionModel = candidates[i].Endpoint.Metadata?.GetMetadata<ActionDescriptor>()?.GetApiVersionModel();
                if (apiVersionModel != null)
                {
                    list.Add(apiVersionModel);
                }
            }

            return ApiVersionSelector.SelectVersion(httpContext.Request, list.Aggregate());
        }

        private void ClientError(HttpContext httpContext, CandidateSet candidateSet)
        {
            List<ActionDescriptor> list = new List<ActionDescriptor>(candidateSet.Count);
            for (int i = 0; i < candidateSet.Count; i++)
            {
                ActionDescriptor actionDescriptor = candidateSet[i].Endpoint.Metadata?.GetMetadata<ActionDescriptor>();
                if (actionDescriptor != null)
                {
                    list.Add(actionDescriptor);
                }
            }

            //return new ClientErrorBuilder
            //{
            //    Options = Options,
            //    ApiVersionReporter = ApiVersionReporter,
            //    HttpContext = httpContext,
            //    Candidates = list,
            //    Logger = Logger
            //}.Build();
        }
    }
}
