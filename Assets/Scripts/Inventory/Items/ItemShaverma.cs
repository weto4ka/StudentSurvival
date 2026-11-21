namespace StudentSurvival.Inventory.Items
{
    public class ItemShaverma : InventoryItem
    {
        protected override void Awake()
        {
            base.Awake();
            itemType = Item.Shaverma;
            itemUsingType = ItemTypes.Usable;
        }
    }
}
