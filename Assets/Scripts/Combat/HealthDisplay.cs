using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay : MonoBehaviour
{
    [SerializeField] private Health health = null;
    [SerializeField] private GameObject healthBarParent = null;
    [SerializeField] private Image healthBarImage = null;

    private void Awake()
    {
        health.ClientOnHealthUpdated += HandleHealthUpdated;
    }

    private void OnDestroy()
    {
        health.ClientOnHealthUpdated -= HandleHealthUpdated;
    }

    private void OnMouseEnter()
    {
        healthBarParent.SetActive(true); // turn on UI
    }

    private void OnMouseExit()
    {
        healthBarParent.SetActive(false); // turn off UI
    }

    private void HandleHealthUpdated(int currentHealth, int maxHealth)
    {
        healthBarImage.fillAmount = (float)currentHealth / maxHealth; // currentHealth converted to float so will give us a float and not an int value.
    }
}
