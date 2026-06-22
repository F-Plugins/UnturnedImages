using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UnturnedImages.API.Items;
using UnturnedImages.Configuration.Overrides;
using UnturnedImages.Ranges;
using UnturnedImages.Repositories;
using UnturnedImages.RocketMod.Configuration;

namespace UnturnedImages.RocketMod.Items
{
    public class ItemImageDirectory : IItemImageDirectorySync, IItemImageDirectoryAsync
    {
        private static readonly FieldInfo AssetOrigin = typeof(Asset).GetField("origin", BindingFlags.Instance | BindingFlags.NonPublic)!;

        private string? _defaultItemRepository;
        private List<RepositoryOverride> _overrideRepositories = new List<RepositoryOverride>();

        public ItemImageDirectory(UnturnedImagesRocketConfiguration configuration)
        {
            Reload(configuration);
        }

        public void Reload(UnturnedImagesRocketConfiguration configuration)
        {
            _defaultItemRepository = configuration.DefaultRepositories?.Items;
            _overrideRepositories = ParseOverrides(configuration.ItemOverrides);
        }

        public string? GetItemImageUrlSync(Guid guid, bool includeWorkshop = true)
        {
            var asset = GetItemAsset(guid);

            return asset == null ? null : GetItemImageUrlSync(asset, includeWorkshop);
        }

        [Obsolete]
        public string? GetItemImageUrlSync(ushort id, bool includeWorkshop = true)
        {
            var asset = GetItemAsset(id);

            return asset == null ? null : GetItemImageUrlSync(asset, includeWorkshop);
        }

        public Task<string?> GetItemImageUrlAsync(Guid guid, bool includeWorkshop = true)
        {
            return Task.FromResult(GetItemImageUrlSync(guid, includeWorkshop));
        }

        [Obsolete]
        public Task<string?> GetItemImageUrlAsync(ushort id, bool includeWorkshop = true)
        {
            return Task.FromResult(GetItemImageUrlSync(id, includeWorkshop));
        }

        private string? GetItemImageUrlSync(ItemAsset asset, bool includeWorkshop)
        {
            if (!(AssetOrigin.GetValue(asset) is AssetOrigin origin))
            {
                return null;
            }

            if (!includeWorkshop && origin.workshopFileId != 0)
            {
                return null;
            }

            var @override = _overrideRepositories.FirstOrDefault(x => x.Contains(asset.GUID, asset.id, origin.workshopFileId));
            var repository = @override?.Repository ?? _defaultItemRepository;

            return string.IsNullOrWhiteSpace(repository) ? null : repository!.Replace("{ItemId}", asset.GUID.ToString());
        }

        private static List<RepositoryOverride> ParseOverrides(IEnumerable<OverrideConfig>? overrideConfigs)
        {
            var overrides = new List<RepositoryOverride>();

            foreach (var overrideConfig in overrideConfigs ?? Array.Empty<OverrideConfig>())
            {
                RepositoryOverride? @override = null;
                var repository = overrideConfig.Repository;

                if (string.IsNullOrWhiteSpace(repository))
                {
                    continue;
                }

                if (!string.IsNullOrWhiteSpace(overrideConfig.Id))
                {
                    @override = new RepositoryOverride(RangeHelper.ParseMulti(overrideConfig.Id!), repository);
                }
                else if (!string.IsNullOrWhiteSpace(overrideConfig.Guid) && Guid.TryParse(overrideConfig.Guid, out var guid))
                {
                    @override = new RepositoryOverride(guid, repository);
                }
                else if (!string.IsNullOrWhiteSpace(overrideConfig.WorkshopId) && ulong.TryParse(overrideConfig.WorkshopId, out var workshopId))
                {
                    @override = new RepositoryOverride(workshopId.ToString(), repository);
                }

                if (@override != null)
                {
                    overrides.Add(@override);
                }
            }

            return overrides;
        }

        private static ItemAsset? GetItemAsset(ushort id)
        {
            var asset = Assets.find(EAssetType.ITEM, id);

            return asset == null ? null : GetItemAsset(asset.GUID);
        }

        private static ItemAsset? GetItemAsset(Guid guid)
        {
            var asset = Assets.find(guid);

            if (asset is ItemAsset itemAsset)
            {
                return itemAsset;
            }

            if (asset is RedirectorAsset redirectorAsset)
            {
                return Assets.find<ItemAsset>(redirectorAsset.TargetGuid);
            }

            return null;
        }
    }
}
