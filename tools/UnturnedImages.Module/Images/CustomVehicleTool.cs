﻿using SDG.Unturned;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;

namespace UnturnedImages.Module.Images
{
    public class CustomVehicleTool : MonoBehaviour
    {
        public class CustomVehicleIconInfo
        {
            public VehicleAsset VehicleAsset { get; }

            public string OutputPath { get; }

            public int Width { get; }

            public int Height { get; }

            public Vector3 Angles { get; }

            public CustomVehicleIconInfo(VehicleAsset vehicleAsset, string outputPath, int width, int height,
                Vector3 angles)
            {
                VehicleAsset = vehicleAsset;
                OutputPath = outputPath;
                Width = width;
                Height = height;
                Angles = angles;
            }
        }

        private static CustomVehicleTool? _instance;
        private static readonly string[] _weirdLookingObjects = new[]
        {
            "DepthMask"
        };

        private readonly Queue<CustomVehicleIconInfo> Icons = new();
        private Transform _camera = null!;

        public static void Load()
        {
            _instance = UnturnedImagesModule.Instance!.GameObject!.AddComponent<CustomVehicleTool>();
        }

        public static void Unload()
        {
            Destroy(_instance);

            _instance = null;
        }

        private void Start()
        {
            var camera = new GameObject();
            _camera = Instantiate(camera).transform;
        }

        public static Transform? GetVehicle(VehicleAsset vehicleAsset)
		    {
			      var gameObject = vehicleAsset.GetOrLoadModel();

            if (gameObject == null)
            {
                return null;
            }

            return Instantiate(gameObject).transform;
        }

        public static void QueueVehicleIcon(VehicleAsset vehicleAsset, string outputPath, int width, int height,
            Vector3? vehicleAngles = null)
        {
            if (_instance == null)
            {
                return;
            }

            vehicleAngles ??= Vector3.zero;

            var vehicleIconInfo = new CustomVehicleIconInfo(vehicleAsset, outputPath, width, height, vehicleAngles.Value);

            _instance.Icons.Enqueue(vehicleIconInfo);
        }

        private void Update()
        {
            if (Icons.Count == 0)
            {
                return;
            }

            var vehicleIconInfo = Icons.Dequeue();
            var vehicleAsset = vehicleIconInfo.VehicleAsset;
            var vehicle = GetVehicle(vehicleAsset);
            if (vehicle == null)
            {
                UnturnedLog.error($"Could not get model for vehicle with ID {vehicleAsset.GUID}");
                return;
            }

            Layerer.relayer(vehicle, LayerMasks.VEHICLE);
            foreach (var weirdLookingObject in _weirdLookingObjects)
            {
                var child = vehicle.Find(weirdLookingObject);
                if (child != null)
                {
                    child.gameObject.SetActive(false);
                }
            }

            // fix rotors
            var rotors = vehicle.Find("Rotors");
            if (rotors != null)
            {
                for (var i = 0; i < rotors.childCount; i++)
                {
                    var rotor = rotors.GetChild(i);

                    var material0 = rotor.Find("Model_0").GetComponent<Renderer>().material;
                    var material1 = rotor.Find("Model_1").GetComponent<Renderer>().material;

                    if (vehicleAsset.requiredShaderUpgrade)
                    {
                        if (StandardShaderUtils.isMaterialUsingStandardShader(material0))
                        {
                            StandardShaderUtils.setModeToTransparent(material0);
                        }
                        if (StandardShaderUtils.isMaterialUsingStandardShader(material1))
                        {
                            StandardShaderUtils.setModeToTransparent(material1);
                        }
                    }

                    var color = material0.color;
                    color.a = 1f;
                    material0.color = color;

                    color.a = 0f;
                    material1.color = color;

                    rotor.localRotation = Quaternion.identity;
                }
            }

            var vehicleParent = new GameObject().transform;
            vehicle.SetParent(vehicleParent);

            vehicleParent.position = new Vector3(-256f, -256f, 0f);

            if (_camera == null)
                _camera = Instantiate(new GameObject()).transform;

            _camera.SetParent(vehicle, false);

            vehicle.Rotate(vehicleIconInfo.Angles);
            _camera.rotation = Quaternion.identity;

            var orthographicSize = CustomImageTool.CalculateOrthographicSize(vehicleAsset, vehicleParent.gameObject,
                _camera, vehicleIconInfo.Width, vehicleIconInfo.Height, out var cameraPosition);

            _camera.position = cameraPosition;

            if(!vehicleAsset.SupportsPaintColor)
            {
                Texture2D texture = CustomImageTool.CaptureIcon(vehicleAsset.GUID, 0, vehicle, _camera,
                vehicleIconInfo.Width, vehicleIconInfo.Height, orthographicSize, true);

                var path = $"{vehicleIconInfo.OutputPath}.png";

                var bytes = texture.EncodeToPNG();

                ReadWrite.writeBytes(path, false, false, bytes);
            }
            else
            {
                foreach(var color in GetPaintColors(vehicleAsset))
                {
                    PaintableVehicleSection[] paintableVehicleSections = vehicleAsset.PaintableVehicleSections;
                    for (int i = 0; i < paintableVehicleSections.Length; i++)
                    {
                        PaintableVehicleSection paintableVehicleSection = paintableVehicleSections[i];
                        Transform transform = vehicle.Find(paintableVehicleSection.path);
                        if (transform == null)
                        {
                            Assets.reportError(vehicleAsset, "paintable section missing transform \"" + paintableVehicleSection.path + "\"");
                            continue;
                        }

                        Renderer component = transform.GetComponent<Renderer>();
                        if (component == null)
                        {
                            Assets.reportError(vehicleAsset, "paintable section missing renderer \"" + paintableVehicleSection.path + "\"");
                            continue;
                        }

                        List<Material> materials = new();
                        component.GetMaterials(materials);
                        foreach (var material in materials) { material.SetColor(Shader.PropertyToID("_PaintColor"), color);  };
                    }

                    Texture2D texture = CustomImageTool.CaptureIcon(vehicleAsset.GUID, 0, vehicle, _camera,
                            vehicleIconInfo.Width, vehicleIconInfo.Height, orthographicSize, true);

                    var path = $"{vehicleIconInfo.OutputPath}-{color.r}-{color.g}-{color.b}.png";

                    var bytes = texture.EncodeToPNG();

                    ReadWrite.writeBytes(path, false, false, bytes);
                }
            }

            _camera.SetParent(null);
            Destroy(vehicleParent.gameObject);
        }

        private List<Color32> GetPaintColors(VehicleAsset asset)
        {
            if (asset.DefaultPaintColors != null && asset.DefaultPaintColors.Count > 0)
                return asset.DefaultPaintColors;

            return new List<Color32>()
            {
                new Color32(53, 53, 53, 1), // black
                new Color32(55, 101, 140, 1), // blue
                new Color32(46, 100, 46, 1), // green
                new Color32(189, 110, 39, 1), // orange
                new Color32(106, 70, 109, 1), // purple
                new Color32(154, 37, 37, 1), // red
                new Color32(212, 212, 212, 1), // white
                new Color32(205, 170, 30, 1) // yellow
            };
        }
    }
}
