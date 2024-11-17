using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    public VendorUI vendorUI; // Reference to the VendorUI script
    public InventoryObject playerInventory; // Reference to the player's inventory
    public Vendor currentVendor; // Reference to the vendor player is interacting with
    public int playerCurrency = 100; // Example player currency, modify as needed

    private void Update()
    {
        // Open the vendor UI when the player presses 'E' near a vendor
        if (Input.GetKeyDown(KeyCode.E) && currentVendor != null)
        {
            // Example: Select the first item from the player's inventory to sell
            if (playerInventory.Container.Count > 0)
            {
                ItemObject itemToSell = playerInventory.Container[0].item; // Select an item from inventory
                vendorUI.OpenVendorUI(itemToSell, playerCurrency); // Open Vendor UI with selected item
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the player enters the vendor's range
        if (other.CompareTag("Vendor"))
        {
            currentVendor = other.GetComponent<Vendor>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Reset the vendor reference when the player leaves the vendor's range
        if (other.CompareTag("Vendor"))
        {
            currentVendor = null;
        }
    }
}
