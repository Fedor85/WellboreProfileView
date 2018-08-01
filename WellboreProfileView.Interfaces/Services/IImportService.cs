using System.Collections.Generic;
using WellboreProfileView.Models;

namespace WellboreProfileView.Interfaces.Services
{
    public interface IImportService
    {
        List<ProfilePathPoint> GetProfilePaths(string filePath);
    }
}