using UnityEngine;
using System.Collections;
using QRCodeReaderAndGenerator;

namespace QRCodeReaderAndGenerator
{
public class RefreshGalleryWrapper : MonoBehaviour {

	#if UNITY_ANDROID
	 
	private const string STORAGE_READ_PERMISSION = "android.permission.READ_EXTERNAL_STORAGE";
	private const string STORAGE_WRITE_PERMISSION = "android.permission.WRITE_EXTERNAL_STORAGE";
	private const string CAMERA_PERMISSION = "android.permission.CAMERA";

	void Start()
	{
		if (Application.platform == RuntimePlatform.Android) {
			StoragePermissionRequest ();
		}
		
	}

	void SetGalleryPath()
	{
		if (Application.platform == RuntimePlatform.Android) {
			if (Application.platform == RuntimePlatform.Android) {
				AndroidJavaClass javaClass = new AndroidJavaClass ("com.astricstore.androidutil.AndroidGallery");
				javaClass.CallStatic ("SetGalleryPath", gameObject.name);
			}
		}
	}

	public void StoragePermissionRequest()
	{
		if (Application.platform == RuntimePlatform.Android) {
			AndroidPermissionsManager.RequestPermission (new [] {
				STORAGE_READ_PERMISSION,
				STORAGE_WRITE_PERMISSION,
				CAMERA_PERMISSION
			}, new AndroidPermissionCallback (
				grantedPermission =>
				{
					// The permission was successfully granted, restart the change avatar routine
					SetGalleryPath ();
				},
				deniedPermission =>
				{
					// The permission was denied.
					// Show in-game pop-up message stating that the user can change permissions in Android Application Settings
					// if he changes his mind (also required by Google Featuring program)
				}));
		}
	}


	string androidImagePath = "";
	void AndroidGalleryPath(string androidPath)
	{
		androidImagePath = androidPath;
		if (!System.IO.Directory.Exists (androidImagePath)) {
			System.IO.Directory.CreateDirectory (androidImagePath);
		}
	}

	void OnError(string err)
	{
		if (QRCodeManager.onErrorInvoker != null)
			QRCodeManager.onErrorInvoker (err);
	}

	public string GalleryPath
	{
		get{
			return androidImagePath;
		}
	}

	public void RefreshGallery(string path)
	{
		if (Application.platform == RuntimePlatform.Android) {
			AndroidJavaClass javaClass = new AndroidJavaClass ("com.astricstore.androidutil.AndroidGallery");
			javaClass.CallStatic ("RefreshGallery", path);
		}
	}

	public bool CheckPermissions()
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return true;
		}
		
		return  AndroidPermissionsManager.IsPermissionGranted (STORAGE_READ_PERMISSION) && 
				AndroidPermissionsManager.IsPermissionGranted (STORAGE_WRITE_PERMISSION) && 
				AndroidPermissionsManager.IsPermissionGranted (CAMERA_PERMISSION);
	}
		
		
		#endif
	}
}
