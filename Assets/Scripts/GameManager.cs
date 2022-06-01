using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;


public class PickedScreen
{
    public const string LogIn = "LogIn";
    public const string Register = "Register";
    public const string QRReader = "QR Detector";
    public const string GameSelect = "GameSelect";
}

public class GameManager : MonoBehaviour
{
    private Device deviceConnected;
    private User self;
    public Request request;
    public TCP TCPconexion;
    public GameObject mainCanvas,gamesList, gamePrefab,alertPrefab;
    public InputField email, pass;
    private string screen = PickedScreen.LogIn;

    private Color32 colorSuccesfull = new Color32(0, 214, 0, 100);
    private Color32 colorError = new Color32(255, 41, 0, 100);


    // Start is called before the first frame update
    void Start()
    {
        CheckSelf();
    }

    void OnApplicationQuit()
    {
        TCPconexion.tcpDisconect();
        Debug.Log("Application ending after " + Time.time + " seconds");
    }

    void Alert(string message,Color32 color)
    {
        GameObject alertPrefabInstance = Instantiate(alertPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        alertPrefabInstance.transform.SetParent(mainCanvas.transform, false);
        alertPrefabInstance.GetComponent<Image>().color = color;
        alertPrefabInstance.transform.Find("Text").GetComponent<Text>().text = message;
    }

    public void ChangeScreen(string screenName)
    {
        mainCanvas.transform.Find(screen).gameObject.SetActive(false);
        mainCanvas.transform.Find(screenName).gameObject.SetActive(true);
        screen = screenName;
    }

    #region LogIn & Register
    public void LogIn()
    {
        WWWForm form = new WWWForm();
        form.AddField("email", email.text);
        form.AddField("password", pass.text);
        StartCoroutine(request.Post(form, "http://192.168.1.113:8000/login", checkRequest));
    }
    public void Register()
    {
        Transform registerCanvas = mainCanvas.transform.Find(screen);
        WWWForm form = new WWWForm();
        Debug.Log(registerCanvas.Find("Name").Find("Text").GetComponent<Text>().text + registerCanvas.Find("Email").Find("Text").GetComponent<Text>().text + registerCanvas.Find("Pass").Find("Text").GetComponent<Text>().text);
        form.AddField("name", registerCanvas.Find("Name").Find("Text").GetComponent<Text>().text);
        form.AddField("email", registerCanvas.Find("Email").Find("Text").GetComponent<Text>().text);
        form.AddField("password", registerCanvas.Find("Pass").Find("Text").GetComponent<Text>().text);
        form.AddField("password_confirmation", registerCanvas.Find("Pass").Find("Text").GetComponent<Text>().text);
        
        StartCoroutine(request.Post(form, "http://192.168.1.113:8000/register", checkRequest));
    }

    public void LogOut()
    {
        WWWForm form = new WWWForm();
        StartCoroutine(request.Post(form, "http://192.168.1.113:8000/logout", checkRequest));
        ChangeScreen(PickedScreen.LogIn);
    }

    private void CheckSelf()
    {
        //Check if user is connected
        StartCoroutine(request.getSelfUserCorrutine(CheckSelfCallback));
    }

    private void CheckSelfCallback(User user)
    {
        if (user == null)
        {
            Debug.Log("No hi ha user");
            Alert("No se encuentra tu usuario",colorError);
        }
        else
        {
            Debug.Log("Hi ha user");
            self = user;
            ChangeScreen(PickedScreen.QRReader);
            Alert(String.Format("Bienvenido {0}", user.name),colorSuccesfull);
            Debug.Log(user.name);
        }
    }

    public void checkRequest(bool works,string petition)
    {
        Debug.Log("Post" + petition + " works?" + works);
        if (works)
        {
            switch (petition)
            {
                case "login":
                    CheckSelf();
                    break;
                case "logout":
                    Alert("Te has desconectado", colorSuccesfull);
                    break;
                case "register":
                    CheckSelf();
                    break;
                default:
                    Debug.Log(String.Format("No se ha encontrado caso para {0}",petition));
                    break;
            }
        }
        else
        {
            Alert("Algo ha ido mal", colorError);
        }
    }

    #endregion

    #region GameController
    public string FirstCharToUpper(string input)
    {
        switch (input)
        {
            case null: throw new ArgumentNullException(nameof(input));
            case "": throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input));
            default: return input[0].ToString().ToUpper() + input.Substring(1);
        }
    }
    public void ConnectTCP()
    {
        TCPconexion.tcpConect(deviceConnected.ip);
    }

    public void DeviceDetected(Device device)
    {
        deviceConnected = device;
        try
        {
            ConnectTCP();
        }
        catch (Exception e)
        {
            Debug.Log("Problema intentant de conectar al dispositiu: " + e);
        }

        foreach(Game game in deviceConnected.games)
        {
            gamePrefab = Instantiate(gamePrefab, new Vector3(0, 0, 0), Quaternion.identity);
            gamePrefab.transform.Find("name").GetComponent<Text>().text = FirstCharToUpper(game.name);
            gamePrefab.transform.SetParent(gamesList.transform,false);
            gamePrefab.GetComponent<Button>().onClick.AddListener(delegate { openGame(game); });
            StartCoroutine(request.GetImageURL(game.urlcover,gamePrefab.GetComponent<Image>()));
            StartCoroutine(request.GetImageURL(game.urllogo, gamePrefab.transform.Find("logoimage").GetComponent<Image>()));
            Debug.Log(game.urlcover);
        }

        ChangeScreen(PickedScreen.GameSelect);
    }
 
    void openGame(Game game)
    {
        Debug.Log("Trying to open " + game); 
        TCPconexion.tcpSend(String.Format("{0}|{1}|{2}", game.name, game.pathexe, "megadrive"));
    }

    #endregion
    

}
