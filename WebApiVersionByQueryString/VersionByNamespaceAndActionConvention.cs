using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Versioning.Conventions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace WebApiVersionByQueryString
{
    public class VersionByNamespaceAndActionConvention : IControllerConvention
    {
        //
        // 摘要:
        //     Applies a controller convention given the specified builder and model.
        //
        // 参数:
        //   controller:
        //     The builder used to apply conventions.
        //
        //   controllerModel:
        //     The model to build conventions from.
        //
        // 返回结果:
        //     True if any conventions were applied to the controllerModel; otherwise, false.
        public virtual bool Apply(IControllerConventionBuilder controller, ControllerModel controllerModel)
        {
            if (controller == null)
            {
                throw new ArgumentNullException("controller");
            }

            if (controllerModel == null)
            {
                throw new ArgumentNullException("controllerModel");
            }

            var @namespace = RenameNamespace(controllerModel.ControllerType.Namespace);
            var namespaceInferred = ApiVersion.TryParse(GetRawApiVersion(@namespace), out ApiVersion version);
            var apiVersions = new List<ApiVersion>();
            foreach (var action in controllerModel.Actions)
            {
                var atts = action.Attributes.OfType<MapToApiVersionAttribute>();
                if (atts.Any())
                {
                    var attList = atts.Cast<MapToApiVersionAttribute>().ToList();
                    //var test = attList.SelectMany(attr=>attr.Versions).Distinct();
                    foreach (var att in attList)
                    {
                        foreach (var apiVersion in att.Versions)
                        {
                            if (!apiVersions.Contains(apiVersion) && apiVersion != version)
                            {
                                apiVersions.Add(apiVersion);
                                controller.HasApiVersion(apiVersion);
                            }
                        }
                    }
                }
            }

            if (!namespaceInferred && !apiVersions.Any())
            {
                return false;
            }

            if (controllerModel.Attributes.OfType<ObsoleteAttribute>().Any())
            {
                controller.HasDeprecatedApiVersion(version);
            }
            else
            {
                controller.HasApiVersion(version);
            }
            

            return true;
        }

        /// <summary>
        /// Rename namespace, eg:Contrller.V1->Controller.V1_0
        /// </summary>
        /// <param name="namespace"></param>
        /// <returns></returns>
        private static string RenameNamespace(string @namespace)
        {
            if (Regex.IsMatch(@namespace, "^\\.(v|V)+(\\d{1,})$", RegexOptions.Singleline))
                @namespace += "_0";
            return @namespace;
        }

        private static string GetRawApiVersion(string @namespace)
        {
            Match match = Regex.Match(@namespace, "[^\\.]?[vV](\\d{4})?_?(\\d{2})?_?(\\d{2})?_?(\\d+)?_?(\\d*)_?([a-zA-Z][a-zA-Z0-9]*)?[\\.$]?", RegexOptions.Singleline);
            List<string> list = new List<string>();
            StringBuilder stringBuilder = new StringBuilder();
            while (match.Success)
            {
                ExtractDateParts(match, stringBuilder);
                ExtractNumericParts(match, stringBuilder);
                ExtractStatusPart(match, stringBuilder);
                if (stringBuilder.Length > 0)
                {
                    list.Add(stringBuilder.ToString());
                }

                stringBuilder.Clear();
                match = match.NextMatch();
            }

            return list.Count switch
            {
                0 => null,
                1 => list[0],
                _ => throw new InvalidOperationException($"Multiple apiVersions inferred from namespaces {@namespace}."),
            };
        }

        private static void ExtractDateParts(Match match, StringBuilder text)
        {
            Group group = match.Groups[1];
            Group group2 = match.Groups[2];
            Group group3 = match.Groups[3];
            if (group.Success && group2.Success && group3.Success)
            {
                text.Append(group.Value);
                text.Append('-');
                text.Append(group2.Value);
                text.Append('-');
                text.Append(group3.Value);
            }
        }

        private static void ExtractNumericParts(Match match, StringBuilder text)
        {
            Group group = match.Groups[4];
            if (!group.Success)
            {
                return;
            }

            if (text.Length > 0)
            {
                text.Append('.');
            }

            text.Append(group.Value);
            Group group2 = match.Groups[5];
            if (group2.Success)
            {
                text.Append('.');
                if (group2.Length > 0)
                {
                    text.Append(group2.Value);
                }
                else
                {
                    text.Append('0');
                }
            }
        }

        private static void ExtractStatusPart(Match match, StringBuilder text)
        {
            Group group = match.Groups[6];
            if (group.Success && text.Length > 0)
            {
                text.Append('-');
                text.Append(group.Value);
            }
        }
    }
}
