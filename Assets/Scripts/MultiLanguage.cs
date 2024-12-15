using System.Collections;
using System.Collections.Generic;
using Assets.SimpleLocalization.Scripts;
using UnityEngine;
using UnityEngine.UI;

public class MultiLanguage : MonoBehaviour
{
    
   public Toggle languageToggle; // Reference to the toggle
   private string previousLanguage = "Portuguese"; // Default starting language

        private void Awake()
   {
        LocalizationManager.Read();

        switch (Application.systemLanguage)
        {
            case SystemLanguage.English:
                LocalizationManager.Language = "English";
                break;
            case SystemLanguage.Portuguese:
                LocalizationManager.Language = "Portuguese";
                break;
        }
   }

        public void Language(string language)
   {
    LocalizationManager.Language = language;
   }

    private void Start()
    {
        // Set the toggle's initial state based on the current language
        languageToggle.isOn = LocalizationManager.Language == "English";
        languageToggle.onValueChanged.AddListener(OnToggleChanged);
    }

    private void OnDestroy()
    {
        // Remove listener when the object is destroyed
        languageToggle.onValueChanged.RemoveListener(OnToggleChanged);
    }

    private void OnToggleChanged(bool isOn)
    {
        if (isOn)
        {
            // Save the previous language and switch to Portuguese
            previousLanguage = LocalizationManager.Language;
            LocalizationManager.Language = "English";
        }
        else
        {
            // Return to the previously saved language
            LocalizationManager.Language = previousLanguage;
        }
    }

}
