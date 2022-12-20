using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallReinforcement : Card
{
    public override string Name { get { return "CallReinforcement"; } }
    public override Rarity rarity { get { return Rarity.trash; } }
    public override int Speed { get { return 10; } }
    public override int ID { get { return 24; } }
    public override IEnumerator Play()
    {
        yield return new WaitForSeconds(0.1f);
        UpdateData(0, ID, Info);
    }

    public override void Activate(InfoForActivate Info)
    {
        if (Info.owner_ID == 0)
        {
            //TODO:show animation 
            Debug.Log("player call someone, but noone actually");
        }
        else
        {
            //create a enemy obj in scene and its battledata
            Debug.Log("new enemy coming");
        }
    }
    public override void ReSetTarget()
    {
        TargetNum = 0;
    }
}
