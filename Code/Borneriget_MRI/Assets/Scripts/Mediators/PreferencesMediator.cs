using PureMVC.Patterns.Mediator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Borneriget.MRI
{

    public class PreferencesMediator : Mediator
    {
        public PreferencesMediator() : base(NAME) {}

        public new static string NAME = "PreferencesMediator";

        private PreferencesView View => (PreferencesView)ViewComponent;
        private PreferencesProxy Preferences;

        public static class Notifications {
            public const string PreferencesSelected = "PreferencesSelected";
        }

        public override void OnRegister()
        {
            base.OnRegister();

            if (!Facade.HasProxy<PreferencesProxy>())
            {
                Facade.RegisterProxy(new PreferencesProxy());
            }
            Preferences = Facade.RetrieveProxy<PreferencesProxy>();
            InitializeView();
        }

        public override void OnRemove()
        {
            View.LanguageSelected -= View_LanguageSelected;
            View.AvatarSelected -= View_AvatarSelected;
            View.FormatSelected -= View_FormatSelected;
            View.Hide();
            base.OnRemove();
        }

        private void InitializeView()
        {
            // The preferences menu should already be present in the scene. Just get it.
            ViewComponent = Object.FindObjectOfType<PreferencesView>(true);
            View.LanguageSelected += View_LanguageSelected;
            View.AvatarSelected += View_AvatarSelected;
            View.FormatSelected += View_FormatSelected;
            View.Show();
        }

        private void View_LanguageSelected(string language)
        {
            Preferences.Language = language;
        }

        private void View_AvatarSelected(PreferencesProxy.Avatars avatar)
        {
            Preferences.Avatar = avatar;
        }

        private void View_FormatSelected(bool useVr)
        {
            Preferences.UseVr = useVr;
            Facade.SendNotification(FaderMediator.Notifications.StartFade, Notifications.PreferencesSelected);
        }
    }
}
