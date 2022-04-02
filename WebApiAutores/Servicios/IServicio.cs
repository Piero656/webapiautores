
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiAutores.Servicios
{
    public interface IServicio
    {
        Guid ObtenerScope();
        Guid ObtenerSinglenton();
        Guid ObtenerTransient();
        void RealizarTarea();
    }


    public class ServicioA : IServicio
    {
        private readonly ILogger<ServicioA> logger;
        private readonly ServicioScoped servicioScoped;
        private readonly ServicioSinglenton servicioSinglenton;
        private readonly ServicioTransient servicioTransient;

        public ServicioA(ILogger<ServicioA> logger, ServicioScoped servicioScoped,
            ServicioSinglenton servicioSinglenton, ServicioTransient servicioTransient)
        {
            this.logger = logger;
            this.servicioScoped = servicioScoped;
            this.servicioSinglenton = servicioSinglenton;
            this.servicioTransient = servicioTransient;
        }


        public Guid ObtenerTransient() { return servicioTransient.Guid; }
        public Guid ObtenerScope() { return servicioScoped.Guid; }
        public Guid ObtenerSinglenton() { return servicioSinglenton.Guid; }


        public void RealizarTarea()
        {
            
        }
    }

    public class ServicioB : IServicio
    {
        public Guid ObtenerScope()
        {
            throw new NotImplementedException();
        }

        public Guid ObtenerSinglenton()
        {
            throw new NotImplementedException();
        }

        public Guid ObtenerTransient()
        {
            throw new NotImplementedException();
        }

        public void RealizarTarea()
        {
            
        }
    }


    public class ServicioTransient
    {
        public Guid Guid = Guid.NewGuid();

    }

    public class ServicioScoped
    {
        public Guid Guid = Guid.NewGuid();

    }
    public class ServicioSinglenton
    {
        public Guid Guid = Guid.NewGuid();

    }

}
