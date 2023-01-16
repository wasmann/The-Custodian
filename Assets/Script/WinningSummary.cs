using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WinningSummary : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField]
    TMP_Text killed;

    [SerializeField]
    TMP_Text duplicated;
    void Start()
    {
        killed.text = "Killed enemies: " + GameData.killed;
        duplicated.text = "Duplicated cards: " + GameData.duplicated;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
