using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ApplicationManager : MonoBehaviour {

	[SerializeField]
	public GameObject menuPanel;
	[SerializeField]
	public GameObject background;

	[SerializeField]
	public Slider tickSlider;

	[SerializeField]
	public Slider volumeSlider;

	[SerializeField]
	public Toggle musicTiggle;

	public bool paused = false;
	[SerializeField]
	public GameObject gameManager;
	public void Quit () 
	{
		#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
		#else
		Application.Quit();
		#endif
	}
	public void Continue()
    {
		SceneManager.LoadScene("worldMap");
    }

	public void NewGame()
	{
		SceneManager.LoadScene("Tutorial");
	}

	public void BackToStart()
	{
		SceneManager.LoadScene("Start");
	}
	private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!paused)
            {
				
				background.SetActive(true);
				menuPanel.SetActive(true);
				paused = true;
				gameManager.GetComponent<BattleLevelDriver>().Pause();
            }
            else
            {
				Resume();
				
			}
				
		}

		AudioListener.volume = volumeSlider.value;
        if (musicTiggle.isOn)
        {
			AudioListener.volume = 1;

        }
        else
        {
			AudioListener.volume = 0;
		}
	}

	public void Resume()
    {

		background.SetActive(false);
		menuPanel.SetActive(false);
		paused = false;
		gameManager.GetComponent<BattleLevelDriver>().Pause();
	}

	public void SetTickSpeed()
    {
		GameData.tickspeed = tickSlider.value;
    }
}
