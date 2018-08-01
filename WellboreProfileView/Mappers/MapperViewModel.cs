using System.Collections.Generic;
using AutoMapper;
using WellboreProfileView.Models.DataBaseModels;
using WellboreProfileView.ViewModels;

namespace WellboreProfileView.Mappers
{
    public static class MapperViewModel 
    {
        public static RootTreeView GetAreaRootTreeView(List<Area> areasDataModel)
        {
            return new RootTreeView(areasDataModel);
        }

        public static AreasRootGridViewModel GetAreaRootGridViewModel(List<Area> areasDataModel)
        {
            return new AreasRootGridViewModel(areasDataModel);
        }

        public static WellboresRootGridViewModel GetWellboreRootGridViewModel(Well wellDataModel)
        {
            return new WellboresRootGridViewModel(wellDataModel);
        }

        public static IEnumerable<Area> GetRemoveAreas(AreasRootGridViewModel rootGridViewModel)
        {
            return Mapper.Map(rootGridViewModel.MainItems.RemoveItems, new List<Area>());
        }

        public static IEnumerable<Well> GetRemoveWells(AreasRootGridViewModel rootGridViewModel)
        {
            List<Well> wells = new List<Well>();
            foreach (AreaGridViewModel areaGridViewModel in rootGridViewModel.MainItems)
            {
                if (areaGridViewModel.ChildItems.RemoveItems.Count > 0)
                    wells.AddRange(Mapper.Map(areaGridViewModel.ChildItems.RemoveItems, new List<Well>()));
            }
            return wells;
        }

        public static IEnumerable<Wellbore> GetRemoveWellbores(WellboresRootGridViewModel wellboresRootGridViewModel)
        {
            return Mapper.Map(wellboresRootGridViewModel.MainItems.RemoveItems, new List<Wellbore>());
        }

        public static IEnumerable<ProfilePath> GetRemoveProfilePaths(WellboresRootGridViewModel wellboresRootGridViewModel)
        {
            List<ProfilePath> profilePaths = new List<ProfilePath>();
            foreach (WellboreGridViewModel wellboreGridViewModel in wellboresRootGridViewModel.MainItems)
            {
                if (wellboreGridViewModel.ChildItems.RemoveItems.Count > 0)
                    profilePaths.AddRange(Mapper.Map(wellboreGridViewModel.ChildItems.RemoveItems, new List<ProfilePath>()));
            }
            return profilePaths;
        }

        public static IEnumerable<Area> GetUpdateAreas(AreasRootGridViewModel rootGridViewModel)
        {
           return Mapper.Map(rootGridViewModel.MainItems.ChangedItems, new List<Area>());
        }

        public static IEnumerable<Wellbore> GetUpdateWellbores(WellboresRootGridViewModel wellboresRootGridViewModel)
        {
            return Mapper.Map(wellboresRootGridViewModel.MainItems.ChangedItems, new List<Wellbore>());
        }
    }
}