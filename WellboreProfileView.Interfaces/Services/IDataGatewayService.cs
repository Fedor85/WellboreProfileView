using System.Collections.Generic;
using WellboreProfileView.Models.DataBaseModels;

namespace WellboreProfileView.Interfaces.Services
{
    public interface IDataGatewayService
    {
        bool ConnectionExists(out string error);

        Area GetArea(long areaId);

        List<Area> GetAllAreaAndWellOrderByName();

        Well GetWellAndWellboreProfilePaths(long wellId);

        void RemoveAndUpdateAreas(IEnumerable<Area> removeAreas, IEnumerable<Area> updateAreas);

        void RemoveWells(IEnumerable<Well> removeWells);

        void RemoveAndUpdateWellbores(IEnumerable<Wellbore> removeWellbores, IEnumerable<Wellbore> updateWellbores);

        void RemoveProfilePaths(IEnumerable<ProfilePath> removeProfilePaths);
    }
}
