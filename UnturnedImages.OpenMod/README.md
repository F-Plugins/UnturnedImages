# Unturned Images OpenMod

An OpenMod plugin for Unturned that exposes item and vehicle image URL services to other plugins.

The folder is named `UnturnedImages.OpenMod` to distinguish it from the RocketMod port, but the project still builds as `UnturnedImages.dll` and keeps the existing package identity for compatibility.

## Installation

Install the plugin from the OpenMod NuGet package:

```powershell
dotnet add package Feli.UnturnedImages
```

Server releases also include `UnturnedImages.OpenMod.zip` with the compiled plugin DLL and default `config.yaml`.

## Developer API

Reference the package from your OpenMod plugin and request one of the services through dependency injection:

- `IItemImageDirectorySync`
- `IItemImageDirectoryAsync`
- `IVehicleImageDirectorySync`
- `IVehicleImageDirectoryAsync`

Example:

```csharp
using System;
using UnturnedImages.API.Items;

public class ExampleService
{
    private readonly IItemImageDirectorySync _itemImages;

    public ExampleService(IItemImageDirectorySync itemImages)
    {
        _itemImages = itemImages;
    }

    public string? GetImage(Guid itemGuid)
    {
        return _itemImages.GetItemImageUrlSync(itemGuid);
    }
}
```

## Configuration

The plugin uses `config.yaml`.

```yaml
DefaultRepositories:
  Items: "https://cdn.jsdelivr.net/gh/F-Plugins/UnturnedImages@images/vanilla/items/{ItemId}.png"
  Vehicles: "https://cdn.jsdelivr.net/gh/F-Plugins/UnturnedImages@images/vanilla/vehicles/{VehicleId}.png"

ItemOverrides:
#- Id: "46000-47000"
#  Repository: "https://cdn.jsdelivr.net/gh/F-Plugins/UnturnedImages@images/workshop/example/items/{ItemId}.png"
#- WorkshopId: "3030546506"
#  Repository: "https://cdn.jsdelivr.net/gh/F-Plugins/UnturnedImages@images/workshop/3030546506/items/{ItemId}.png"

VehicleOverrides:
#- Id: "12000-12100"
#  Repository: "https://cdn.jsdelivr.net/gh/F-Plugins/UnturnedImages@images/workshop/example/vehicles/{VehicleId}.png"
#- Guid: "b1db3548447c4cde9f92d274e22f57a2"
#  Repository: "https://cdn.jsdelivr.net/gh/F-Plugins/UnturnedImages@images/workshop/example/vehicles/{VehicleId}.png"
```

Overrides are checked in order. The first matching override wins.

## Placeholders

- `{ItemId}` is replaced with the item's asset GUID.
- `{VehicleId}` is replaced with the vehicle's asset GUID. Paintable vehicles append RGB values as `guid-r-g-b`.
