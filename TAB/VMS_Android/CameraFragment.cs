using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using Android.App;
using Android.Content;
using Android.Hardware;
using Android.OS;
using Android.Preferences;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace VMS_Android
{
    public class CameraFragment : Fragment
    {
        static Camera _camera = null;
        FrameLayout _frameLayout;
        bool _cameraReleased = false;
        ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(Application.Context);
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var ignor = base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(Resource.Layout.CameraFragmentLayout, container, false);
            
            var snapButton = view.FindViewById<Button>(Resource.Id.snapButton);
            snapButton.BringToFront();
            snapButton.Click += SnapButtonClick;
            _camera = SetUpCamera();
            _frameLayout = view.FindViewById<FrameLayout>(Resource.Id.camera_preview);
            SetCameraPreview();

            return view;
        }
        /// <summary>
        /// Take a picture.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void SnapButtonClick(object sender, EventArgs e)
        {
            try
            {
                _camera.StartPreview();
                _camera.TakePicture(null, null, new CameraPictureCallBack(Activity));
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public void SnapWithTime()
        {
            try
            {
                _camera.StartPreview();
                _camera.TakePicture(null, null, new CameraPictureCallBack(Activity));
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public void RetakePhoto()
        {
            _camera.StartPreview();
        }

        public override void OnDestroy()
        {
            _camera.StopPreview();
            _camera.Release();
            _cameraReleased = true;
            base.OnDestroy();
        }

        public override void OnResume()
        {
            if (_cameraReleased)
            {
                _camera.Reconnect();
                _camera.StartPreview();
                _cameraReleased = false;
            }
            base.OnResume();
        }

        /// <summary>
        /// Set the Camera Preview, and pass it the current device's Camera
        /// </summary>
        public void SetCameraPreview()
        {
            _frameLayout.AddView(new CameraPreview(Activity, _camera));
        }

        /// <summary>
        /// Get an instace of the current hardware camera of the device
        /// </summary>
        /// <returns></returns>
        /// 
        private void releaseCameraAndPreview()
        {
            if (_camera != null)
            {
                _camera.Release();
                _camera = null;
            }
        }
        Camera SetUpCamera()
        {
            Camera c = null;
            try
            {
                c = Camera.Open(1);

            }
            catch (Exception e)
            {
                Log.Debug("", "Device camera not available now." + e.ToString());
            }

            return c;
        }
    }
}