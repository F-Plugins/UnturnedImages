using System;
using UnityEngine;

namespace UnturnedImages.API.Vehicles
{
    public interface IVehicleImageDirectorySync
    {
        string? GetVehicleImageUrlSync(Guid guid, bool includeWorkshop = true);

        string? GetVehicleImageUrlSync(Guid guid, Color32 paintColor, bool includeWorkshop = true);

        [Obsolete]
        string? GetVehicleImageUrlSync(ushort id, bool includeWorkshop = true);
    }
}
