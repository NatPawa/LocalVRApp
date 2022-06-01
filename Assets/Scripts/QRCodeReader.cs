using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using QRCodeReaderAndGenerator;

public class QRCodeReader : MonoBehaviour {

	[SerializeField]
	RawImage rawImage;

	[SerializeField]
	Text text;

	[SerializeField]
	Request rq;

	int CamPick = 0;


	// Use this for initialization
	
	IEnumerator Start()
	{
		ScanQRCode();
		Debug.Log(WebCamTexture.devices.Length);
		text.text = text + "total:"  + WebCamTexture.devices.Length;
		yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
	}


	void OnEnable () {
		QRCodeManager.onError += HandleOnError;
		QRCodeManager.onQrCodeFound += HandleOnQRCodeFound;
	}

	void OnDisable () {
		QRCodeManager.onError -= HandleOnError;
		QRCodeManager.onQrCodeFound -= HandleOnQRCodeFound;
	}

	void HandleOnQRCodeFound (ZXing.BarcodeFormat barCodeType, string barCodeValue)
	{
		Debug.Log (barCodeType + " __ " + barCodeValue);
		rq.getDevice(barCodeValue);
	}

	void HandleOnError (string err)
	{
		Debug.LogError (err);
	}
		
	public void ScanQRCode()
	{
		if(rawImage)
		{
            try
			{
				text.text = WebCamTexture.devices[CamPick].name;
				QRCodeManager.CameraSettings camSettings = new QRCodeManager.CameraSettings();
				string rearCamName = WebCamTexture.devices[CamPick].name;
				if (rearCamName != null)
				{
					camSettings.deviceName = rearCamName;
					camSettings.maintainAspectRatio = true;
					camSettings.makeSquare = true;
					camSettings.requestedWidth = 1280;
					camSettings.requestedHeight = 720;
					camSettings.scanType = ScanType.CONTINUOUS;
					QRCodeManager.Instance.ScanQRCode(camSettings, rawImage, 1f);
				}
			}
            catch (System.Exception e)
            {
				text.text = e.ToString();
            }
			
		}
        if (CamPick < WebCamTexture.devices.Length - 1)
        {
			CamPick++;

        }
        else
        {
			CamPick = 0;
        }
	}

	// this function is require to call to stop scanning when camSettings.scanType = ScanType.CONTINUOUS;
	// no need to call when camSettings.scanType = ScanType.ONCE;
	public void StopScanning()
	{
		QRCodeManager.Instance.StopScanning ();
	}

	string GetRearCamName()
	{
		foreach (WebCamDevice device in WebCamTexture.devices) {
			Debug.Log(device.name);
			if (!device.isFrontFacing) {
				return device.name;
			}
		}
		return null;
	}
}
