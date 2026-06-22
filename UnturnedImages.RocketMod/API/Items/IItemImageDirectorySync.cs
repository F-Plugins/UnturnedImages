using System;

namespace UnturnedImages.API.Items
{
    public interface IItemImageDirectorySync
    {
        string? GetItemImageUrlSync(Guid guid, bool includeWorkshop = true);

        [Obsolete]
        string? GetItemImageUrlSync(ushort id, bool includeWorkshop = true);
    }
}
