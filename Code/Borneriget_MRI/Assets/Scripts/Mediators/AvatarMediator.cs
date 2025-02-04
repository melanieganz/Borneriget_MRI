﻿using PureMVC.Interfaces;
using PureMVC.Patterns.Mediator;
using UnityEngine;

namespace Borneriget.MRI
{
    public class AvatarMediator : Mediator
    {
        public AvatarMediator(PreferencesProxy.Avatars avatar, bool danishSpeaks) : base(NAME) 
        {
            DanishSpeaks = danishSpeaks;
            Avatar = avatar;
        }

        public new static string NAME = "AvatarMediator";

        public static class Notifications
        {
            public const string TouchAvatar = "TouchAvatar";
            public const string AvatarAwake = "AvatarAwake";
            public const string AvatarSpeak = "AvatarSpeak";
            public const string StopSpeak = "StopSpeak";
            public const string SpeakDone = "SpeakDone";
        }

        private AvatarView View => (AvatarView)ViewComponent;

        private PreferencesProxy.Avatars Avatar;
        private bool DanishSpeaks;
        private bool AvatarWokenUp = false;

        public override void OnRegister()
        {
            base.OnRegister();
            InitializeView();
        }

        public override void OnRemove()
        {
            base.OnRemove();
            View.ResetAvatar();
        }

        public override string[] ListNotificationInterests()
        {
            return new[] {
                Notifications.TouchAvatar,
                Notifications.AvatarSpeak,
                Notifications.StopSpeak,
            };
        }

        private void InitializeView()
        {
            ViewComponent = Object.FindObjectOfType<AvatarView>(true);
            View.Show(Avatar, DanishSpeaks);
            View.WakeUpSpeak();
        }

        public override void HandleNotification(INotification notification)
        {
            switch (notification.Name)
            {
                case Notifications.TouchAvatar:
                    if (AvatarWokenUp)
                    {
                        View.ShowStars();
                    }
                    else
                    {
                        AvatarWokenUp = true;
                        View.WakeUp(() => SendNotification(Notifications.AvatarAwake));
                    }
                    break;
                case Notifications.AvatarSpeak:
                    View.Speak((int)notification.Body, () => Facade.SendNotification(Notifications.SpeakDone));
                    break;
                case Notifications.StopSpeak:
                    View.StopSpeak();
                    Facade.SendNotification(Notifications.SpeakDone);
                    break;
            }
        }
    }
}
