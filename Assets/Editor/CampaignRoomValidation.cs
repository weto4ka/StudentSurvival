using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace StudentSurvival.Editor
{
    // Read-only asset validation. Does not create/regenerate rooms or change the open scene.
    [InitializeOnLoad]
    public static class CampaignRoomValidation
    {
        [Serializable] private sealed class Report
        {
            public string version = "1.8.0", dateUtc;
            public bool passed;
            public int prefabs, sprites, colliders, authoredTeachers, authoredStudents;
            public List<string> errors = new List<string>();
        }
        static CampaignRoomValidation() { EditorApplication.delayCall += AutoCheck; }
        private static void AutoCheck()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isCompiling || EditorApplication.isUpdating) return;
            Validate();
        }

        [MenuItem("Tools/Student Survival/Render authored room previews")]
        public static void CapturePreviews()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode) return;
            Directory.CreateDirectory(".utmp/room-previews");
            foreach (string name in new[] { "PracticalClassroom", "OralExamRoom", "CampusToilet" })
            {
                GameObject asset = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Campaign/" + name + ".prefab");
                if (asset == null) continue;
                var preview = new PreviewRenderUtility();
                Texture2D picture = null;
                try
                {
                    preview.InstantiatePrefabInScene(asset);
                    preview.camera.orthographic = true;
                    preview.camera.orthographicSize = name == "CampusToilet" ? 3.3f : 3.8f;
                    preview.camera.transform.position = new Vector3(0, 5.75f, -10);
                    preview.camera.transform.rotation = Quaternion.identity;
                    preview.camera.nearClipPlane = .1f;
                    preview.camera.farClipPlane = 30;
                    preview.camera.clearFlags = CameraClearFlags.SolidColor;
                    preview.camera.backgroundColor = new Color(.07f, .09f, .10f);
                    preview.BeginStaticPreview(new Rect(0, 0, name == "CampusToilet" ? 1000 : 1600, 600));
                    preview.Render();
                    picture = preview.EndStaticPreview();
                    File.WriteAllBytes(".utmp/room-previews/" + name + ".png", picture.EncodeToPNG());
                }
                finally
                {
                    if (picture != null) UnityEngine.Object.DestroyImmediate(picture);
                    preview.Cleanup();
                }
            }
        }
        [MenuItem("Tools/Student Survival/Validate authored campaign rooms")]
        public static void Validate()
        {
            var report = new Report { dateUtc = DateTime.UtcNow.ToString("O") };
            string[] names = { "PracticalClassroom", "OralExamRoom", "CampaignEntrance", "CampusToilet" };
            foreach (string name in names)
            {
                string path = "Assets/Prefabs/Campaign/" + name + ".prefab";
                GameObject asset = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (asset == null) { report.errors.Add("Cannot load prefab: " + path); continue; }
                report.prefabs++;
                foreach (Transform node in asset.GetComponentsInChildren<Transform>(true))
                    if (GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(node.gameObject) > 0)
                        report.errors.Add(path + ": missing script at " + node.name);
                foreach (SpriteRenderer renderer in asset.GetComponentsInChildren<SpriteRenderer>(true))
                {
                    if (renderer.sprite == null) report.errors.Add(path + ": missing sprite at " + renderer.name);
                    else report.sprites++;
                    if (renderer.sharedMaterial == null) report.errors.Add(path + ": missing sprite material at " + renderer.name);
                }
                foreach (Collider2D collider in asset.GetComponentsInChildren<Collider2D>(true)) report.colliders++;
                foreach (MonoBehaviour component in asset.GetComponentsInChildren<MonoBehaviour>(true))
                {
                    if (component == null) continue;
                    if (component.GetType().Name == "Prepod") report.authoredTeachers++;
                    if (component.GetType().Name == "FlirtDistraction") report.authoredStudents++;
                    if (component.GetType().Name != "CampaignRooms") continue;
                    var serialized = new SerializedObject(component);
                    string[] anchors = { "teacherSpawn", "studyPoint", "notesPoint", "hidePoint", "guardStart", "patrolLeft", "patrolRight", "studentPoint", "backDoor", "forwardDoor", "backArrival", "forwardArrival", "labSupply", "foodSupply", "energySupply" };
                    int roomId=serialized.FindProperty("roomId").intValue;
                    if(roomId==4)anchors=new[]{"notesPoint","hidePoint","backDoor","backArrival"};
                    foreach (string anchor in anchors)
                        if (serialized.FindProperty(anchor)?.objectReferenceValue == null)
                            report.errors.Add(path + ": missing marker " + anchor);
                }
            }
            int slices = 0;
            foreach (UnityEngine.Object asset in AssetDatabase.LoadAllAssetsAtPath("Assets/Art/Campaign/UniversityProps.png"))
                if (asset is Sprite) slices++;
            if (slices != 9) report.errors.Add("Expected 8 prop sprites and 1 status pixel; found " + slices);
            if (report.authoredTeachers != 2 || report.authoredStudents != 1) report.errors.Add("Expected two authored teachers and one friendly student.");
            report.passed = report.errors.Count == 0;
            Directory.CreateDirectory(".utmp");
            File.WriteAllText(".utmp/AuthoredRooms.validation.json", JsonUtility.ToJson(report, true));
            if (report.passed) Debug.Log("Authored campaign rooms 1.8.0: asset validation passed (not a gameplay test).");
            else Debug.LogError("Authored campaign room validation: " + string.Join("; ", report.errors));
        }
    }
}
