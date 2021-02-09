using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Borneriget.MRI
{
    public class PreferencesView : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField]
        private GameObject LanguageSelection;
        [SerializeField]
        private GameObject Danish;
        [SerializeField]
        private GameObject English;
        [SerializeField]
        private GameObject AvatarSelection;
        [SerializeField]
        private GameObject Theo;
        [SerializeField]
        private GameObject Thea;
        [SerializeField]
        private GameObject FormatSelection;
        [SerializeField]
        private GameObject Tablet;
        [SerializeField]
        private GameObject Cardboard;

        public event Action<string> LanguageSelected;
        public event Action<PreferencesProxy.Avatars> AvatarSelected;
        public event Action<bool> FormatSelected;

        private void Awake()
        {
            gameObject.SetActive(false);
            LanguageSelection.SetActive(false);
            AvatarSelection.SetActive(false);
            FormatSelection.SetActive(false);
        }

        public void Show()
        {
            gameObject.SetActive(true);
            LanguageSelection.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            var target = eventData.pointerCurrentRaycast.gameObject;
            if (LanguageSelection.activeInHierarchy)
            {
                if (target == Danish)
                {
                    SelectLanguage("da-DK");
                }
                if (target == English)
                {
                    SelectLanguage("en-EN");
                }
            }
            else if (AvatarSelection.activeInHierarchy)
            {
                if (target == Theo)
                {
                    SelectAvatar(PreferencesProxy.Avatars.Theo);
                }
                if (target == Thea)
                {
                    SelectAvatar(PreferencesProxy.Avatars.Theo);
                }
            }
            else if (FormatSelection.activeInHierarchy)
            {
                if (target == Tablet)
                {
                    SelectFormat(false);
                }
                if (target == Cardboard)
                {
                    SelectFormat(true);
                }
            }
        }

        private void SelectLanguage(string language)
        {
            LanguageSelection.SetActive(false);
            LanguageSelected?.Invoke(language);
            AvatarSelection.SetActive(true);
        }

        private void SelectAvatar(PreferencesProxy.Avatars avatar)
        {
            AvatarSelection.SetActive(false);
            AvatarSelected?.Invoke(avatar);
            FormatSelection.SetActive(true);
        }

        private void SelectFormat(bool useVr)
        {
            FormatSelection.SetActive(false);
            FormatSelected?.Invoke(useVr);
        }
    }
}
