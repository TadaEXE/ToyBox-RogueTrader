using Kingmaker.View;
using ToyBox.Classes.Infrastructure.Features;
using ToyBox.Infrastructure.Keybinds;
using ToyBox.Infrastructure.Utilities;
using UnityEngine;

namespace ToyBox.Features.BagOfTricks.Camera;

[IsTested]
[HarmonyPatch, ToyBoxPatchCategory("ToyBox.Features.BagOfTricks.Camera.AllowMouse3DraggingToAimCameraFeature")]
public partial class AllowMouse3DraggingToAimCameraFeature : FeatureWithPatch, IToggledWithBinding
{
    private static float m_OriginalMinSpaceCameraAngle;
    private static float m_OriginalMaxSpaceCameraAngle;
    private static bool m_OriginalEnableOrbitCamera;
    public override ref bool IsEnabled
    {
        get
        {
            return ref Settings.EnableMouse3DraggingToAimCamera;
        }
    }
    [LocalizedString("ToyBox_Features_BagOfTricks_Camera_AllowMouse3DraggingToAimCameraFeature_Name", "Enable Mouse3 Dragging To Aim The Camera")]
    public override partial string Name { get; }
    [LocalizedString("ToyBox_Features_BagOfTricks_Camera_AllowMouse3DraggingToAimCameraFeature_Description", "Allows orbiting the camera while holding down Mouse Wheel.")]
    public override partial string Description { get; }

    protected override string HarmonyName
    {
        get
        {
            return "ToyBox.Features.BagOfTricks.Camera.AllowMouse3DraggingToAimCameraFeature";
        }
    }
    public ref float VerticalRotationExtraDownwards
    {
        get
        {
            return ref Settings.VerticalRotationExtraDownwards;
        }
    }
    public ref float VerticalRotationExtraUpwards
    {
        get
        {
            return ref Settings.VerticalRotationExtraUpwards;
        }
    }

    public override void OnGui()
    {
        using (VerticalScope())
        {
            using (HorizontalScope())
            {
                UI.Toggle(Name, Description, ref IsEnabled, Enable, Disable);

                var current = Keybind;
                if (UI.HotkeyPicker(ref current, this))
                {
                    Keybind = current;
                }
            }
            if (IsEnabled)
            {
                using (HorizontalScope())
                {
                    Space(20);
                    UI.Label("┗━");
                    Space(10);
                    UI.Label("Extra Up Rotation:");
                    UI.Slider(ref VerticalRotationExtraUpwards, 0f, 45f, 33.3f, 1, null, null, AutoWidth(), GUILayout.MinWidth(50), GUILayout.MaxWidth(150));
                    Space(10);
                    UI.Label("Extra Down Rotation:");
                    UI.Slider(ref VerticalRotationExtraDownwards, 0f, 45f, 33.3f, 1, null, null, AutoWidth(), GUILayout.MinWidth(50), GUILayout.MaxWidth(150));
                }
            }
        }
    }

    public Hotkey? Keybind
    {
        get;
        set;
    }

    public override void Enable()
    {
        base.Enable();
        Keybind = Hotkeys.MaybeGetHotkey(GetType());
    }
    public void ExecuteAction(ActionParameter parameter)
    {
        LogExecution(parameter);
        IsEnabled = !IsEnabled;
        if (IsEnabled)
        {
            Enable();
        }
        else
        {
            Disable();
        }
    }
    public void LogExecution(ActionParameter parameter)
    {
        Helpers.LogExecution(this, parameter);
    }
    [HarmonyPatch(typeof(CameraRig), nameof(CameraRig.TickRotate)), HarmonyPrefix]
    private static void CameraRig_TickRotate_PrePatch(CameraRig __instance)
    {
        m_OriginalMinSpaceCameraAngle = __instance.MinSpaceCameraAngle;
        m_OriginalMaxSpaceCameraAngle = __instance.MaxSpaceCameraAngle;
        m_OriginalEnableOrbitCamera = __instance.m_EnableOrbitCamera;
        __instance.MinSpaceCameraAngle = -GetInstance<AllowMouse3DraggingToAimCameraFeature>().VerticalRotationExtraUpwards;
        __instance.MaxSpaceCameraAngle = GetInstance<AllowMouse3DraggingToAimCameraFeature>().VerticalRotationExtraDownwards;
        __instance.m_EnableOrbitCamera = true;
    }
    [HarmonyPatch(typeof(CameraRig), nameof(CameraRig.TickRotate)), HarmonyPostfix]
    private static void CameraRig_TickRotate_PostPatch(CameraRig __instance)
    {
        __instance.MinSpaceCameraAngle = m_OriginalMinSpaceCameraAngle;
        __instance.MaxSpaceCameraAngle = m_OriginalMaxSpaceCameraAngle;
        __instance.m_EnableOrbitCamera = m_OriginalEnableOrbitCamera;
        // __instance.ScrollToImmediately(__instance.EnsureAboveGround(__instance.posi));
    }
}
