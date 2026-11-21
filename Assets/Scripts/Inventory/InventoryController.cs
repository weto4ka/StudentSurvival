using System;
using System.Collections.Generic;
using StudentSurvival.Characters;
using StudentSurvival.Controls;
using StudentSurvival.World;
using UnityEngine;

namespace StudentSurvival.Inventory
{
    public class InventoryController : MonoBehaviour, IRunSaveParticipant
    {
        [SerializeField] private List<InventoryItem> items = new List<InventoryItem>(GameInput.SlotCount);
        [SerializeField] private GameObject player;
        private Character owner;
        private float nextRepeatAt;
        private const float LabRepeatInterval = .18f;
        public event Action Changed;
        public int SelectedSlot { get; private set; }
        public int RestoreOrder { get { return 20; } }
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
            if (GameInput.UseItemPressed)
            { UseSlot(SelectedSlot); nextRepeatAt = Time.time + LabRepeatInterval; }
            else if (GameInput.UseItemHeld && Time.time >= nextRepeatAt)
            {
                InventoryItem selected = ItemAt(SelectedSlot);
                if (selected != null && selected.itemType == Item.Lab)
                { UseSlot(SelectedSlot); nextRepeatAt = Time.time + LabRepeatInterval; }
            }
            for (int i = 0; i < GameInput.SlotCount; i++)
                if (GameInput.SlotPressed(i)) UseSlot(i);
        }
        public void UseSlot(int index)
        {
            if (GameSession.BlocksGameplay || owner == null || index < 0 || index >= items.Count) return;
            SelectedSlot = index;
            InventoryItem item = ItemAt(index);
            if (item == null) { GameplayFeedback.Show("feedback.emptySlot"); return; }
            if (!item.TryUse(owner)) return;
            if (item.Count <= 0) { if (item.itemType == Item.Lab) GameplayFeedback.Show("feedback.labsEmpty"); items[index] = null; Destroy(item.gameObject); }
            Changed?.Invoke();
            RunSaveService.RequestSave();
        }
        public bool SpendCollectedLab()
        {
            if (GameSession.BlocksGameplay) return false;
            for (int i = 0; i < items.Count; i++)
            {
                InventoryItem item = items[i];
                if (item == null || item.itemType != Item.Lab || item.Count < 1) continue;
                item.SetCount(item.Count - 1);
                if (item.Count == 0) { items[i] = null; Destroy(item.gameObject); }
                Changed?.Invoke(); RunSaveService.RequestSave();
                return true;
            }
            return false;
        }
        private void Select(int direction)
        {
            for (int step = 1; step <= items.Count; step++)
            {
                int next = (SelectedSlot + direction * step + items.Count) % items.Count;
                if (items[next] == null) continue;
                SelectedSlot = next;
                Changed?.Invoke();
                RunSaveService.RequestSave();
                return;
            }
        }
        public bool Add(InventoryItem item)
        {
            if (GameSession.BlocksGameplay || item == null || owner == null) return false;
            int units = item.PickupUnits;
            LootBoxItem pickup = item.GetComponent<LootBoxItem>();
            string pickupId = pickup != null ? pickup.PersistentId : null;
            for (int i = 0; i < items.Count; i++)
            {
                InventoryItem stored = items[i];
                if (stored == item) return false;
                if (stored == null || stored.itemType != item.itemType) continue;
                stored.SetCount(stored.Count + units);
                SelectedSlot = i;
                item.gameObject.SetActive(false);
                Destroy(item.gameObject);
                GameSession.RecordPickup();
                RunSaveService.CollectPickup(pickupId);
                Changed?.Invoke();
                return true;
            }
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i] != null) continue;
                item.SetCount(units);
                items[i] = item;
                SelectedSlot = i;
                item.transform.SetParent(transform, false);
                item.gameObject.SetActive(false);
                GameSession.RecordPickup();
                RunSaveService.CollectPickup(pickupId);
                Changed?.Invoke();
                return true;
            }
            return false;
        }
        public void Capture(RunSnapshot snapshot)
        {
            snapshot.selectedSlot = SelectedSlot;
            snapshot.inventory = new RunInventorySlot[GameInput.SlotCount];
            for (int i = 0; i < items.Count; i++)
            {
                // Explicit empty records avoid depending on Unity inline-serialization null semantics.
                snapshot.inventory[i] = new RunInventorySlot();
                InventoryItem item = items[i];
                if (item == null || item.Count <= 0) continue;
                LootBoxItem pickup = item.GetComponent<LootBoxItem>();
                snapshot.inventory[i] = new RunInventorySlot { itemType = (int)item.itemType, count = item.Count,
                    sourcePickupId = pickup != null ? pickup.PersistentId : null };
            }
        }
        public void Restore(RunSnapshot snapshot)
        {
            // Resolve all resources before touching live slots, so a missing prefab cannot discard inventory.
            var prefabs = new InventoryItem[GameInput.SlotCount];
            for (int i = 0; i < prefabs.Length; i++)
            {
                RunInventorySlot slot = snapshot.inventory[i];
                if (slot == null || slot.count == 0) continue;
                string resource = slot.itemType == (int)Item.Shaverma ? "ShavermaDropped" : ((Item)slot.itemType).ToString();
                prefabs[i] = Resources.Load<InventoryItem>("Inventory/Items/" + resource);
                if (prefabs[i] == null) throw new InvalidOperationException("Missing saved inventory prefab: " + resource);
            }
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i] != null) Destroy(items[i].gameObject);
                items[i] = null;
                if (prefabs[i] == null) continue;
                // Parenting at instantiation lets pickup Awake distinguish held clones from world pickups.
                InventoryItem item = Instantiate(prefabs[i], transform, false);
                item.SetCount(snapshot.inventory[i].count);
                LootBoxItem pickup = item.GetComponent<LootBoxItem>();
                if (pickup != null) pickup.SetHeldId(snapshot.inventory[i].sourcePickupId);
                item.gameObject.SetActive(false);
                items[i] = item;
            }
            SelectedSlot = snapshot.selectedSlot;
            Changed?.Invoke();
        }
    }
}
