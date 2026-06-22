using System;
using System.Threading.Tasks;
using UnityEngine;

namespace UnturnedImages.API.Vehicles
{
    public interface IVehicleImageDirectoryAsync
    {
        Task<string?> GetVehicleImageUrlAsync(Guid guid, bool includeWorkshop = true);

        Task<string?> GetVehicleImageUrlAsync(Guid guid, Color32 paintColor, bool includeWorkshop = true);

        [Obsolete]
        Task<string?> GetVehicleImageUrlAsync(ushort id, bool includeWorkshop = true);
    }
}
