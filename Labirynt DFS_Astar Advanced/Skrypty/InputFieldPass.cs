using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine;

public class InputFieldPass : MonoBehaviour
{
    [SerializeField]
    GameObject mapGenerator;
    [SerializeField]
    private InputField inputFieldSX;
    [SerializeField]
    private InputField inputFieldSY;
    [SerializeField]
    private InputField inputFieldEX;
    [SerializeField]
    private InputField inputFieldEY;

    private void Start()
    {
        this.inputFieldEX.text = (this.mapGenerator.GetComponent<MapGenerator>().MapSize.x - 1).ToString();
        this.inputFieldEY.text = (this.mapGenerator.GetComponent<MapGenerator>().MapSize.y - 1).ToString();
        this.GetComponent<Button>().onClick.AddListener(delegate {
            this.mapGenerator.GetComponent<MapGenerator>().SetStartEnd(
                int.Parse(this.inputFieldSX.text),
                int.Parse(this.inputFieldSY.text),
                int.Parse(this.inputFieldEX.text),
                int.Parse(this.inputFieldEY.text)
            );
        });

    }
}
