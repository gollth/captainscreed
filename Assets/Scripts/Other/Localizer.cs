using UnityEngine;

using SmartLocalization;

public class Localizer : MonoBehaviour {

	public enum Language { System, English, German }
	public Language language = Language.System;
	
	void Start () {	SetLanguage ();	}
	
	void SetLanguage(bool suppressLog = false) {
		string l = Application.systemLanguage.ToString ();
		if (language != Language.System) l = language.ToString();
		
		if (!suppressLog) Debug.Log ("System Language is: " + l);
		LanguageManager.Instance.ChangeLanguage (Util.GetLocaleFromLanguageName (l));

	}
}
