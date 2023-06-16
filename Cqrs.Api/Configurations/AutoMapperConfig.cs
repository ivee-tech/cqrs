using AutoMapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using Cqrs.Api.Models;
using Cqrs.Domain.Comparers;
using Cqrs.Domain.Models;
using m = Cqrs.Domain.Models;

namespace Cqrs.Api.Configurations
{
    public class AutoMapperConfig : Profile
    {
        public AutoMapperConfig()
        {

            CreateMap<Address, AddressDto>()
                .ForMember(d => d.Postcode, opt => opt.MapFrom(s => s.Code))
                .ReverseMap()
                .ForMember(d => d.Code, opt => opt.MapFrom(s => s.Postcode))
                ;
            CreateMap<Client, ClientDto>()
                .ReverseMap();
            CreateMap<Client, ClientListItemDto>();
            CreateMap<Client, ClientFormDto>()
                .ReverseMap();

        }
    }
}
