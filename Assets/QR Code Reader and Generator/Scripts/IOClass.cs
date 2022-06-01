using UnityEngine;
using System.Runtime.InteropServices;

namespace QRCodeReaderAndGenerator
{
	public class IOClass
	{
		//IOS
#if UNITY_IOS
		[DllImport ("__Internal")]
		private static extern int _SaveImage (string str, bool move);

		public static void SaveQRCodeToGallery(Texture2D qrCode)
		{
			string path = System.IO.Path.Combine (Application.persistentDataPath, "QrCode.jpg");
			byte[] bytes = qrCode.EncodeToJPG ();
			System.IO.File.WriteAllBytes (path, bytes);

			// Call plugin only when running on real device
			if (Application.platform == RuntimePlatform.IPhonePlayer)
			{
				_SaveImage(path,true);
			}
		}
#else
		public static void SaveQRCodeToGallery(Texture2D qrCode)
		{
			string path;
			byte[] bytes = qrCode.EncodeToJPG ();


			// Call plugin only when running on real device
			if (Application.platform == RuntimePlatform.Android)
			{
#if UNITY_ANDROID

				RefreshGalleryWrapper galleryWrapper = GameObject.FindObjectOfType<RefreshGalleryWrapper>();

				if(galleryWrapper != null){
					// check for permission
					if(!galleryWrapper.CheckPermissions())
					{
						galleryWrapper.StoragePermissionRequest();
						return;
					}

					path = galleryWrapper.GalleryPath;
					if (!string.IsNullOrEmpty (path)) {
						path = System.IO.Path.Combine (path, "QRCode_" + System.DateTime.Now.ToFileTime () + ".jpg");
						try{
							System.IO.File.WriteAllBytes (path, bytes);
							Debug.Log ("Saved : "+path);
						}
						catch(System.Exception ex)
						{
							if(QRCodeManager.onErrorInvoker != null)
							{
								QRCodeManager.onErrorInvoker(ex.Message);
							}
						}

						galleryWrapper.RefreshGallery(path);
					}
					else{
						if(QRCodeManager.onErrorInvoker != null)
						{
							QRCodeManager.onErrorInvoker("Gallery path not found. Please check RefreshGalleryWrapper script attached to any gameobject and SDCard write permission in player settings.");
							galleryWrapper.StoragePermissionRequest();
						}
					}
				}
#endif
			}else if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.LinuxPlayer || Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.WindowsEditor) {
				//save to pictures
				path = System.Environment.GetFolderPath (System.Environment.SpecialFolder.MyPictures);
				path = System.IO.Path.Combine (path, "QRCode_" + System.DateTime.Now.ToFileTime () + ".jpg");

				try{
					System.IO.File.WriteAllBytes (path, bytes);
					Debug.Log ("Saved : "+path);
				}
				catch(System.Exception ex)
				{
					if(QRCodeManager.onErrorInvoker != null)
					{
						QRCodeManager.onErrorInvoker(ex.Message);
					}
				}
			}

		}
#endif
	}
}