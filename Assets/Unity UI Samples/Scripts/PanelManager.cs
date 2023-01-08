using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class PanelManager : MonoBehaviour {

	public Animator initiallyOpen;

	private int m_OpenParameterId;
	private Animator m_Open;
	private GameObject m_PreviouslySelected;

	const string k_OpenTransitionName = "Open";
	const string k_ClosedStateName = "Closed";

	public static bool GameIsPaused = false;

	void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                Resume();
            }
            else //if (!GameIsPaused)
            {
				Pause();
            }
        }
    }

	void Pause()
    {
		m_OpenParameterId = Animator.StringToHash (k_OpenTransitionName);

		if (initiallyOpen == null)
			return;

		OpenPanel(initiallyOpen);
        //Time.timeScale = 0f; // Stop the Unity clock!
        GameIsPaused = true;
    }

	public void Resume()
    {
        CloseCurrent();
        //Time.timeScale = 1f;    // Set Unity clock back to normal!
        GameIsPaused = false;
    }

	public void LoadStartMenu()
    {
        Debug.Log("Loading the start menu...");
        Loader.Load(Loader.Scene.StartMenu);
    }

	public void LoadNewGame()
    {
        Debug.Log("Starting a new game...");
        Loader.Load(Loader.Scene.Battle);
    }

	public void OnEnable()
	{
		if (GameIsPaused) { 
			m_OpenParameterId = Animator.StringToHash (k_OpenTransitionName);

			if (initiallyOpen == null)
				return;

			OpenPanel(initiallyOpen);
		}
	}

	public void OpenPanel (Animator anim)
	{
		if (m_Open == anim)
			return;

		anim.gameObject.SetActive(true);
		var newPreviouslySelected = EventSystem.current.currentSelectedGameObject;

		anim.transform.SetAsLastSibling();

		CloseCurrent();

		m_PreviouslySelected = newPreviouslySelected;

		m_Open = anim;
		m_Open.SetBool(m_OpenParameterId, true);

		GameObject go = FindFirstEnabledSelectable(anim.gameObject);

		SetSelected(go);
	}

	static GameObject FindFirstEnabledSelectable (GameObject gameObject)
	{
		GameObject go = null;
		var selectables = gameObject.GetComponentsInChildren<Selectable> (true);
		foreach (var selectable in selectables) {
			if (selectable.IsActive () && selectable.IsInteractable ()) {
				go = selectable.gameObject;
				break;
			}
		}
		return go;
	}

	public void CloseCurrent()
	{
		if (m_Open == null)
			return;

		m_Open.SetBool(m_OpenParameterId, false);
		SetSelected(m_PreviouslySelected);
		StartCoroutine(DisablePanelDeleyed(m_Open));
		m_Open = null;
	}

	IEnumerator DisablePanelDeleyed(Animator anim)
	{
		bool closedStateReached = false;
		bool wantToClose = true;
		while (!closedStateReached && wantToClose)
		{
			if (!anim.IsInTransition(0))
				closedStateReached = anim.GetCurrentAnimatorStateInfo(0).IsName(k_ClosedStateName);

			wantToClose = !anim.GetBool(m_OpenParameterId);

			yield return new WaitForEndOfFrame();
		}

		if (wantToClose)
			anim.gameObject.SetActive(false);
	}

	private void SetSelected(GameObject go)
	{
		EventSystem.current.SetSelectedGameObject(go);
	}
}
