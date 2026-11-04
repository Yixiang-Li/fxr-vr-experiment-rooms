using System;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class RoomIntegrationAudit
{
    public static readonly string[] Sources = {
        "Assets/Rooms/Memory/Scenes/MemoryDev.unity",
        "Assets/Rooms/RoomB/Scenes/RoomB.unity",
        "Assets/Rooms/AidaRoom/Scenes/AidaExperiment.unity"
    };
    public static void Dump()
    {
        var b = new StringBuilder();
        foreach (var path in Sources)
        {
            var scene = EditorSceneManager.OpenScene(path);
            b.AppendLine("SCENE " + path);
            foreach (var r in scene.GetRootGameObjects())
                b.AppendLine($"ROOT {r.name} active={r.activeSelf} pos={r.transform.position} rot={r.transform.eulerAngles}");
            foreach (var t in scene.GetRootGameObjects().SelectMany(r=>r.GetComponentsInChildren<Transform>(true)))
            {
                int missing = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(t.gameObject);
                if(missing>0)b.AppendLine("MISSING SCRIPT " + PathOf(t));
                foreach (var c in t.GetComponents<Component>())
                {
                    if(c==null)continue;
                    if(c is Camera || c is CharacterController || c is Canvas || c is Light || c is MonoBehaviour && !(c is TMPro.TMP_Text) && c.GetType().Assembly.GetName().Name=="Assembly-CSharp")
                    {
                        b.AppendLine($"COMP {PathOf(t)} : {c.GetType().Name} active={t.gameObject.activeSelf} pos={t.position} local={t.localPosition} rot={t.eulerAngles}");
                        if(c is MonoBehaviour)
                        {
                            var s = new SerializedObject(c);var it=s.GetIterator();
                            while(it.NextVisible(true))
                                if(it.propertyType==SerializedPropertyType.ObjectReference)
                                    b.AppendLine($"  REF {it.propertyPath} => {it.objectReferenceValue}");
                        }
                    }
                }
            }
        }
        Directory.CreateDirectory("Builds/Review");
        File.WriteAllText("Builds/Review/source-room-audit.txt", b.ToString());
        Debug.Log("Room source audit saved.");
    }
    static string PathOf(Transform t) => t.parent==null?t.name:PathOf(t.parent)+"/"+t.name;
}
