﻿using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Video;

namespace Borneriget.MRI
{
    public class Story2dView : MonoBehaviour, IPointerClickHandler, IStoryView
    {
        [SerializeField]
        private Camera MenuCam;
        [SerializeField]
        private RawImage Background;
        [SerializeField]
        private Texture[] BackgroundImages;
        [SerializeField]
        private GameObject Bear;
        [SerializeField]
        private GameObject VideoImage;
        [SerializeField]
        private Image VideoProgress;
        [SerializeField]
        private GameObject ExitButton;

        [SerializeField]
        private GameObject ButtonParent;
        [SerializeField]
        private Button[] Buttons;

        public event Action<int> SelectRoom;
        public event Action Exit;
        private bool avatarClicked;

        private void Awake()
        {
            VideoProgress.fillAmount = 0;
            Background.gameObject.SetActive(false);
            Bear.SetActive(false);
            ButtonParent.SetActive(false);
            VideoImage.SetActive(false);
            ExitButton.SetActive(false);
            var buttonIndex = 0;
            foreach (var button in Buttons)
            {
                var idx = buttonIndex++;
                button.onClick.AddListener(() => Button_Click(idx));
            }
        }

        public void Initialize(string doneNotification)
        {
            StartCoroutine(InitializeCo(doneNotification));
        }

        private IEnumerator InitializeCo(string doneNotification)
        {
            yield return null;
            Bootstrap.Facade.SendNotification(doneNotification);
        }

        public void Show(int room, string doneNotification)
        {
            avatarClicked = false;
            VideoProgress.fillAmount = 0;
            ExitButton.SetActive(true);
            VideoImage.SetActive(false);
            MenuCam.enabled = true;
            Background.gameObject.SetActive(true);
            Bear.SetActive(true);
            if (string.IsNullOrEmpty(doneNotification))
            {
                // We have no notification, so we will show the menu and wait for a click.
                Background.texture = BackgroundImages.SafeGet(0);
                ButtonParent.SetActive(true);
            }
            else
            {
                Background.texture = BackgroundImages.SafeGet(room);
                StartCoroutine(ShowCo(doneNotification));
            }
        }

        private IEnumerator ShowCo(string doneNotification)
        {
            yield return new WaitForSeconds(1f);
            Bootstrap.Facade.SendNotification(doneNotification);
            yield return new WaitForSeconds(10f);
            if (!avatarClicked)
            {
                Bootstrap.Facade.SendNotification(StoryMediator.Notifications.AvatarClicked);
            }
        }

        public void Hide()
        {
            Background.gameObject.SetActive(false);
            Bear.SetActive(false);
            ButtonParent.SetActive(false);
            VideoImage.SetActive(false);
            ExitButton.SetActive(false);
        }

        public void ShowButtons()
        {
            ButtonParent.SetActive(true);
        }

        private void Button_Click(int index)
        {
            ButtonParent.SetActive(false);
            SelectRoom?.Invoke(index);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (VideoImage.activeInHierarchy)
            {
                // We are playing a video. A click will pause it
                Bootstrap.Facade.SendNotification(VideoMediator.Notifications.TogglePause);
            }
            else
            {
                var target = eventData.pointerCurrentRaycast.gameObject;
                if (target == Bear)
                {
                    if (!avatarClicked)
                    {
                        avatarClicked = true;
                        Bootstrap.Facade.SendNotification(StoryMediator.Notifications.AvatarClicked);
                    }
                }
                if (target == ExitButton)
                {
                    Exit?.Invoke();
                }
            }
        }

        public void ShowVideo()
        {
            Bear.SetActive(false);
            Background.gameObject.SetActive(false);
            VideoImage.SetActive(true);
        }

        public void SetVideoProgress(VideoProgress progress)
        {
            VideoProgress.fillAmount = progress.Progress;
        }
    }
}
