using UnityEngine;

public class Vendor : MonoBehaviour
{
    // This method handles selling an item
    public void SellItem(ItemObject itemToSell, int amount, InventoryObject inventory)
    {
        // Validate inputs
        if (itemToSell == null || amount <= 0)
        {
            Debug.Log("Invalid item or amount.");
            return;
        }

        // Check if the item exists in the player's inventory
        for (int i = 0; i < inventory.Container.Count; i++)
        {
            // Compare the item in the inventory with the item to sell
            if (inventory.Container[i].item == itemToSell)
            {
                // Check if the player has enough of the item
                if (inventory.Container[i].amount >= amount)
                {
                    // Deduct the specified amount from the inventory
                    inventory.Container[i].AddAmount(-amount);

                    // Example reward logic (customize as needed)
                    int currencyEarned = CalculateSellPrice(itemToSell, amount);
                    // You would need to implement currency handling
                    // playerCurrency.Add(currencyEarned);

                    Debug.Log($"Sold {amount} of {itemToSell.name} for {currencyEarned} currency.");
                    return;
                }
                else
                {
                    Debug.Log("Not enough items to sell.");
                    return;
                }
            }
        }

        Debug.Log("Item not found in inventory.");
    }

    // Example of calculating the selling price
    private int CalculateSellPrice(ItemObject item, int amount)
    {
        int basePrice = 10; // Base price for items (can vary by item type)
        return basePrice * amount; // Total price based on amount sold
    }
}
