using System;
using UnityEngine;
using System.Collections;
using System.IO;
using QRCodeReaderAndGenerator;
using UnityEngine.UI;

public class QRCodeGeneratorExample : MonoBehaviour {

	[SerializeField]
	InputField inputField;

	[SerializeField]
	RawImage image;

	[SerializeField]
	Texture2D icon;

	Texture2D qrCode;

	ZXing.BarcodeFormat codeFormat = ZXing.BarcodeFormat.QR_CODE;

	void OnEnable()
	{
		QRCodeManager.onError += QRCodeManager_onError;

		// Set output texture size
		QRCodeManager.Instance.SetOutputTextureSize (512, 512); // default is 256x256

		// Set overlay icon
		QRCodeManager.Instance.SetOverlayIcon (icon);

		// set invert qr code
		QRCodeManager.Instance.Inverted = false;
	}

	void QRCodeManager_onError (string err)
	{
		Debug.Log (err);
	}

	public void GenerateQRCode()
	{
		if (inputField && image) {
			qrCode = QRCodeManager.Instance.GenerateQRCode (inputField.text, codeFormat);
			image.texture = qrCode;
		} else {
			Debug.Log ("Assign Input Field or Image.");
		}
	}

	public void SaveQRCode()
	{
		if (qrCode) {
			IOClass.SaveQRCodeToGallery(qrCode);
		} else {
			Debug.Log ("No QR code to save.");
		}
	}

	public void GenerateQRCodePayloads_WIFI()
	{
		if (image) {
			qrCode = QRCodeManager.Instance.GenerateQRCode_WiFi ("Wifi-SSID", "myPassword", WIFI_Authentication.WPA);
			image.texture = qrCode;
		} else {
			Debug.Log ("Assign Image.");
		}
	}


	public void GenerateQRCodePayloads_SMS()
	{
		if (image) {
			qrCode = QRCodeManager.Instance.GenerateQRCode_SMS ("123123123", "Message", SMS_Encoding.SMS);
			image.texture = qrCode;
		} else {
			Debug.Log ("Assign Image.");
		}
	}

	public void GenerateQRCodePayloads_SkypeCall()
	{
		if (image) {
			qrCode = QRCodeManager.Instance.GenerateQRCode_SkypeCall ("skypename");
			image.texture = qrCode;
		} else {
			Debug.Log ("Assign Image.");
		}
	}

	public void GenerateQRCodePayloads_PhoneCall()
	{
		if (image) {
			qrCode = QRCodeManager.Instance.GenerateQRCode_PhoneNumber ("123123123");
			image.texture = qrCode;
		} else {
			Debug.Log ("Assign Image.");
		}
	}

	public void GenerateQRCodePayloads_MMS()
	{
		if (image) {
			qrCode = QRCodeManager.Instance.GenerateQRCode_MMS ("123123123","Message",MMS_Encoding.MMS);
			image.texture = qrCode;
		} else {
			Debug.Log ("Assign Image.");
		}
	}

	public void GenerateQRCodePayloads_Mail()
	{
		if (image) {
			qrCode = QRCodeManager.Instance.GenerateQRCode_Mail ("abc@gmail.com","Test Message","Message from QRCode",Mail_Encoding.MAILTO);
			image.texture = qrCode;
		} else {
			Debug.Log ("Assign Image.");
		}
	}


	// Next scene
	public void OnPayloadGeneratorClick()
	{
		UnityEngine.SceneManagement.SceneManager.LoadScene (1);
	}

	public void OnStringQRGeneratorClick()
	{
		UnityEngine.SceneManagement.SceneManager.LoadScene (0);
	}

	public void OnQRCodeScannerClick()
	{
		UnityEngine.SceneManagement.SceneManager.LoadScene (2);
	}

	public void OnGenerateQRCode()
	{
		codeFormat = ZXing.BarcodeFormat.QR_CODE;
	}

	public void OnGenerateEAN_13()
	{
		codeFormat = ZXing.BarcodeFormat.EAN_13;
	}

	public void OnGenerateCODE_39()
	{
		codeFormat = ZXing.BarcodeFormat.CODE_39;
	}

	public void OnGenerateCODE_128()
	{
		codeFormat = ZXing.BarcodeFormat.CODE_128;
	}

	public void OnGenerateITF()
	{
		codeFormat = ZXing.BarcodeFormat.ITF;
	}

	public void OnGenerateUPC_A()
	{
		codeFormat = ZXing.BarcodeFormat.UPC_A;
	}


}
