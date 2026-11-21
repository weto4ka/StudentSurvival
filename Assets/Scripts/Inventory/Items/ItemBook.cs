namespace StudentSurvival.Inventory.Items
{
    public class ItemBook : InventoryItem
    {
        protected override void Awake()
        {
            base.Awake();
            itemType = Item.Book;
            itemUsingType = ItemTypes.Usable;
        }
    }
}
