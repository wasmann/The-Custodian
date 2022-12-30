using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoodLabWorkerEnemy : Enemy
{
    // Start is called before the first frame update
    public override string EnemyName { get { return "Good Lab Worker"; } }

    public List<Card> deck;
    public override List<Card> CardsDeck
    {
        get
        {
            return deck;
        }
    }

    public override int Health { get { return 10; } }

    public override int HandCardNum { get { return 3; } }

    public override void EnemyChooseACardToPlay()
    {
        throw new System.NotImplementedException();
    }
}
