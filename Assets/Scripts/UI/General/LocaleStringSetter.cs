using UnityEngine;
using UnityEngine.UI;
using SmartLocalization;

using System.Linq;

public class LocaleStringSetter : MonoBehaviour {

	public static LocaleStringSetter Find (GameObject root, string key) {
		return root.GetComponentsInChildren<LocaleStringSetter>()
			.FirstOrDefault (item => item.key.Equals (key));
	}
		
	public string key;


	public string Original { get; private set; }
	public int Lines { get { return text.text.Split ('\n').Length; } }

	public Text text;
    object[] cache;

	// Use this for initialization
	void Awake () {
        text = GetComponent<Text> ();
		SyncKeyAndText(null);
        LanguageManager.Instance.OnChangeLanguage += SyncKeyAndText;
	}
	
	void OnDestroy () {
		if (LanguageManager.HasInstance) LanguageManager.Instance.OnChangeLanguage -= SyncKeyAndText;
	}

	public void SyncKeyAndText (LanguageManager manager=null) {
		if (!enabled) return;
		if (manager == null) manager = LanguageManager.Instance;
		Original = manager.GetTextValue (key);  //Cache the original
        if (Util.CountPlaceholders (Original) == 0) 
        	text.text = Original;
        else 
        	SetValues (cache);
	}

    public void SetValues(params object[] numbers) {
		if (Original == null) SyncKeyAndText();
		if (numbers != null) text.text = string.Format(Original, numbers);
        cache = numbers;
    }
}
