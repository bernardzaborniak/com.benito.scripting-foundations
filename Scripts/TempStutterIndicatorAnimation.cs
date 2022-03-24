using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TempStutterIndicatorAnimation : MonoBehaviour
{
    [SerializeField] Image image;
    [SerializeField] float speed;

    void Start()
    {
        
    }

    void Update()
    {
       // image.fillAmount = (Mathf.Sin(Time.time * speed)+1)/2;
        image.fillAmount = Time.time % 1;
    }
}
