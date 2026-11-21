using StudentSurvival.Characters;
using StudentSurvival.World;
using UnityEngine;

namespace StudentSurvival.Inventory
{
    public enum ItemTypes { Usable, Dropable }
    public enum Item { Shaverma, Book, Lab, EnergyDring }
    public class InventoryItem : MonoBehaviour
    {
        [SerializeField] protected int countItems;
        [SerializeField] public string itemName;
        [SerializeField] protected ItemTypes itemUsingType;
        [SerializeField] public Item itemType;
        public SpriteRenderer sprite;
        public GameObject Parent { get; set; }
        public Vector3 Direction { get; set; }
        public int Count { get { return countItems; } }
        public int PickupUnits { get { return Mathf.Max(1, countItems); } }
        public string NameKey { get { return "item." + itemType; } }
        public string EffectKey { get { return "effect." + itemType; } }
        protected virtual void Awake() { sprite = GetComponentInChildren<SpriteRenderer>(); }
        public void AddOneMore() { countItems++; }
        public void SetCount(int count) { countItems = Mathf.Max(0, count); }
        // One effect implementation for keyboard and on-screen slot controls.
        public bool TryUse(Character player)
        {
            if (player == null || Count <= 0 || GameSession.BlocksGameplay) return false;
            switch (itemType)
            {
                case Item.Book: player.Intelligence++; break;
                case Item.Shaverma: player.Sleep++; break;
                case Item.EnergyDring: player.Sleep++; player.speed += 1f; break;
                case Item.Lab: if (!player.ThrowLab()) return false; break;
                default: return false;
            }
            countItems--;
            GameplayFeedback.Show("feedback.used", NameKey);
            return true;
        }
    }
}
