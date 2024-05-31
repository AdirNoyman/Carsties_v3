using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;
using Contracts;

namespace AuctionService.RequestHelpers
{
    public class MappingProfiles : Profile
    {

        public MappingProfiles()
        {

            CreateMap<Auction, AuctionDTO>().IncludeMembers(x => x.Item);
            CreateMap<Item, AuctionDTO>();
            CreateMap<CreateAuctionDTO, Auction>().ForMember(destination => destination.Item, obj => obj.MapFrom(source => source));
            CreateMap<CreateAuctionDTO, Item>();
            CreateMap<AuctionDTO, AuctionCreated>();
        }

    }
}