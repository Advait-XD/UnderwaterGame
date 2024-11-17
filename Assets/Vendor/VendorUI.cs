using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VendorUI : MonoBehaviour
{
    public GameObject vendorPanel; // The panel for the vendor UI
    public TextMeshProUGUI itemText; // Text to display the item name
    public TextMeshProUGUI itemDescriptionText; // Text to display the item description
    public Image itemImage; // Image to display the item
    public TMP_InputField amountInputField; // Input field for the quantity of items to sell
    public TextMeshProUGUI currencyText; // Text to display player's currency

    public Vendor vendor; // Reference to the vendor
    public InventoryObject inventory; // Reference to the player's inventory

    private ItemObject itemToSell; // Reference to the item selected for selling

    private void Start()
    {
        vendorPanel.SetActive(false); // Ensure the panel is hidden when the game starts
    }

    private void Update()
    {
        // Check for E key press to close the UI
        if (Input.GetKeyDown(KeyCode.E) && vendorPanel.activeSelf)
        {
            CloseVendorUI(); // Call the method to close the vendor UI
        }
    }

    // This method opens the vendor UI and displays the selected item
    public void OpenVendorUI(ItemObject item, int playerCurrency)
    {
        if (item == null)
        {
            Debug.Log("No item selected for selling.");
            return;
        }

        vendorPanel.SetActive(true); // Show the vendor panel
        itemToSell = item; // Set the item to sell

        // Update UI with item information
        itemText.text = itemToSell.name; // Item name
        itemDescriptionText.text = itemToSell.description; // Item description
        itemImage.sprite = itemToSell.prefab.GetComponent<SpriteRenderer>()?.sprite; // Assuming the prefab has a SpriteRenderer

        amountInputField.text = "1"; // Default amount to sell
        currencyText.text = $"Currency: {playerCurrency}"; // Display the player's current currency
    }

    // Method to handle the sell button click
    public void OnSellButtonClicked()
    {
        if (int.TryParse(amountInputField.text, out int amountToSell))
        {
            // Call the vendor's SellItem method with the selected item and amount
            vendor.SellItem(itemToSell, amountToSell, inventory);
        }

        vendorPanel.SetActive(false); // Close the vendor UI after selling
    }

    // Method to close the vendor UI without selling
    public void CloseVendorUI()
    {
        vendorPanel.SetActive(false); // Hide the vendor panel
    }
}
