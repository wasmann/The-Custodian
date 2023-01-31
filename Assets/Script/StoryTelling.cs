using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StoryTelling : MonoBehaviour
{
    
    public float speed = 0.2f;

    private string story = "\n    In the cyberpunk era, the first super robot is created by the Qoogle company. The term \"super robot\" was first defined by a " +
        "game development team by TUM in 2022 and it describes a kind of robot which can learn from everything in the surrounding environment. " +
        "Different with those artifitial intellegent robot at that time, super robot has something more powerful. If you are old enough, " +
        "you might have heard of \"transformer\". The super robot is like transformer with powerful weapon and independent thought " +
        "but unlimited potential power. \n \n   Terrorism has been the world biggest enemy in the past thirty years. " +
        "They occupied majority of the cities and even some capitals of some nations were lost.In order to fight back, super robot was created and " +
        "put into actual combat. And this story is about the adventure of the first super robot.  It is the first day the super robot comes into being, " +
        "and also the first day when the terrorism break in the research center of Qoogle." ;

    private bool finished;

    private float timer;

    private int currentChar;

    public TMP_Text text;

    public Button button;

    // Start is called before the first frame update
    void Start()
    {
        //text = GetComponent<TMP_Text>().text;
    }

    // Update is called once per frame
    void Update()
    {
        Typing();
    }

    public void Typing()
    {
        if (!finished)
        {
            timer += Time.deltaTime;
            if(timer >= speed)
            {
                timer = 0;
                currentChar++;
                text.text = story.Substring(0, currentChar);

                if(currentChar >= story.Length)
                {
                    Finish();
                }
            }
        }
    }

    public void Finish()
    {
        finished = true;
        timer = 0;
        currentChar = 0;
        text.text = story;
        button.GetComponentInChildren<TMP_Text>().text = "Next";
    }

    public void Skip()
    {
        if (!finished)
        {
            Finish();
        }
        else
        {
            SceneManager.LoadScene("Tutorial");
        }
    }
}
