using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class TimeFieldPass : MonoBehaviour
{
    private void Start()
    {
        TimeController.InitTime(0.5f);
        this.GetComponent<InputField>().text = TimeController.TimeGate.ToString();
        this.GetComponent<InputField>().onEndEdit.AddListener(delegate
        {
            TimeController.InitTime(float.Parse(this.GetComponent<InputField>().text));
        });
    }
}
