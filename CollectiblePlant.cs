using UnityEngine;

public class CollectiblePlant : MonoBehaviour
{
    public int plantID = 0; // A unique ID for each plant type (optional, useful if you have multiple types of plants)
    public string plantName = "Plant"; // Name of the plant (optional, can be used for UI or inventory)
    public AudioClip pickupSound; // Sound to play when the plant is picked up
    public float pickupVolume = 1.0f; // Volume of the pickup sound (1.0 is the default volume)

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the player collided with this plant
        if (other.CompareTag("Player"))
        {
            Collect(other.gameObject);
        }
    }

    private void Collect(GameObject player)
    {
        // Implement pickup logic here
        Debug.Log(plantName + " collected!");

        // Optional: Add the plant to the player's inventory
        PlayerInventory inventory = player.GetComponent<PlayerInventory>();
        if (inventory != null)
        {
            inventory.AddPlant(plantID, plantName);
        }

        // Play pickup sound with specified volume if available
        if (pickupSound != null)
        {
            AudioSource.PlayClipAtPoint(pickupSound, transform.position, pickupVolume);
        }

        // Destroy the plant after it's collected
        Destroy(gameObject);
    }
}
