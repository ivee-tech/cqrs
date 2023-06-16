using AutoMapper;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using m = Cqrs.Domain.Models;
using e = Cqrs.Infrastructure.Repositories.Entities;
using NSwag.Generation.Processors;
using System;
using Cqrs.Infrastructure.Repositories.Entities;

namespace Cqrs.Infrastructure.Configurations
{
    public class AutoMapperConfig : Profile
    {
        public AutoMapperConfig()
        {

            CreateMap<e.Address, m.Address>()
                .ReverseMap();
            CreateMap<e.Client, m.Client>()
                .ReverseMap();

        }

    }
}