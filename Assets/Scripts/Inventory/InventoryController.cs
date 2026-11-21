using System;
using System.Collections.Generic;
using StudentSurvival.Characters;
using StudentSurvival.Controls;
using StudentSurvival.World;
using UnityEngine;

namespace StudentSurvival.Inventory
{
    public class InventoryController : MonoBehaviour
    {
        [SerializeField] private List<InventoryItem> items = new List<InventoryItem>(GameInput.SlotCount);
        [SerializeField] private GameObject player;
        private Character owner;
        public event Action Changed;
        public int SelectedSlot { get; private set; }
        public InventoryItem ItemAt(int index) { return index >= 0 && index < items.Count ? items[index] : null; }
        private void Awake()
        {
            if (player == null) player = GameObject.FindGameObjectWithTag("Player");
            owner = player != null ? player.GetComponent<Character>() : null;
            if (items == null) items = new List<InventoryItem>();
            while (items.Count < GameInput.SlotCount) items.Add(null);
            if (items.Count > GameInput.SlotCount) items.RemoveRange(GameInput.SlotCount, items.Count - GameInput.SlotCount);
        }
        private void Update()
        {
            if (GameSession.BlocksGameplay) return;
            if (GameInput.PreviousSlotPressed) Select(-1);
            if (GameInput.NextSlotPressed) Select(1);
            if (GameInput.UseItemPressed) UseSlot(SelectedSlot);
            for (int i = 0; i < GameInput.SlotCount; i++)
                if (GameInput.SlotPressed(i)) UseSlot(i);
        }
        public void UseSlot(int index)
        {
            if (GameSession.BlocksGameplay || owner == null) return;
            SelectedSlot = index;
            InventoryItem item = ItemAt(index);
            if (item == null || !item.TryUse(owner)) return;
            if (item.Count <= 0) { items[index] = null; Destroy(item.gameObject); }
            Changed?.Invoke();
        }
        private void Select(int direction)
        {
            for (int step = 1; step <= items.Count; step++)
            {
                int next = (SelectedSlot + direction * step + items.Count) % items.Count;
                if (items[next] == null) continue;
                SelectedSlot = next;
                Changed?.Invoke();
                return;
            }
        }
        public bool Add(InventoryItem item)
        {
            if (GameSession.BlocksGameplay || item == null || owner == null) return false;
            int units = item.PickupUnits;
            for (int i = 0; i < items.Count; i++)
            {
                InventoryItem stored = items[i];
                if (stored == item) return false;
                if (stored == null || stored.itemType != item.itemType) continue;
                stored.SetCount(stored.Count + units);
                item.gameObject.SetActive(false);
                Destroy(item.gameObject);
                GameSession.RecordPickup();
                Changed?.Invoke();
                return true;
            }
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i] != null) continue;
                item.SetCount(units);
                items[i] = item;
                item.transform.SetParent(transform, false);
                item.gameObject.SetActive(false);
                GameSession.RecordPickup();
                Changed?.Invoke();
                return true;
            }
            return false;
        }
    }
}
