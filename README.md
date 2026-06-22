# Unturned Images

A repository of image URLs for assets in the game Unturned, plus plugins that expose those URLs to OpenMod and RocketMod developers.

Images are available on the [images branch](https://github.com/SilKsPlugins/UnturnedImages/tree/images).

For information on contributing, see [CONTRIBUTING.md](/CONTRIBUTING.md).

## Projects

- `UnturnedImages.OpenMod` contains the OpenMod plugin. It still builds as `UnturnedImages.dll` and keeps the existing NuGet/package compatibility.
- `UnturnedImages.RocketMod` contains the RocketMod port and builds as `UnturnedImages.RocketMod.dll`.
- `tools/UnturnedImages.Module` contains the image capture module used to generate image assets.
- `samples/UnturnedImages.Example` contains an OpenMod usage example.

## Image URL Usage

To access images directly, use the [jsDelivr CDN](https://www.jsdelivr.com/?docs=gh). If an image does not exist, the HTTP request returns `404`.

Eaglefire image URL:

```text
https://cdn.jsdelivr.net/gh/SilKsPlugins/UnturnedImages@images/vanilla/items/4.png
```

Blimp image URL:

```text
https://cdn.jsdelivr.net/gh/SilKsPlugins/UnturnedImages@images/vanilla/vehicles/189.png
```

## Plugin API

Both plugin versions expose item and vehicle image directory APIs:

- `UnturnedImages.API.Items.IItemImageDirectorySync`
- `UnturnedImages.API.Items.IItemImageDirectoryAsync`
- `UnturnedImages.API.Vehicles.IVehicleImageDirectorySync`
- `UnturnedImages.API.Vehicles.IVehicleImageDirectoryAsync`

OpenMod consumes these through dependency injection. RocketMod consumes them through `UnturnedImagesRocketPlugin.ItemImages`, `ItemImagesAsync`, `VehicleImages`, and `VehicleImagesAsync`.

## Releases

The unified GitHub release workflow publishes these artifacts together:

- `UnturnedImages.OpenMod.zip`
- `UnturnedImages.RocketMod.zip`
- `UnturnedImages.Module.zip`
- `UnturnedImages.All.zip`

The OpenMod plugin remains available as a NuGet package. The RocketMod project also generates `Feli.UnturnedImages.RocketMod` for developers who want to reference its API from their RocketMod plugins.
