﻿using OWML.ModHelper.Events;
using UnityEngine;

namespace NomaiVR {
    public class EffectFixes: MonoBehaviour {
        OWCamera _camera;

        void Start () {
            NomaiVR.Log("Started FogFix");

            // Make dark bramble lights visible in the fog.
            var fogLightCanvas = GameObject.Find("FogLightCanvas").GetComponent<Canvas>();
            fogLightCanvas.renderMode = RenderMode.ScreenSpaceCamera;
            fogLightCanvas.worldCamera = Locator.GetActiveCamera().mainCamera;
            fogLightCanvas.planeDistance = 100;

            // Disable underwater effect.
            GameObject.FindObjectOfType<UnderwaterEffectBubbleController>().gameObject.SetActive(false);

            // Disable water entering and exiting effect.
            var visorEffects = FindObjectOfType<VisorEffectController>();
            visorEffects.SetValue("_waterClearLength", 0);
            visorEffects.SetValue("_waterFadeInLength", 0);

            _camera = Locator.GetPlayerCamera();
        }

        void Update () {
            _camera.postProcessingSettings.bloomEnabled = false;
            _camera.postProcessingSettings.chromaticAberrationEnabled = false;
            _camera.postProcessingSettings.colorGradingEnabled = false;
            _camera.postProcessingSettings.phosphenesEnabled = false;
            _camera.postProcessingSettings.vignetteEnabled = false;
        }

        internal static class Patches {
            public static void Patch () {
                NomaiVR.Helper.HarmonyHelper.AddPrefix<PlanetaryFogController>("ResetFogSettings", typeof(Patches), "PatchResetFog");
                NomaiVR.Helper.HarmonyHelper.AddPrefix<PlanetaryFogController>("UpdateFogSettings", typeof(Patches), "PatchUpdateFog");
                NomaiVR.Helper.HarmonyHelper.AddPrefix<FogOverrideVolume>("OverrideFogSettings", typeof(Patches), "PatchOverrideFog");
                NomaiVR.Helper.HarmonyHelper.AddPrefix<Flashback>("OnTriggerFlashback", typeof(Patches), "PatchTriggerFlashback");
                NomaiVR.Helper.HarmonyHelper.AddPrefix<Flashback>("Update", typeof(Patches), "FlashbackUpdate");
            }
            static bool PatchResetFog () {
                return Camera.current.stereoActiveEye != Camera.MonoOrStereoscopicEye.Left;
            }

            static bool PatchUpdateFog () {
                return Camera.current.stereoActiveEye != Camera.MonoOrStereoscopicEye.Right;
            }

            static bool PatchOverrideFog () {
                return Camera.current.stereoActiveEye != Camera.MonoOrStereoscopicEye.Right;
            }

            static void PatchTriggerFlashback (Flashback __instance, Transform ____maskTransform, Transform ____screenTransform) {
                Transform parent;

                if (____screenTransform.parent == __instance.transform) {
                    parent = new GameObject().transform;
                    parent.position = __instance.transform.position;
                    parent.rotation = __instance.transform.rotation;
                    foreach (Transform child in __instance.transform) {
                        child.parent = parent;
                    }
                } else {
                    parent = ____screenTransform.parent;
                }


                parent.position = __instance.transform.position;
                parent.rotation = __instance.transform.rotation;

                ____maskTransform.parent = parent;
            }

            static void FlashbackUpdate (Flashback __instance, Transform ____maskTransform, Transform ____screenTransform) {
                var parent = ____maskTransform.parent;
                parent.rotation = Quaternion.RotateTowards(parent.rotation, __instance.transform.rotation, Time.fixedDeltaTime * 10f);
                parent.position = __instance.transform.position;
            }
        }
    }

}