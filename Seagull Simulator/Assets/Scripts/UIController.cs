using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIController : MonoBehaviour
{
    public Image healthFill;
    public Image CDFill;
    public TextMeshProUGUI wantedDisplay;
    private GameObject seagull;
    private SeagullController seagullController;

    void Start()
    {
        seagull = GameObject.Find("Seagull");
        seagullController = seagull.GetComponent<SeagullController>();
    }

    // Update is called once per frame
    void Update()
    {
        healthFill.fillAmount = seagullController.getHealth() / 100;
        CDFill.fillAmount = seagullController.getFlightCD() / 10;
        wantedDisplay.text = "Wanted level: " + seagullController.getWantedLevel();
    }
}
