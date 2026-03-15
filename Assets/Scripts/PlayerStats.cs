using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private float maxHealth;
    [SerializeField] private float maxMana;
    [SerializeField] private GameObject backBtn;

    public Slider healthBar;
    public Slider manaBar;

    public float currentHealth, currentMana;

    private void Awake()
    {
        GetSliders();
    }

    // Start is called before the first frame update
    void Start()
    {
        SetSlidersMax();

        currentHealth = maxHealth;
        currentMana = maxMana;
    }

    // Update is called once per frame
    void Update()
    {
        RestoreMana();
        UpdateSliders();
        SelectableState();
    }

    // a function that is used to find the correct sliders for the selected unit
    private void GetSliders()
    {
        Slider[] sliders = GetComponentsInChildren<Slider>();
        
        foreach (Slider slider in sliders)
        {
            if (slider.CompareTag("Health"))
            {
                healthBar = slider;
            }
            else if (slider.CompareTag("Mana"))
            {
                manaBar = slider;
            }
        }
    }

    // a function that is used to set the initial size of the health and mana bar to the max
    private void SetSlidersMax()
    {
        healthBar.maxValue = maxHealth;
        manaBar.maxValue = maxMana;

        healthBar.value = maxHealth;
        manaBar.value = maxMana;
    }

    // a function that is used to reduce the health of the unit when it takes damage
    public void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;
    }

    // a function that is used to reduce the mana of the unit when it performs an action
    public void UseMana(float manaAmount)
    {
        currentMana -= manaAmount;
    }

    // a function that is used to automatically restore the unit's mana over time
    private void RestoreMana()
    {
        if (!backBtn.activeSelf)
        {
            currentMana += Time.deltaTime;
        }

        if (currentMana > maxMana)
        {
            currentMana = maxMana;
        }

    }

    // a function that is use to update the bvars display according to the correct stats
    private void UpdateSliders()
    {
        healthBar.value = currentHealth;
        manaBar.value = currentMana;
    }

    // a function that is used to decide wether the unit is selectable or not based on its mana amount
    private void SelectableState()
    {
        if (currentMana <= maxMana / 2 )
        {
            GetComponent<PlayerController>().isSelectable = false;
        }
        else
        {
            GetComponent<PlayerController>().isSelectable = true;
        }
    }
}
