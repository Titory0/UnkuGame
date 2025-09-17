using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class DebugCameraShake : MonoBehaviour
{
    public float shakeDistance = 1f;
    public float shakesPerSec = 10f;
    public float shakeTime = 1f;    
    public AnimationCurve shakeCurve = null;

    public void TestCamShake()
    {
        CameraShake.Instance.AddShake(shakeTime, shakeDistance, shakesPerSec, shakeCurve);
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(DebugCameraShake))]
[CanEditMultipleObjects]
public class DebugCameraShakeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        EditorGUILayout.Space();

        using (new EditorGUI.DisabledScope(!EditorApplication.isPlaying))
        {
            if (GUILayout.Button("Test Cam Shake"))
            {
                foreach (Object t in targets)
                {
                    var comp = t as DebugCameraShake;
                    if (comp != null)
                        comp.TestCamShake();
                }
            }
        }

        if (!EditorApplication.isPlaying)
            EditorGUILayout.HelpBox("Le test nécessite le Play Mode.", MessageType.Info);
    }

    [MenuItem("CONTEXT/DebugCameraShake/Test Cam Shake")]
    private static void ContextTest(MenuCommand command)
    {
        var comp = command.context as DebugCameraShake;
        if (comp == null) return;

        if (!EditorApplication.isPlaying)
        {
            EditorUtility.DisplayDialog("Test Cam Shake", "Passez en Play Mode pour lancer le test.", "OK");
            return;
        }

        comp.TestCamShake();
    }
}
#endif

