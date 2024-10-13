using UnityEngine;

public class VariationPooler : MonoBehaviour
{
    public GameObject[] hair;
    public GameObject[] head;
    public GameObject[] torsos;
    public GameObject[] ties;

    private void OnEnable()
    {
        EnableRandomVariations();
    }

    // Call this function to enable one random object from each array
    public void EnableRandomVariations()
    {
        // Disable all objects in each array, and enable one at random
        DisableAllAndEnableRandom(hair);
        DisableAllAndEnableRandom(head);
        DisableAllAndEnableRandom(torsos);
        DisableAllAndEnableRandom(ties);
    }

    // Method to disable all objects in the array and enable one random object
    private void DisableAllAndEnableRandom(GameObject[] objects)
    {
        if (objects == null || objects.Length == 0)
            return;

        // Disable all objects in the array
        foreach (GameObject obj in objects)
        {
            if (obj != null)
                obj.SetActive(false);
        }

        // Choose a random object to enable
        int randomIndex = Random.Range(0, objects.Length);
        if (objects[randomIndex] != null)
            objects[randomIndex].SetActive(true);
    }
}
