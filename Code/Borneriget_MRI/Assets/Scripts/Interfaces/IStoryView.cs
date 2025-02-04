﻿using System;

namespace Borneriget.MRI
{
    public interface IStoryView
    {
        public void Initialize(bool isDanish, string doneNotification);
        public void Show(int room, string doneNotification, bool avatarAwake);
        public void Hide();
        public void ShowVideo();
        public void ShowSpinner();
        public void SetVideoProgress(VideoProgress progress);

        public event Action<int> SelectRoom;
        public event Action Exit;
    }
}