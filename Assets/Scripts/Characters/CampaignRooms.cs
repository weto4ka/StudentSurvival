using System.Collections.Generic;
using StudentSurvival.Audio;
using UnityEngine;

namespace StudentSurvival.Characters
{
    // Scene-authored room references. This component creates no room geometry or artwork.
    public sealed class CampaignRooms : MonoBehaviour
    {
        public const float FloorY = 3.04f;
        private static readonly List<CampaignRooms> rooms = new List<CampaignRooms>();
        [SerializeField, Range(2, 4)] private int roomId = 2;
        [SerializeField] private Transform teacherSpawn, studyPoint, notesPoint, hidePoint;
        [SerializeField] private Transform guardStart, patrolLeft, patrolRight, studentPoint;
        [SerializeField] private Transform backDoor, forwardDoor, backArrival, forwardArrival, toiletArrival, toiletDoor;
        [SerializeField] private Transform labSupply, foodSupply, energySupply;
        public int RoomId => roomId;
        public int RequiredStage => roomId == 4 ? 2 : roomId;
        public Transform TeacherSpawn => teacherSpawn;
        public Transform StudyPoint => studyPoint;
        public Transform NotesPoint => notesPoint;
        public Transform HidePoint => hidePoint;
        public Transform GuardStart => guardStart;
        public Transform PatrolLeft => patrolLeft;
        public Transform PatrolRight => patrolRight;
        public Transform StudentPoint => studentPoint;
        public Transform BackDoor => backDoor;
        public Transform ForwardDoor => forwardDoor;
        public Transform BackArrival => backArrival;
        public Transform ForwardArrival => forwardArrival;
        public Transform ToiletArrival => toiletArrival;
        public Transform ToiletDoor => toiletDoor;
        public Transform LabSupply => labSupply;
        public Transform FoodSupply => foodSupply;
        public Transform EnergySupply => energySupply;
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetRegistry() { rooms.Clear(); }
        private void OnEnable() { if (!rooms.Contains(this)) rooms.Add(this); }
        private void OnDisable() { rooms.Remove(this); }
        public static CampaignRooms Find(int number)
        {
            foreach (CampaignRooms room in rooms) if (room != null && room.roomId == number) return room;
            return null;
        }
        public static void ConfigureMusic()
        {
            AudioController template = null;
            foreach (AudioController region in FindObjectsByType<AudioController>(FindObjectsSortMode.None))
            {
                AudioSource source = region.GetComponent<AudioSource>();
                if (source != null && source.loop && source.clip != null && source.clip.name.IndexOf("Boss", System.StringComparison.OrdinalIgnoreCase) >= 0)
                { template = region; break; }
            }
            if (template == null) return;
            foreach (CampaignRooms room in rooms)
            {
                if(room.roomId>3)continue;
                GameObject music = Instantiate(template.gameObject, room.transform.TransformPoint(new Vector3(0, 5.75f, 0)), Quaternion.identity, room.transform);
                music.name = "Exam music region";
                music.transform.localScale = Vector3.one;
                BoxCollider2D region = music.GetComponent<BoxCollider2D>();
                if (region != null)
                {
                    StudentSurvival.Cameras.CameraBoundsRegion frame = room.GetComponent<StudentSurvival.Cameras.CameraBoundsRegion>();
                    float width = frame != null ? frame.WorldBounds.size.x : 18f;
                    region.isTrigger = true; region.offset = Vector2.zero; region.size = new Vector2(width, 5.42f);
                }
            }
        }
    }
}
