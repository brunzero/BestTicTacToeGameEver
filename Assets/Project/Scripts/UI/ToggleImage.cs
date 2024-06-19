using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ToggleImage : MonoBehaviour
{
    public Sprite onImage;
    public Sprite offImage;
    public UnityEvent onEvent;
    public UnityEvent offEvent;

    void Start()
    {
        if (GetComponent<Toggle>().isOn)
            GetComponent<Image>().sprite = onImage;
        else GetComponent<Image>().sprite = offImage;
        GetComponent<Toggle>().onValueChanged.AddListener(OnImageValueChanged);
    }

    void OnImageValueChanged(bool value)
    {
        if (value)
        {
            GetComponent<Image>().sprite = onImage;
            onEvent.Invoke();
        }
        else
        {
            GetComponent<Image>().sprite = offImage;
            offEvent.Invoke();
        }
    }
}
