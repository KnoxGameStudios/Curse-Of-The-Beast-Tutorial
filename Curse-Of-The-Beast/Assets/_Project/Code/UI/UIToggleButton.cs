using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIToggleButton : MonoBehaviour
{
    [SerializeField] private GameObject toggleObject;

    public void ToggleObject()
    {
        if (toggleObject == null) return;

        toggleObject.SetActive(!toggleObject.activeSelf);
    }
}
