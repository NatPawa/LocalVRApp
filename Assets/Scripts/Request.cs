using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;
using Newtonsoft.Json;
using System;
using System.Net.Sockets;
using UnityEngine.UI;

public class Request : MonoBehaviour
{

    [SerializeField]
    GameManager gm;

    // Start is called before the first frame update
    void Start()
    {
    }



    #region GetData
    public void getDevice(string deviceName)
    {
        StartCoroutine(getDeviceCorrutine(deviceName));
    }

    IEnumerator getDeviceCorrutine(string deviceName)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(String.Format("http://192.168.1.113:8000/api/devices/{0}", deviceName)))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            try
            {
                switch (webRequest.result)
                {
                    case UnityWebRequest.Result.ConnectionError:
                    case UnityWebRequest.Result.DataProcessingError:
                        Debug.LogError(": Error: " + webRequest.error);
                        break;
                    case UnityWebRequest.Result.ProtocolError:
                        Debug.LogError(": HTTP Error: " + webRequest.error);
                        break;
                    case UnityWebRequest.Result.Success:
                        string result = webRequest.downloadHandler.text;
                        Debug.Log(result);
                        gm.DeviceDetected(JsonConvert.DeserializeObject<Device>(result));
                        break;
                }
            }
            catch (Exception e)
            {
                Debug.Log(e);
                throw;
            }
           

        }
    }

    public IEnumerator getSelfUserCorrutine(System.Action<User> callbackOnFinish)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get("http://192.168.1.113:8000/me"))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            try
            {
                switch (webRequest.result)
                {
                    case UnityWebRequest.Result.ConnectionError:
                    case UnityWebRequest.Result.DataProcessingError:
                        Debug.LogError(": Error: " + webRequest.error);
                        break;
                    case UnityWebRequest.Result.ProtocolError:
                        Debug.LogError(": HTTP Error: " + webRequest.error);
                        break;
                    case UnityWebRequest.Result.Success:
                        string result = webRequest.downloadHandler.text;
                        Debug.Log(result);
                        //gm.selfUser(JsonConvert.DeserializeObject<User>(result));
                        callbackOnFinish(JsonConvert.DeserializeObject<User>(result));
                        break;
                }
            }
            catch (Exception e)
            {
                Debug.Log(e);
                throw;
            }


        }
    }

    #endregion

    #region GetImages
    public IEnumerator GetImageURL(string url, Image img)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture("http://192.168.1.113:8000/" + url);
        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError)
        {
            Debug.LogWarning(request.error);
            yield break;
        }

        var texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
        var sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), Vector2.one * 0.5f);
        img.sprite = sprite;
    }

    public IEnumerator DownloadImage(string MediaUrl, RawImage RW)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(MediaUrl);
        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError)
            Debug.Log(request.error);
        else
            RW.texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
    }
    #endregion

    public IEnumerator Post(WWWForm form,string url, System.Action<bool,string> callbackOnFinish)
    {

        UnityWebRequest wwwToken = UnityWebRequest.Get("http://192.168.1.113:8000/token");
        yield return wwwToken.SendWebRequest();

        if (wwwToken.isNetworkError || wwwToken.isHttpError)
        {
            Debug.LogError(wwwToken.error);
        }
        else
        {
            string csrf_token = wwwToken.downloadHandler.text;
            form.AddField("_token", csrf_token);
            using (UnityWebRequest www = UnityWebRequest.Post(url, form))
            {
                yield return www.SendWebRequest();

                if (www.result != UnityWebRequest.Result.Success)
                {
                    Debug.Log(www.error);
                    callbackOnFinish(false,url.Split('/')[url.Split('/').Length - 1]);
                }
                else
                {
                    Debug.Log("Form upload complete!");
                    Debug.Log(www);
                    callbackOnFinish(true, url.Split('/')[url.Split('/').Length - 1]);
                }
            }
        }
    }
}


[Serializable]
public class Device
{
    public int id { get; set; }
    public string QRName { get; set; }
    public string type { get; set; }
    public string state { get; set; }
    public string ip { get; set; }
    public string created_at { get; set; }
    public string updated_at { get; set; }
    public List<Game> games { get; set; }

}


[Serializable]
public class User
{
    public int id { get; set; }
    public string name { get; set; }
    public string email { get; set; }
    public List<Partida> partidas { get; set; }
}

[Serializable]
public class Partida
{
    public int id { get; set; }
    public string code { get; set; }
    public string state { get; set; }
    public List<User> users { get; set; }
}

[Serializable]
public class Game
{
    public int id { get; set; }
    public string name { get; set; }
    public string description { get; set; }
    public string pathexe { get; set; }
    public string urllogo { get; set; }
    public string urlcover { get; set; }
}