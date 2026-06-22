# Unturned Images RocketMod

A RocketMod port of the Unturned Images plugin. It exposes item and vehicle image URL APIs for other RocketMod plugins.

## Installation

Use `UnturnedImages.RocketMod.zip` from the GitHub release and place `UnturnedImages.RocketMod.dll` in your Rocket plugins folder.

The project also builds a NuGet package for developers:

```powershell
dotnet add package Feli.UnturnedImages.RocketMod
```

## Developer API

Reference `UnturnedImages.RocketMod.dll` or the NuGet package from your RocketMod plugin. At runtime, access the loaded plugin through its static API:

```csharp
using System;
using UnturnedImages.API.Items;
using UnturnedImages.API.Vehicles;
using UnturnedImages.RocketMod;

IItemImageDirectorySync? itemImages = UnturnedImagesRocketPlugin.ItemImages;
IVehicleImageDirectorySync? vehicleImages = UnturnedImagesRocketPlugin.VehicleImages;

string? itemUrl = itemImages?.GetItemImageUrlSync(itemGuid);
string? vehicleUrl = vehicleImages?.GetVehicleImageUrlSync(vehicleGuid);
```

Async wrappers are available through:

- `UnturnedImagesRocketPlugin.ItemImagesAsync`
- `UnturnedImagesRocketPlugin.VehicleImagesAsync`

The plugin logs its assembly version when it loads.

## Configuration

RocketMod generates an XML configuration from `UnturnedImagesRocketConfiguration`. The default config includes two sample item overrides and two sample vehicle overrides.

Supported fields:

- `DefaultRepositories.Items`
- `DefaultRepositories.Vehicles`
- `ItemOverrides`
- `VehicleOverrides`
- `Id`
- `Guid`
- `WorkshopId`
- `Repository`

Overrides are checked in order. The first matching override wins.

## Placeholders

- `{ItemId}` is replaced with the item's asset GUID.
- `{VehicleId}` is replaced with the vehicle's asset GUID. Paintable vehicles append RGB values as `guid-r-g-b`.

## Example Overrides

```xml
<ItemOverrides>
  <OverrideConfig>
    <Id>46000-47000</Id>
    <Repository>https://cdn.jsdelivr.net/gh/F-Plugins/UnturnedImages@images/workshop/example/items/{ItemId}.png</Repository>
  </OverrideConfig>
  <OverrideConfig>
    <WorkshopId>3030546506</WorkshopId>
    <Repository>https://cdn.jsdelivr.net/gh/F-Plugins/UnturnedImages@images/workshop/3030546506/items/{ItemId}.png</Repository>
  </OverrideConfig>
</ItemOverrides>

<VehicleOverrides>
  <OverrideConfig>
    <Id>12000-12100</Id>
    <Repository>https://cdn.jsdelivr.net/gh/F-Plugins/UnturnedImages@images/workshop/example/vehicles/{VehicleId}.png</Repository>
  </OverrideConfig>
  <OverrideConfig>
    <Guid>b1db3548447c4cde9f92d274e22f57a2</Guid>
    <Repository>https://cdn.jsdelivr.net/gh/F-Plugins/UnturnedImages@images/workshop/example/vehicles/{VehicleId}.png</Repository>
  </OverrideConfig>
</VehicleOverrides>
```
