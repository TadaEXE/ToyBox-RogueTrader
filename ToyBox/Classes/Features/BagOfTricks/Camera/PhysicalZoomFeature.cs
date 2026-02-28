using Kingmaker.View;
using UnityEngine;

namespace ToyBox.Features.BagOfTricks.Camera;

[HarmonyPatch, ToyBoxPatchCategory("ToyBox.Features.BagOfTricks.Camera.PhysicalZoomFeature")]
public partial class PhysicalZoomFeature : FeatureWithPatch
{
    public ref float ZoomMin
    {
        get { return ref Settings.PhysicalZoomMin; }
    }
    public ref float ZoomMax
    {
        get { return ref Settings.PhysicalZoomMax; }
    }
    public ref bool UseZoom
    {
        get { return ref Settings.EnablePhysicalZoom; }
    }
    public override ref bool IsEnabled
    {
        get { return ref UseZoom; }
    }

    [LocalizedString("ToyBox_Features_BagOfTricks_Camera_PhysicalZoomFeature_Name", "Use physical zoom")]
    public override partial string Name { get; }
    [LocalizedString("ToyBox_Features_BagOfTricks_Camera_PhysicalZoomFeature_Description", "Move the camera physically along the Z-Axis towards/away from the player (when following). Overrides the FOV based zoom on scroll wheel.")]
    public override partial string Description { get; }
    protected override string HarmonyName
    {
        get { return "ToyBox.Features.BagOfTricks.Camera.PhysicalZoomFeature"; }
    }

    public override void OnGui()
    {
        using (VerticalScope())
        {
            using (HorizontalScope())
            {
                UI.Toggle(Name, Description, ref IsEnabled, Enable, Disable);
            }
            if (IsEnabled)
            {
                using (HorizontalScope())
                {
                    Space(10);
                    UI.Label("->");
                    Space(10);
                    UI.Label("Min:");
                    UI.Slider(ref ZoomMin, 0f, 5f, 5f, 2, null, null, AutoWidth(), GUILayout.MinWidth(50), GUILayout.MaxWidth(150));
                    Space(10);
                    UI.Label("Max:");
                    UI.Slider(ref ZoomMax, 5f, 50f, 20f, 2, null, null, AutoWidth(), GUILayout.MinWidth(50), GUILayout.MaxWidth(150));
                }
            }
        }
    }
    [HarmonyPatch(typeof(CameraZoom), nameof(CameraZoom.EnablePhysicalZoom), MethodType.Getter), HarmonyPostfix]
    private static void CameraZoom_getEnablePhysicalZoom_Patch(ref bool __result)
    {
        __result = GetInstance<PhysicalZoomFeature>().UseZoom;
    }
    [HarmonyPatch(typeof(CameraZoom), nameof(CameraZoom.PhysicalZoomMin), MethodType.Getter), HarmonyPostfix]
    private static void CameraZoom_getPhysicalZoomMin_Path(ref float __result)
    {
        __result = GetInstance<PhysicalZoomFeature>().ZoomMin;
    }

    [HarmonyPatch(typeof(CameraZoom), nameof(CameraZoom.PhysicalZoomMax), MethodType.Getter), HarmonyPostfix]
    private static void CameraZoom_getPhysicalZoomMax_Patch(ref float __result)
    {
        __result = GetInstance<PhysicalZoomFeature>().ZoomMax;
    }
}
