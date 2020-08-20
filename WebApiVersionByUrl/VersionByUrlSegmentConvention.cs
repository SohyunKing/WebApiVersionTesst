using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Versioning.Conventions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WebApiVersionByUrl
{
    public class VersionByUrlSegmentConvention : IControllerConvention
    {
        public bool Apply(IControllerConventionBuilder controller, ControllerModel controllerModel)
        {
            if (controller == null)
            {
                throw new ArgumentNullException("controller");
            }

            if (controllerModel == null)
            {
                throw new ArgumentNullException("controllerModel");
            }

            
            var test = "^(\\/(v|V))(\\d{1,})\\.(\\d{1,})\\/";
            return true;
        }
    }
}
