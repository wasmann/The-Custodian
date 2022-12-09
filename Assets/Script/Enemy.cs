using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    public abstract string EnemyName { get; }
    public abstract List<Card> Deck { get; }
    public abstract int Health { get; }
    public abstract int EnemyID { get; set; }

    public abstract void EnermyChooseACardToPlay(int ID); 

}
