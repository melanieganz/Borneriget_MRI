﻿using PureMVC.Interfaces;
using PureMVC.Patterns.Mediator;
using UnityEngine;

namespace Borneriget.MRI
{
    public class VideoMediator : Mediator
    {
        public VideoMediator(VideoUrls urls) : base(NAME) 
        {
            Urls = urls;
        }

        public new static string NAME = "VideoMediator";

        public static class Notifications
        {
            public const string PrepareVideo = "PrepareVideo";
            public const string PlayVideo = "PlayVideo";
            public const string TogglePause = "TogglePause";
            public const string StopVideo = "StopVideo";
            public const string VideoDone = "VideoDone";
            public const string VideoProgressUpdate = "VideoProgressUpdate";
            public const string VideoPaused = "VideoPaused";
            public const string VideoResumed = "VideoResumed";
            public const string StartVideoSeek = "StartVideoSeek";
            public const string VideoSeek = "VideoSeek";
            public const string EndVideoSeek = "EndVideoSeek";
            public const string ShowSpinner = "ShowSpinner";
        }

        private VideoView View => (VideoView)ViewComponent;
        private PreferencesProxy Preferences;
        private VideoUrls Urls;

        public override void OnRegister()
        {
            base.OnRegister();
            ViewComponent = Object.FindObjectOfType<VideoView>(true);
            View.VideoPrepared += View_VideoPrepared;
            View.VideoDone += View_VideoDone;
            View.VideoProgressUpdate += View_VideoProgressUpdate;
            View.ShowSpinner += View_ShowSpinner;
        }

        private void View_ShowSpinner()
        {
            SendNotification(Notifications.ShowSpinner);
        }

        public override void OnRemove()
        {
            base.OnRemove();
            View.VideoPrepared -= View_VideoPrepared;
            View.VideoDone -= View_VideoDone;
            View.VideoProgressUpdate -= View_VideoProgressUpdate;
            View.ShowSpinner -= View_ShowSpinner;
        }

        private void View_VideoProgressUpdate(VideoProgress progress)
        {
            Facade.SendNotification(Notifications.VideoProgressUpdate, progress);
        }

        private void View_VideoDone()
        {
            Facade.SendNotification(Notifications.VideoDone);
        }

        private void View_VideoPrepared()
        {
            // We are not really doing anything here. It might be involved when we try error scenarios like offline.
        }

        public override string[] ListNotificationInterests()
        {
            return new[] 
            { 
                PreferencesMediator.Notifications.PreferencesSelected,
                Notifications.PrepareVideo,
                Notifications.PlayVideo,
                Notifications.TogglePause,
                Notifications.StopVideo,
                Notifications.StartVideoSeek,
                Notifications.VideoSeek,
                Notifications.EndVideoSeek
            };
        }

        public override void HandleNotification(INotification notification)
        {
            switch (notification.Name)
            {
                case PreferencesMediator.Notifications.PreferencesSelected:
                    Preferences = Facade.RetrieveProxy<PreferencesProxy>();
                    View.Initialize(Preferences.UseVr);
                    break;
                case Notifications.PrepareVideo:
                    var progress = (int)notification.Body;
                    var languageKey = (Preferences.IsDanish) ? "da" : "en";
                    View.Prepare((Preferences.UseVr) ? Urls.VrVideos[languageKey].SafeGet(progress) : Urls.NormalVideos[languageKey].SafeGet(progress));
                    break;
                case Notifications.PlayVideo:
                    View.PlayVideo();
                    break;
                case Notifications.TogglePause:
                    var paused = View.TogglePause();
                    SendNotification((paused) ? Notifications.VideoPaused : Notifications.VideoResumed);
                    break;
                case Notifications.StopVideo:
                    View.StopVideo();
                    break;
                case Notifications.StartVideoSeek:
                    View.StartSeek();
                    break;
                case Notifications.VideoSeek:
                    View.SeekTo((float)notification.Body);
                    break;
                case Notifications.EndVideoSeek:
                    View.EndSeek();
                    break;
            }
        }
    }
}
