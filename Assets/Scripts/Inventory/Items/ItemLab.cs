namespace StudentSurvival.Inventory.Items
{
    public class ItemLab : InventoryItem
    {
        protected override void Awake()
        {
            base.Awake();
            itemType = Item.Lab;
            itemUsingType = ItemTypes.Dropable;
        }
    }
}
