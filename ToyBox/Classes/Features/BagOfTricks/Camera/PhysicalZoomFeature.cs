using Kingmaker.View;
using UnityEngine;

namespace ToyBox.Features.BagOfTricks.Camera;

[HarmonyPatch, ToyBoxPatchCategory("ToyBox.Features.BagOfTricks.Camera.PhysicalZoomFeature")]
public partial class PhysicalZoomFeature : FeatureWithPatch
{
    public ref float ZoomFarLimit
    {
        get { return ref Settings.PhysicalZoomFarLimit; }
    }
    public ref float ZoomCloseLimit
    {
        get { return ref Settings.PhysicalZoomCloseLimit; }
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
    [LocalizedString("ToyBox_Features_BagOfTricks_Camera_PhysicalZoomFeature_Description", "Move the camera physically along the Z-Axis towards/away from the player (when following). Overrides the FOV based zoom, but not the custom FOV.")]
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
                    Space(20);
                    UI.Label("┗━");
                    Space(10);
                    UI.Label("Far Limit:");
                    UI.Slider(ref ZoomFarLimit, -15f, 15f, 5f, 0, null, null, AutoWidth(), GUILayout.MinWidth(50), GUILayout.MaxWidth(150));
                    Space(10);
                    UI.Label("Close Limit:");
                    UI.Slider(ref ZoomCloseLimit, 15f, 25f, 20f, 0, null, null, AutoWidth(), GUILayout.MinWidth(50), GUILayout.MaxWidth(150));
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
        __result = GetInstance<PhysicalZoomFeature>().ZoomFarLimit;
    }

    [HarmonyPatch(typeof(CameraZoom), nameof(CameraZoom.PhysicalZoomMax), MethodType.Getter), HarmonyPostfix]
    private static void CameraZoom_getPhysicalZoomMax_Patch(ref float __result)
    {
        __result = GetInstance<PhysicalZoomFeature>().ZoomCloseLimit;
    }
}
