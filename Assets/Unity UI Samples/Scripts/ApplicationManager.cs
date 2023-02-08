using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ApplicationManager : MonoBehaviour {

	[SerializeField]
	public GameObject menuPanel;

	[SerializeField]
	public Slider tickSlider;

	[SerializeField]
	public Slider volumeSlider;

	[SerializeField]
	public Toggle musicTiggle;

	public bool paused = false;
	[SerializeField]
	public GameObject gameManager;

	public GameObject PauseButton;

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
		SceneManager.LoadScene("StoryTelling");
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

				menuPanel.SetActive(true);
				paused = true;
				gameManager.GetComponent<BattleLevelDriver>().Pause();
				HideHandCard();
				PauseButton.SetActive(false);

			}
            else
            {
				Resume();
				ShowHandCard();
				PauseButton.SetActive(true);
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
		menuPanel.SetActive(false);
		paused = false;
		UI.waitForDuplicate = false;
		gameManager.GetComponent<BattleLevelDriver>().Pause();
		
	}

	public void SetTickSpeed()
    {
		GameData.tickspeed = tickSlider.value;
    }

	public void HideHandCard()
	{
		for (int i = 0; i < BattleData.playerData.handCard.Count; i++)
		{
			BattleData.playerData.handCard[i].gameObject.SetActive(false);
		}
	}

	public void ShowHandCard()
	{
		for (int i = 0; i < BattleData.playerData.handCard.Count; i++)
		{
			BattleData.playerData.handCard[i].gameObject.SetActive(true);
		}
	}

	public void gogogo()
    {
		SceneManager.LoadScene("WorldMap");
		GameData.accessible = 3;
    }

	public void gogogo2()
	{
		SceneManager.LoadScene("WorldMap");
		GameData.accessible = 5;
	}
}
