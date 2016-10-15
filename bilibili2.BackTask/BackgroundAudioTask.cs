using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Foundation.Collections;
using Windows.Media;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage.Streams;

namespace BackTask
{
    public sealed class BackgroundAudioTask : IBackgroundTask
    {
       
        MediaPlayer mediaplayer = BackgroundMediaPlayer.Current;
        private BackgroundTaskDeferral _deferral; // Used to keep task alive
        private SystemMediaTransportControls _smtc;

        public void Run(IBackgroundTaskInstance taskInstance)
        {
            taskInstance.Canceled += TaskInstance_Canceled;
            BackgroundMediaPlayer.MessageReceivedFromForeground += BackgroundMediaPlayer_MessageReceivedFromForeground;
            mediaplayer.CurrentStateChanged += Mediaplayer_CurrentStateChanged;
            _deferral = taskInstance.GetDeferral();
            _smtc = BackgroundMediaPlayer.Current.SystemMediaTransportControls;
            _smtc.ButtonPressed += _smtc_ButtonPressed;
            _smtc.IsEnabled = true;
           
            _smtc.IsNextEnabled = false;
            _smtc.IsPreviousEnabled = false;
            _smtc.IsPlayEnabled = true;
            _smtc.IsPauseEnabled = true;

            mediaplayer.Play();
        }

        private void BackgroundMediaPlayer_MessageReceivedFromForeground(object sender, MediaPlayerDataReceivedEventArgs e)
        {
           
            if (e.Data != null)
            {
                //mediaplayer.CurrentState= MediaPlayerState.Playing
                string values = (string)e.Data.ToList()[0].Value;
                switch (values)
                {
                    
                    case "Playing":
                        mediaplayer.Play();
                        _smtc.PlaybackStatus = MediaPlaybackStatus.Playing;
                        //_smtc.IsPlayEnabled = false;
                        //_smtc.IsPauseEnabled = true;
                        break;
                    case "Paused":
                        mediaplayer.Pause();
                        _smtc.PlaybackStatus = MediaPlaybackStatus.Paused;
                        //_smtc.IsPlayEnabled = true;
                        //_smtc.IsPauseEnabled = false;
                        break;
                    default:
                        break;
                }
                //SystemMediaTransportControlsDisplayUpdater c = _smtc.DisplayUpdater;
                //c.Update();
                //if (values)
                //{
                //    mediaplayer.Play();
                //    //_smtc.IsPauseEnabled = true;
                //    //_smtc.IsPlayEnabled = false;

                //}
                //else
                //{
                //    mediaplayer.Pause();
                //    //_smtc.IsPauseEnabled = false;
                //    //_smtc.IsPlayEnabled = true;

                //}
            }



        }

        private void Mediaplayer_CurrentStateChanged(MediaPlayer sender, object args)
        {
            try
            {
                _smtc = SystemMediaTransportControls.GetForCurrentView();
            }
            catch (Exception)
            {
                _smtc = mediaplayer.SystemMediaTransportControls;
            }
          

        }

        private void TaskInstance_Canceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            if (_deferral != null)
            {
                _deferral.Complete();
            }
           
        }

        private void _smtc_ButtonPressed(SystemMediaTransportControls sender, SystemMediaTransportControlsButtonPressedEventArgs args)
        {

            try
            {
                switch (args.Button)
                {
                    case Windows.Media.SystemMediaTransportControlsButton.Play:
                        mediaplayer.Play();
                        {
                            ValueSet vs = new ValueSet();
                            vs.Add("Play", "Playing");
                            BackgroundMediaPlayer.SendMessageToForeground(vs);
                        }
                        //_smtc.IsPlayEnabled = false;
                        //_smtc.IsPauseEnabled = true;
                        _smtc.PlaybackStatus = MediaPlaybackStatus.Playing;
                        break;
                    case Windows.Media.SystemMediaTransportControlsButton.Pause:
                        mediaplayer.Pause();
                        {
                            ValueSet vs = new ValueSet();
                            vs.Add("Play", "Paused");
                            BackgroundMediaPlayer.SendMessageToForeground(vs);
                        }
                        _smtc.PlaybackStatus = MediaPlaybackStatus.Paused;
                        // _smtc.IsPlayEnabled = true;
                        //_smtc.IsPauseEnabled = false;
                        break;
                    default:
                        break;
                }
            }
            catch (Exception)
            {

                throw;
            }
           
        }
    }
}
