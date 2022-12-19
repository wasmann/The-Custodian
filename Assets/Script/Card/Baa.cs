using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Baa : Card
{
    public override string Name { get { return "Baa"; } }
    public override Rarity rarity { get { return Rarity.trash; } }
    public override int Speed { get { return 3; } }
    public override int ID { get { return 11; } }
    public override IEnumerator Play()
    {

        Info.owner_ID = 0;
        yield return new WaitForSeconds(0.1f);
        UpdateData(0, ID, Info);
    }

    public override void Activate(InfoForActivate Info)
    {
        if (Info.owner_ID == 0)
        {
            //TODO:show animation 
            Debug.Log("player baa");
        }
        else
        {
            //TODO:show animation 
            Debug.Log("sheep baa");
        }
    }
    public override void ReSetTarget()
    {
        TargetNum = 0;
    }
}
