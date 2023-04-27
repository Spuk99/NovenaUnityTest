using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderEvent : MonoBehaviour
{
    [SerializeField]
    private AudioSource audioSource;

    void Start()
    {
        this.gameObject.GetComponent<Slider>().onValueChanged.AddListener(delegate { SliderEventMethod(); });
    }

    public void SliderEventMethod()
    {
        audioSource.time = this.gameObject.GetComponent<Slider>().value;
    }
}
