
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Hardware;
using Android.Net;
using Android.OS;
using Android.Preferences;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.IO;
using VMS.Core.Class;
using VMS.Core.Model;
using VMS.Core.Repository;

namespace VMS_Android
{
    public class CameraPictureCallBack : Java.Lang.Object, Camera.IPictureCallback
    {
        ISharedPreferences session;
        ISharedPreferencesEditor SessionEdit;
        VisitorRepository _visitor = new VisitorRepository();
        const string APP_NAME = "SimpleCameraApp";
        Context _context;
        ImageView ImagePhoto;
        string PhotoName;
        public CameraPictureCallBack(Context cont)
        {
            _context = cont;
            session = PreferenceManager.GetDefaultSharedPreferences(_context);
            PhotoName = session.GetString(nameof(gVarEnum.CardId), "");
        }

        /// <summary>
        /// Callback when the picture is taken by the Camera
        /// </summary>
        /// <param name="data"></param>
        /// <param name="camera"></param>
        public async void OnPictureTaken(byte[] data, Camera camera)
        {
            
            PhotoAndroid photo = new PhotoAndroid();
            try
            {
                string fileName = Uri.Parse(PhotoName + ".jpg").LastPathSegment;
                var os = _context.OpenFileOutput(fileName, FileCreationMode.WorldReadable);
                System.IO.BinaryWriter binaryWriter = new System.IO.BinaryWriter(os);
                binaryWriter.Write(data);
                binaryWriter.Close();
                photo.photoData = data;
                photo.photoName = fileName;
                var getData = await _visitor.PostVisitorPhoto(photo);
                SessionEdit = session.Edit();
                SessionEdit.PutString(nameof(gVarEnum.Photo), fileName);
                SessionEdit.Apply();
                //We start the camera preview back since after taking a picture it freezes
                //camera.StartPreview();
                DisplayMessage("Success!! The file name is " + fileName);
            }
            catch (System.Exception e)
            {
                Log.Debug(APP_NAME, "File not found: " + e.Message);
            }
        }
        private void DisplayMessage(string message)
        {
            Toast.MakeText(_context, message, ToastLength.Long).Show();
        }
    }
}