using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;
using UnturnedImages.API.Vehicles;
using UnturnedImages.Configuration.Overrides;
using UnturnedImages.Ranges;
using UnturnedImages.Repositories;
using UnturnedImages.RocketMod.Configuration;

namespace UnturnedImages.RocketMod.Vehicles
{
    public class VehicleImageDirectory : IVehicleImageDirectorySync, IVehicleImageDirectoryAsync
    {
        private static readonly FieldInfo AssetOrigin = typeof(Asset).GetField("origin", BindingFlags.Instance | BindingFlags.NonPublic)!;

        private static readonly List<Color32> DefaultColors = new List<Color32>
        {
            new Color32(53, 53, 53, 1),
            new Color32(55, 101, 140, 1),
            new Color32(46, 100, 46, 1),
            new Color32(189, 110, 39, 1),
            new Color32(106, 70, 109, 1),
            new Color32(154, 37, 37, 1),
            new Color32(212, 212, 212, 1),
            new Color32(205, 170, 30, 1)
        };

        private string? _defaultVehicleRepository;
        private List<RepositoryOverride> _overrideRepositories = new List<RepositoryOverride>();

        public VehicleImageDirectory(UnturnedImagesRocketConfiguration configuration)
        {
            Reload(configuration);
        }

        public void Reload(UnturnedImagesRocketConfiguration configuration)
        {
            _defaultVehicleRepository = configuration.DefaultRepositories?.Vehicles;
            _overrideRepositories = ParseOverrides(configuration.VehicleOverrides);
        }

        public string? GetVehicleImageUrlSync(Guid guid, bool includeWorkshop = true)
        {
            var asset = GetVehicleAsset(guid, out var color);

            return asset == null ? null : GetVehicleImageUrlSync(asset, color, includeWorkshop);
        }

        public string? GetVehicleImageUrlSync(Guid guid, Color32 paintColor, bool includeWorkshop = true)
        {
            var asset = GetVehicleAsset(guid, out _);

            return asset == null ? null : GetVehicleImageUrlSync(asset, paintColor, includeWorkshop);
        }

        [Obsolete]
        public string? GetVehicleImageUrlSync(ushort id, bool includeWorkshop = true)
        {
            var asset = GetVehicleAsset(id, out var color);

            return asset == null ? null : GetVehicleImageUrlSync(asset, color, includeWorkshop);
        }

        public Task<string?> GetVehicleImageUrlAsync(Guid guid, bool includeWorkshop = true)
        {
            return Task.FromResult(GetVehicleImageUrlSync(guid, includeWorkshop));
        }

        public Task<string?> GetVehicleImageUrlAsync(Guid guid, Color32 paintColor, bool includeWorkshop = true)
        {
            return Task.FromResult(GetVehicleImageUrlSync(guid, paintColor, includeWorkshop));
        }

        [Obsolete]
        public Task<string?> GetVehicleImageUrlAsync(ushort id, bool includeWorkshop = true)
        {
            return Task.FromResult(GetVehicleImageUrlSync(id, includeWorkshop));
        }

        private string? GetVehicleImageUrlSync(VehicleAsset asset, Color32? paintColor, bool includeWorkshop)
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
            var repository = @override?.Repository ?? _defaultVehicleRepository;

            if (string.IsNullOrWhiteSpace(repository))
            {
                return null;
            }

            var id = asset.GUID.ToString();

            if (paintColor == null && asset.SupportsPaintColor)
            {
                paintColor = GetRandomPaintColor(asset);
            }

            if (paintColor != null && asset.SupportsPaintColor)
            {
                id += $"-{paintColor.Value.r}-{paintColor.Value.g}-{paintColor.Value.b}";
            }

            return repository!.Replace("{VehicleId}", id);
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

        private static VehicleAsset? GetVehicleAsset(ushort id, out Color32? color)
        {
            var asset = Assets.find(EAssetType.VEHICLE, id);

            if (asset == null)
            {
                color = null;
                return null;
            }

            return GetVehicleAsset(asset.GUID, out color);
        }

        private static VehicleAsset? GetVehicleAsset(Guid guid, out Color32? color)
        {
            var asset = Assets.find(guid);
            color = null;

            if (asset is VehicleAsset vehicleAsset)
            {
                return vehicleAsset;
            }

            if (asset is RedirectorAsset redirectorAsset)
            {
                return Assets.find<VehicleAsset>(redirectorAsset.TargetGuid);
            }

            if (asset is VehicleRedirectorAsset vehicleRedirectorAsset)
            {
                color = vehicleRedirectorAsset.LoadPaintColor ?? vehicleRedirectorAsset.SpawnPaintColor;
                return vehicleRedirectorAsset.TargetVehicle.Find();
            }

            return null;
        }

        private static List<Color32> GetPaintColors(VehicleAsset asset)
        {
            return asset.DefaultPaintColors ?? DefaultColors;
        }

        private static Color32 GetRandomPaintColor(VehicleAsset asset)
        {
            var colors = GetPaintColors(asset);

            return colors[UnityEngine.Random.Range(0, colors.Count)];
        }
    }
}
