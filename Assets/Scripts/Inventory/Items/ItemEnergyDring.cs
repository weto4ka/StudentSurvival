namespace StudentSurvival.Inventory.Items
{
    public class ItemEnergyDring : InventoryItem
    {
        protected override void Awake()
        {
            base.Awake();
            itemType = Item.EnergyDring;
            itemUsingType = ItemTypes.Usable;
        }
    }
}
