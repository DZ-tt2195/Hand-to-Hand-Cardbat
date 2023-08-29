using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LocationInfo : MonoBehaviour
{
    TMP_Dropdown dropdown;
    public static LocationInfo instance;
    public int dropValue;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }

    }

    private void Update()
    {
        if (dropdown == null && SceneManager.GetActiveScene().buildIndex == 1)
        {
            dropdown = GameObject.Find("Event Dropdown").GetComponent<TMP_Dropdown>();
            dropdown.onValueChanged.AddListener(delegate { DropdownValueChanged(); });
        }
    }

    void DropdownValueChanged()
    {
        dropValue = dropdown.value;
    }
}
