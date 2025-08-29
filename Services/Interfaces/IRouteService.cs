using System.Threading.Tasks;
using KesifUygulamasiTemplate.Models;

namespace KesifUygulamasiTemplate.Services.Interfaces
{
    /// <summary>
    /// Interface for route generation services
    /// </summary>
    public interface IRouteService
    {
        /// <summary>
        /// Gets route points between two locations
        /// </summary>
        /// <param name="start">Starting location</param>
        /// <param name="end">Ending location</param>
        /// <returns>List of location points that make up the route</returns>
        Task<System.Collections.Generic.List<LocationModel>> GetRouteAsync(LocationModel start, LocationModel end);
    }
}
