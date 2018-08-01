using AutoMapper;
using WellboreProfileView.Models;
using WellboreProfileView.Models.DataBaseModels;
using WellboreProfileView.ViewModels;

namespace WellboreProfileView.Mappers
{
    public class AutoMapperInitializer : Profile
    {
        private static bool isInitialize;
        public static void Initialize()
        {
            if (!isInitialize)
            {
                Mapper.Initialize(i => { i.AddProfile<AutoMapperInitializer>(); });
                isInitialize = true;
            }
            
        }

        public AutoMapperInitializer()
        {
            CreateMap<AreaGridViewModel, Area>().ForMember(dest => dest.Wells, opt => opt.MapFrom(src => src.ChildItems));

            CreateMap<WellGridViewModel, Well>().ForMember(dest => dest.Wellbores, opt => opt.MapFrom(src => src.ChildItems));

            CreateMap<WellboreGridViewModel, Wellbore>().ForMember(dest => dest.ProfilePaths, opt => opt.MapFrom(src => src.ChildItems));

            CreateMap<ProfilePathGridViewModel, ProfilePath>();

            CreateMap<ProfilePathGridViewModel, ProfilePathPoint>();

            CreateMap<ProfilePathPoint, ProfilePathGridViewModel>();
        }
    }
}