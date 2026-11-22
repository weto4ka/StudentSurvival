using UnityEngine;

namespace StudentSurvival.Inventory
{
    // Tiny original props/NPC sprites are generated once; no per-frame texture work or imported dependencies.
    public sealed class CheatingPixelArt : MonoBehaviour
    {
        public enum Kind { Notes, Guard, Desk, Toilet, Pixel, Attention }
        public SpriteRenderer Renderer { get; private set; }
        private Texture2D texture;
        private Sprite sprite;
        public static CheatingPixelArt Create(Transform parent, string label, Vector3 localPosition, Kind kind)
        {
            var owner = new GameObject(label); owner.transform.SetParent(parent, false); owner.transform.localPosition = localPosition;
            var art = owner.AddComponent<CheatingPixelArt>(); art.Build(kind);
            return art;
        }
        private void Build(Kind kind)
        {
            int width = kind == Kind.Pixel ? 1 : 20, height = kind == Kind.Guard ? 30 : kind == Kind.Pixel ? 1 : 20;
            texture = new Texture2D(width, height, TextureFormat.RGBA32, false) { filterMode = FilterMode.Point, wrapMode = TextureWrapMode.Clamp, name = "Original " + kind + " pixels" };
            texture.SetPixels(new Color[width * height]);
            Color ink = new Color(.12f, .17f, .22f), paper = new Color(.95f, .94f, .8f), blue = new Color(.18f, .35f, .58f);
            if (kind == Kind.Guard)
            {
                Fill(5, 0, 4, 10, ink); Fill(11, 0, 4, 10, ink);
                Fill(3, 10, 14, 12, blue); Fill(1, 10, 3, 9, blue); Fill(16, 10, 3, 9, blue);
                Fill(6, 21, 8, 7, new Color(.8f, .6f, .42f)); Fill(4, 27, 12, 3, ink); Fill(3, 26, 13, 2, ink);
                Fill(9, 27, 3, 2, Color.yellow); Fill(8, 24, 1, 1, ink); Fill(12, 24, 1, 1, ink);
                Fill(5, 12, 10, 2, ink); Fill(12, 18, 3, 3, Color.yellow);
            }
            else if (kind == Kind.Notes)
            {
                Fill(3, 2, 14, 16, ink); Fill(4, 3, 12, 14, paper);
                for (int y = 6; y <= 14; y += 3) Fill(6, y, 8, 1, blue);
                Fill(12, 14, 4, 3, Color.white);
            }
            else if (kind == Kind.Desk)
            {
                Fill(1, 8, 18, 3, new Color(.45f, .27f, .12f)); Fill(3, 0, 2, 8, ink); Fill(15, 0, 2, 8, ink);
                Fill(5, 11, 11, 6, paper); Fill(10, 12, 1, 5, blue); Fill(6, 13, 3, 1, blue); Fill(12, 13, 3, 1, blue);
            }
            else if (kind == Kind.Toilet)
            {
                Fill(10, 9, 8, 10, new Color(.65f, .76f, .8f)); Fill(11, 10, 6, 8, Color.white);
                Fill(3, 8, 14, 4, Color.white); Fill(5, 6, 10, 3, new Color(.65f, .76f, .8f));
                Fill(8, 1, 5, 5, Color.white); Fill(6, 0, 9, 2, Color.white); Fill(12, 16, 2, 1, ink);
            }
            else if (kind == Kind.Attention)
            { Fill(9, 7, 3, 12, new Color(1, .72f, .15f)); Fill(9, 2, 3, 3, new Color(1, .72f, .15f)); }
            else Fill(0, 0, 1, 1, ink);
            texture.Apply(false, true);
            float ppu = kind == Kind.Pixel ? 1 : kind == Kind.Attention ? 28 : 10;
            sprite = Sprite.Create(texture, new Rect(0, 0, width, height), kind == Kind.Pixel ? Vector2.one * .5f : new Vector2(.5f, 0), ppu, 0, SpriteMeshType.FullRect);
            Renderer = gameObject.AddComponent<SpriteRenderer>(); Renderer.sprite = sprite; Renderer.sortingOrder = 40;
        }
        private void Fill(int x, int y, int width, int height, Color color)
        {
            for (int row = y; row < y + height; row++) for (int column = x; column < x + width; column++) texture.SetPixel(column, row, color);
        }
        private void OnDestroy() { if (sprite != null) Destroy(sprite); if (texture != null) Destroy(texture); }
    }
}
