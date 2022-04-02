using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiAutores.Utilidades
{
    public class SwaggerAgrupaPorVersion : IControllerModelConvention
    {
        public void Apply(ControllerModel controller)
        {
            //primera sacamos el namespace
            var namespaceControlador = controller.ControllerType.Namespace; //Controllers.V1

            var versionApi = namespaceControlador.Split(".").Last().ToLower(); // v1

            controller.ApiExplorer.GroupName = versionApi;

        }
    }
}
