using StudentSurvival.Characters;
using StudentSurvival.World;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace StudentSurvival.Inventory
{
    /// <summary>Finite room supplies, registered before save restoration and never respawned by stage events.</summary>
    public sealed class CampaignSupplies : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Initialize()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneLoaded += OnSceneLoaded;
            Attach(SceneManager.GetActiveScene());
        }
        private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        { if (mode == LoadSceneMode.Single) Attach(scene); }
        private static void Attach(Scene scene)
        {
            if (scene.path != "Assets/Main.unity" || FindFirstObjectByType<CampaignSupplies>(FindObjectsInactive.Include) != null) return;
            new GameObject("Campaign supplies — finite per run").AddComponent<CampaignSupplies>();
        }

        private void Awake()
        {
            // Unity defers these clones' Awake until their inactive parent is enabled.
            // This installs the IDs, quantities, and world positions before LootBoxItem registers them.
            var staging = new GameObject("Prepared world pickups");
            staging.transform.SetParent(transform, false);
            staging.SetActive(false);
            Character player = FindFirstObjectByType<Character>();
            SpriteRenderer playerArt = player != null ? player.GetComponentInChildren<SpriteRenderer>() : null;
            Create(staging.transform, playerArt, "Lab", "campaign.stage2.labs", 3, 20.8f);
            Create(staging.transform, playerArt, "ShavermaDropped", "campaign.stage2.food", 2, 26.7f);
            Create(staging.transform, playerArt, "EnergyDring", "campaign.stage2.energy", 1, 24f);
            Create(staging.transform, playerArt, "Lab", "campaign.stage3.labs", 3, 40.8f);
            Create(staging.transform, playerArt, "ShavermaDropped", "campaign.stage3.food", 2, 44.4f);
            staging.SetActive(true);
        }

        private static void Create(Transform parent, SpriteRenderer playerArt, string resource, string id, int units, float x)
        {
            InventoryItem prefab = Resources.Load<InventoryItem>("Inventory/Items/" + resource);
            if (prefab == null) { Debug.LogError("Missing campaign supply resource: " + resource); return; }
            InventoryItem item = Instantiate(prefab, parent, false);
            LootBoxItem pickup = item.GetComponent<LootBoxItem>();
            if (pickup == null)
            {
                Debug.LogError("Campaign supply needs LootBoxItem: " + resource, item);
                Destroy(item.gameObject);
                return;
            }
            item.gameObject.name = id;
            item.transform.position = new Vector3(x, 3.55f, 0);
            item.SetCount(units);
            pickup.SetHeldId(id);
            // Held-item resource prefabs are intentionally transparent. World supplies must be visible.
            foreach (SpriteRenderer art in item.GetComponentsInChildren<SpriteRenderer>(true))
            {
                Color color = art.color; color.a = 1; art.color = color;
                art.enabled = true;
                if (playerArt != null)
                { art.sortingLayerID = playerArt.sortingLayerID; art.sortingOrder = playerArt.sortingOrder - 1; }
            }
            item.gameObject.SetActive(true);
        }
    }
}
