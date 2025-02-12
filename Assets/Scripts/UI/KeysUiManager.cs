﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class KeysUiManager : MonoBehaviour
{
    public GameObject redKeyUIPrefab;
    public GameObject blueKeyUIPrefab;
    public GameObject yellowKeyUIPrefab;
    public GameObject grayKeyUIPrefab;

    /// <summary>
    /// A Dictionary with the current instance of the UI Prefabs. To not have a lot of public GameObjects
    /// </summary>
    private Dictionary<KeyColor, UIKey> uiKeys = new Dictionary<KeyColor, UIKey>();

    private void Start()
    {
        UpdateList();
    }

    private void OnEnable()
    {
        GameEvents.UpdateKeysHandler += UpdateList;
    }

    private void OnDisable()
    {
        GameEvents.UpdateKeysHandler -= UpdateList;

    }

    /// <summary>
    /// Gets the Keys Dictionary from the GameManager and Updates the UI
    /// </summary>
    private void UpdateList()
    {
        foreach (KeyValuePair<KeyColor, int> ky in GameManager.GetInstance().GetKeys())
        {
            SetUIKey(ky.Key, ky.Value);
        }
    }

    /// <summary>
    /// Set the UI Image of the key in the panel. Depends of the keyColor to set the apropiated prefab.
    /// </summary>
    /// <param name="ky"></param>
    private void SetUIKey(KeyColor ky, int amount)
    {
        if (!uiKeys.ContainsKey(ky))
        {
            uiKeys.Add(ky, new UIKey(ky));
        }

        if (uiKeys[ky].instantiated && amount <= 0)
        {
            Destroy(uiKeys[ky].keyInstance);
            uiKeys[ky].instantiated = false;
        }

        if (!uiKeys[ky].instantiated && amount > 0)
        {
            //Instantiate the Prefab by color
            switch (ky)
            {
                case KeyColor.BLUE:
                    //Use the UIKey class to hold the Current GameObject
                    uiKeys[ky].SetInstance(Instantiate(blueKeyUIPrefab, transform));
                    break;
                case KeyColor.RED:
                    uiKeys[ky].SetInstance(Instantiate(redKeyUIPrefab, transform));

                    break;
                case KeyColor.YELLOW:
                    uiKeys[ky].SetInstance(Instantiate(yellowKeyUIPrefab, transform));
                    break;
                case KeyColor.GRAY:
                    uiKeys[ky].SetInstance(Instantiate(grayKeyUIPrefab, transform));
                    break;
            }

            //Opening animation
            LeanTween.scale(uiKeys[ky].keyInstance.transform.GetChild(0).GetComponent<RectTransform>(), new Vector3(1, 1, 1), 0.3f);

        }
        else if (amount > 1)
        {
            uiKeys[ky].keyInstance.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = amount.ToString();
        }
    }
}

/// <summary>
/// Stores the Current Instance of the GO of the UI Key
/// </summary>
public class UIKey
{
    public KeyColor keyColor;
    public GameObject keyInstance;
    public bool instantiated;

    public UIKey(KeyColor c)
    {
        keyColor = c;
        instantiated = false;
    }

    public void SetInstance(GameObject g)
    {
        keyInstance = g;
        instantiated = true;
    }


}
