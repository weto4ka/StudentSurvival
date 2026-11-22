using StudentSurvival.World;
using StudentSurvival.Audio;
using StudentSurvival.Cameras;
using UnityEngine;

namespace StudentSurvival.Characters
{
    public sealed class CampaignRooms : MonoBehaviour
    {
        public const float FloorY = 3.04f;
        private Sprite pixel;
        private CampaignRoomArt roomArt;
        private AudioController bossMusic;
        public void Build()
        {
            pixel = Sprite.Create(Texture2D.whiteTexture, new Rect(0, 0, 1, 1), Vector2.one * 0.5f, 1f, 0, SpriteMeshType.FullRect);
            roomArt = Resources.Load<CampaignRoomArt>("Campaign/RoomArt");
            foreach (AudioController region in FindObjectsByType<AudioController>(FindObjectsSortMode.None))
            {
                AudioSource source = region.GetComponent<AudioSource>();
                if (source != null && source.loop && source.clip != null && source.clip.name.IndexOf("Boss", System.StringComparison.OrdinalIgnoreCase) >= 0)
                { bossMusic = region; break; }
            }
            Room(25, 2, new Color(.16f, .21f, .24f)); Room(45, 3, new Color(.2f, .16f, .23f));
            Portal("Next classroom", new Vector3(-.8f, FloorY, 0), new Vector3(19.7f, FloorY + .04f, 0), 2);
            Portal("Previous classroom", new Vector3(19.7f, FloorY, 0), new Vector3(-1.3f, FloorY + .04f, 0), 1);
            Portal("Final classroom", new Vector3(30.3f, FloorY, 0), new Vector3(39.7f, FloorY + .04f, 0), 3);
            Portal("Previous classroom", new Vector3(39.7f, FloorY, 0), new Vector3(29.7f, FloorY + .04f, 0), 2);
        }
        private void Room(float x, int stage, Color color)
        {
            var cameraFrame = new GameObject("Classroom camera frame " + stage);
            cameraFrame.transform.SetParent(transform, false);
            cameraFrame.AddComponent<CameraBoundsRegion>().Configure(new Bounds(new Vector3(x, stage == 3 ? 5.7f : 5.8f, 0), new Vector3(14f, 7.2f, 0)));
            if (bossMusic != null)
            {
                GameObject music = Instantiate(bossMusic.gameObject, new Vector3(x, 5.75f, 0), Quaternion.identity, transform);
                music.name = "Classroom music region " + stage;
                music.transform.localScale = Vector3.one;
                BoxCollider2D region = music.GetComponent<BoxCollider2D>();
                if (region != null) { region.isTrigger = true; region.offset = Vector2.zero; region.size = new Vector2(14f, 5.42f); }
            }
            Pixel("Classroom backdrop", new Vector2(x, 5.75f), new Vector2(14, 5.42f), color, false, 9);
            Sprite backdrop = roomArt != null ? roomArt.ForStage(stage) : null;
            if (backdrop != null)
            {
                var art = new GameObject("Original university pixel artwork"); art.transform.SetParent(transform, false);
                // Original classroom floor is 2.76 below its sprite center; corridor floor is 2.66 below.
                art.transform.position = new Vector3(x, stage == 3 ? FloorY + 2.66f : FloorY + 2.76f, 0);
                art.transform.localScale = new Vector3(14f / Mathf.Max(.01f, backdrop.bounds.size.x), 1, 1);
                SpriteRenderer renderer = art.AddComponent<SpriteRenderer>(); renderer.sprite = backdrop; renderer.sortingOrder = 10;
            }
            Pixel("Floor", new Vector2(x, FloorY - .2f), new Vector2(14.4f, .4f), new Color(.32f,.31f,.27f), true, 30);
            Pixel("Roof", new Vector2(x, 8.66f), new Vector2(14.4f,.4f), new Color(.32f,.31f,.27f), true, 30);
            Pixel("Left wall", new Vector2(x-7.2f,5.75f), new Vector2(.4f,5.42f), color*.7f, true, 30);
            Pixel("Right wall", new Vector2(x+7.2f,5.75f), new Vector2(.4f,5.42f), color*.7f, true, 30);
            if (backdrop == null)
            {
                Pixel("Blackboard", new Vector2(x,6.5f),new Vector2(6,2.2f),new Color(.07f,.18f,.15f),false,11);
                for(int i=0;i<5;i++) Pixel("Chalk",new Vector2(x-2f,7.1f-i*.28f),new Vector2(2.3f+i*.3f,.035f),new Color(.65f,.73f,.63f),false,12);
            }
        }
        private GameObject Pixel(string name, Vector2 position, Vector2 size, Color color, bool solid, int order)
        {
            var node = new GameObject(name); node.transform.SetParent(transform,false); node.transform.position=position;
            SpriteRenderer art=node.AddComponent<SpriteRenderer>();art.sprite=pixel;art.color=color;art.sortingOrder=order;
            // Size geometry independently of its transform, so trigger and solid collider math stays simple.
            art.drawMode=SpriteDrawMode.Sliced;art.size=size;
            if(solid)node.AddComponent<BoxCollider2D>().size=size;
            return node;
        }
        private void Portal(string name, Vector3 position, Vector3 destination, int unlockedAt)
        {
            GameObject node=Pixel(name,position+Vector3.up*1.2f,new Vector2(.9f,2.4f),new Color(.13f,.38f,.45f),false,31);
            node.transform.position=position;
            // Offset the sprite separately from the actor's feet-origin trigger/teleport destination.
            GameObject graphic=new GameObject("Door frame");graphic.transform.SetParent(node.transform,false);graphic.transform.localPosition=Vector3.up*1.2f;
            SpriteRenderer source=node.GetComponent<SpriteRenderer>(),art=graphic.AddComponent<SpriteRenderer>();
            art.sprite=source.sprite;art.drawMode=source.drawMode;art.size=source.size;art.color=source.color;art.sortingOrder=source.sortingOrder;
            source.enabled=false;
            node.AddComponent<CampaignPortal>().Configure(destination,unlockedAt);
        }
        private void OnDestroy(){if(pixel!=null)Destroy(pixel);}
    }
}
