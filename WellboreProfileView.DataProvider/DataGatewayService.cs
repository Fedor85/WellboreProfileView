using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using WellboreProfileView.DataProvider.Interface;
using WellboreProfileView.Interfaces.Services;
using WellboreProfileView.Models.DataBaseModels;
using WellboreProfileView.Models.Interfaces;

namespace WellboreProfileView.DataProvider
{
    public class DataGatewayService : IDataGatewayService
    {
        private string connectionString;

        public DataGatewayService(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public bool ConnectionExists(out string error)
        {
            bool isExists = false;
            error = String.Empty;
            using (IDatabaseContext db = new DatabaseContext(connectionString))
            {
                try
                {
                    db.GetDatabase().OpenConnection();
                    db.GetDatabase().CloseConnection();
                    isExists = true;
                }
                catch (Exception e)
                {
                    error = e.Message;
                }
            }
            return isExists;
        }

        public Area GetArea(long areaId)
        {
            using (IDatabaseContext db = new DatabaseContext(connectionString))
            {
                IRepository<Area> repositoryArea = new Repository<Area>(db);
                return repositoryArea.GetAllAsNoTrackingQueryable().Where(area => area.Id == areaId).Include(area => area.Wells).OrderBy(area => area.Name).FirstOrDefault();
            }
        }

        public List<Area> GetAllAreaAndWellOrderByName()
        {
            List<Area> areas = new List<Area>();
            using (IDatabaseContext db = new DatabaseContext(connectionString))
            {
                IRepository<Area> repositoryArea = new Repository<Area>(db);
                areas = repositoryArea.GetAllAsNoTrackingQueryable().Include(area => area.Wells).OrderBy(area => area.Name).ToList();

            }
            foreach (Area area in areas)
                area.Wells = area.Wells.OrderBy(well => well.Name).ToList();

            return areas;
        }

        public Well GetWellAndWellboreProfilePaths(long wellId)
        {
            Well neededWell;
            using (IDatabaseContext db = new DatabaseContext(connectionString))
            {
                IRepository<Well> repositoryArea = new Repository<Well>(db);
                neededWell = repositoryArea.GetAllAsNoTrackingQueryable().Where(well => well.Id == wellId).
                                                    Include(well => well.Wellbores).
                                                    ThenInclude(wellbore => wellbore.ProfilePaths).FirstOrDefault();
            }

            if (neededWell != null)
            {
                neededWell.Wellbores = neededWell.Wellbores.OrderBy(wellbore => wellbore.Name).ToList();
                foreach (Wellbore wellbore in neededWell.Wellbores)
                    wellbore.ProfilePaths = wellbore.ProfilePaths.OrderBy(profilePath => profilePath.VerticalDepth).ToList();
            }

            return neededWell;
        }

        public void RemoveAndUpdateAreas(IEnumerable<Area> removeAreas, IEnumerable<Area> updateAreas)
        {
            IEnumerable<Area> neededRemovAreas = removeAreas.Where(IsNotRemovie);
            if (neededRemovAreas.Count() == 0 && updateAreas.Count() == 0)
                return;

            using (IDatabaseContext db = new DatabaseContext(connectionString))
            {
                IRepository<Area> repositoryArea = new Repository<Area>(db);
                repositoryArea.RemoveRange(neededRemovAreas);
                repositoryArea.UpdateRange(updateAreas);
                db.SaveChanges();
            }
        }

        public void RemoveWells(IEnumerable<Well> removeWells)
        {
            IEnumerable<Well> neededRemovWells = removeWells.Where(IsNotRemovie);
            if (neededRemovWells.Count() == 0)
                return;

            using (IDatabaseContext db = new DatabaseContext(connectionString))
            {
                IRepository<Well> repositoryArea = new Repository<Well>(db);
                repositoryArea.RemoveRange(neededRemovWells);
                db.SaveChanges();
            }
        }

        public void RemoveAndUpdateWellbores(IEnumerable<Wellbore> removeWellbores, IEnumerable<Wellbore> updateWellbores)
        {
            IEnumerable<Wellbore> neededRemovWellbores = removeWellbores.Where(IsNotRemovie);
            if (neededRemovWellbores.Count() == 0 && updateWellbores.Count() == 0)
                return;

            using (IDatabaseContext db = new DatabaseContext(connectionString))
            {
                IRepository<Wellbore> repositoryWellbore = new Repository<Wellbore>(db);
                repositoryWellbore.RemoveRange(neededRemovWellbores);
                repositoryWellbore.UpdateRange(updateWellbores);
                db.SaveChanges();
            }
        }

        public void RemoveProfilePaths(IEnumerable<ProfilePath> removeProfilePaths)
        {
            IEnumerable<ProfilePath> neededRemoveProfilePaths = removeProfilePaths.Where(IsNotRemovie);
            if (neededRemoveProfilePaths.Count() == 0)
                return;

            using (IDatabaseContext db = new DatabaseContext(connectionString))
            {
                IRepository<ProfilePath> repositoryArea = new Repository<ProfilePath>(db);
                repositoryArea.RemoveRange(neededRemoveProfilePaths);
                db.SaveChanges();
            }
        }


        private bool IsNotRemovie(IIdEntity entity)
        {
            return entity.Id != 0;
        }
    }
}
