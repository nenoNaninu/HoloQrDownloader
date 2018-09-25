using System;
using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Unity.Buttons;
using HoloToolkit.Unity.InputModule;
using HoloToolkit.UX.Dialog;    
using UnityEngine;

#if ENABLE_WINMD_SUPPORT
using Windows.Web.Http;
using Windows.Storage;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
#endif

namespace Neno.Scripts
{

    public class FileDonwloader : MonoBehaviour, IInputClickHandler
    {

        enum DownLoadStateEnum
        {
            DetectQr,
            CheckUrl,
            Dowloading,
        }

        [SerializeField]
        private Dialog dialogPrefab = null;

        private bool isDialogLaunched = false;

        private DownLoadStateEnum downLoadState = DownLoadStateEnum.DetectQr;

        /// <summary>
        /// ダウンロード先のurl。コルーチンの使い勝手が悪いので仕方なくメンバに。
        /// </summary>
        private string downloadUrl = null;

        public void OnInputClicked(InputClickedEventData eventData)
        {
            if (isDialogLaunched == false)
            {
                HoloCameraManager.Instance.TakePhotoAsync();
            }
        }

        // Use this for initialization
        void Start()
        {
            StartQrRead();
        }

        void CaptureQrCode(List<byte> bytes)
        {
            int imgWidth = HoloCameraManager.Instance.Resolution.width;
            int imgHeight = HoloCameraManager.Instance.Resolution.height;
            string qrString = QrReader.Decode(bytes.ToArray(), imgWidth, imgHeight);
            CheckUrl(qrString);
        }

        /// <summary>
        /// ユーザが正しいurl(変なurlか)か確認するところ。
        /// </summary>
        /// <param name="url"></param>
        void CheckUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                StartCoroutine(LaunchDialog(DialogButtonType.OK, "Try again", "url is null or empty\n take photo again"));
            }
            else
            {
                downloadUrl = url;
                StartCoroutine(LaunchDialog(DialogButtonType.Yes | DialogButtonType.No, "download from url below?", "Would you like to download from\n" + url));
            }
        }

        public void StartQrRead()
        {
            InputManager.Instance.AddGlobalListener(gameObject);
            HoloCameraManager.Instance.TakeImageAction += CaptureQrCode;
        }

        public void StopQrRead()
        {
            InputManager.Instance.RemoveGlobalListener(gameObject);
            HoloCameraManager.Instance.TakeImageAction -= CaptureQrCode;
        }


        protected IEnumerator LaunchDialog(DialogButtonType buttons, string title, string message)
        {
            isDialogLaunched = true;

            //Open Dialog by sending in prefab
            Dialog dialog = Dialog.Open(dialogPrefab.gameObject, buttons, title, message);

            if (dialog != null)
            {
                //listen for OnClosed Event
                dialog.OnClosed += OnClosed;
            }

            // Wait for dialog to close
            while (dialog.State < DialogState.InputReceived)
            {
                yield return null;
            }

            //only let one dialog be created at a time
            isDialogLaunched = false;

            yield break;
        }



        private void OnButtonClicked(GameObject obj)
        {
            if (isDialogLaunched == false)
            {
                // Launch Dialog with two buttons
                StartCoroutine(LaunchDialog(DialogButtonType.Yes | DialogButtonType.No, "download from url below?", "Would you like to download from" + downloadUrl));
            }
        }

        protected void OnClosed(DialogResult result)
        {
            if (result.Result == DialogButtonType.Yes)
            {
                DownLoadFromUrl();
            }
        }

        async void DownLoadFromUrl()
        {
#if ENABLE_WINMD_SUPPORT
            using (var client = new HttpClient())
            {
                try
                {
                    var responses = await client.GetAsync(new Uri(downloadUrl));

                    var fileName = responses.Content.Headers.ContentDisposition.FileName;
                    StorageFolder storageFolder = KnownFolders.Objects3D;
                    StorageFile downloadFile = await storageFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
                    
                    using (var stream = await downloadFile.OpenStreamForWriteAsync())
                    {
                        stream.SetLength(0);
                        var tmp = await responses.Content.ReadAsBufferAsync();
                        stream.Write(tmp.ToArray(), 0, (int)tmp.Length);
                    }
                }
                catch (Exception ex)
                {
                }
            }
#endif
        }
    }
}


