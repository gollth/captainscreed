using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;


public abstract class Quest : MonoBehaviour {

	public static readonly Dictionary<string, Type> Levels = new Dictionary<string, Type> {
		{ "A",  typeof(TutorialQuest) },
		{ "B",  typeof(CleanerQuest) },
		{ "C",  typeof(CleanerQuest) }
	};

	public string keyPrefix = "game.menu.quest";

	GameObject check, cross;
	protected LocaleStringSetter questInMenu;


	#region Properties
	public bool IsFinished { 
		get { return isFinished; }
		protected set {
			isFinished = value;
			if (isFinished) {
				if (OnFinished != null) OnFinished ();
			}
			if (quests.All (item => item.IsFinished) && OnAllFinished != null)
				OnAllFinished (quests);

			if (questInMenu == null) return;
			check.SetActive (isFulfilled);
			cross.SetActive (!isFulfilled);
		}
	}
	public bool IsFulfilled { 
		get { return isFulfilled; }
		protected set { 
			if (IsFinished){ Debug.LogWarning (this.GetType () + " tried to set IsFulfilled to " + value + ", although it was already finished"); return; }

			isFulfilled = value; 
			if (isFulfilled) {
				if (OnFulfilledOrNot != null) OnFulfilledOrNot ();
				if (quests.All (item => item.IsFulfilled) && OnAllFulfilled != null)
					OnAllFulfilled (quests);
				
			} else {
				quests.Add (this);
			}
		}
	}
	private bool isFulfilled;
	private bool isFinished;
	#endregion

	#region Statics
	public static event Action<List<Quest>> OnAllFulfilled;
	public static event Action<List<Quest>> OnAllFinished;
	private static readonly List<Quest> quests = new List<Quest>();
	#endregion

	#region Events
	public event Action OnFulfilledOrNot;
	public event Action OnFinished;
	#endregion

	void OnLevelLoad (Scene scene, LoadSceneMode mode) {
		quests.Clear();
	}
	
	public virtual void OnEnable () {
		quests.Add (this);
		SceneManager.sceneLoaded += OnLevelLoad;
		ResourceLoader.WaitFor("Menu", menu => {
			questInMenu = menu.transform.Find("quest").Find("numbers").GetComponent<LocaleStringSetter>();
			questInMenu.key = keyPrefix +"."+ GetType().Name.ToLower().Replace("quest","");
			check = questInMenu.transform.Find ("true").gameObject;
			cross = questInMenu.transform.Find ("false").gameObject;
		});

		OnFinished += LogQuestFinish;
	}

	public virtual void OnDisable () {
		SceneManager.sceneLoaded -= OnLevelLoad;
		OnFinished -= LogQuestFinish;
		quests.Remove (this);
	}

	void LogQuestFinish () {
		if (Level.Shutdown || SceneChanger.ChangingLevel) return;
		var manager = GameObject.FindGameObjectWithTag ("GameController").GetComponent<SceneChanger> ();

		Fabric.Answers.Answers.LogCustom ("Quest Finished", new Dictionary<string, object> () {
			{ "Quest", this.GetType().Name },
			{ "Success", IsFulfilled },
			{ "Level", manager.Current.name }
		});
	}

	protected void UpdateMenu (params object[] values) {
		if (questInMenu == null) return;
		questInMenu.SetValues (values);

	}
	
}
