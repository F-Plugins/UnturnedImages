using System;
using System.Threading.Tasks;

namespace UnturnedImages.API.Items
{
    public interface IItemImageDirectoryAsync
    {
        Task<string?> GetItemImageUrlAsync(Guid guid, bool includeWorkshop = true);

        [Obsolete]
        Task<string?> GetItemImageUrlAsync(ushort id, bool includeWorkshop = true);
    }
}
