using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using UnityEngine.UI;
using System.Text;
using UnityEngine.Networking;
using TMPro;

public class PlayerData
{
    public string playerName;
    public int score;
}

public class xmlsavesend : MonoBehaviour
{
    public TMPro.TMP_InputField Username;
    public TMPro.TMP_InputField HighScore;
    public TextMeshProUGUI  ShowText;

    private PlayerData playerData;

    private void Start()
    {
        playerData = new PlayerData();
    }

    public void SavePlayerData()
    {
        playerData.playerName = Username.text;
        playerData.score = int.Parse(HighScore.text);

        XmlSerializer serializer = new XmlSerializer(typeof(PlayerData));

        FileStream fileStream = new FileStream(Application.dataPath + "/playerdata.xml", FileMode.Create);

        serializer.Serialize(fileStream, playerData);

        fileStream.Close();

        ShowText.text = "Player Name: " + playerData.playerName + "\n" +
                              "Score: " + playerData.score;
    }

    public void SendPlayerDataToServer()
    {
        XmlSerializer serializer = new XmlSerializer(typeof(PlayerData));
        FileStream fileStream = new FileStream(Application.dataPath + "/playerdata.xml", FileMode.Open);
        playerData = (PlayerData)serializer.Deserialize(fileStream);
        fileStream.Close();

        UnityWebRequest webRequest = UnityWebRequest.Post("http://localhost/xml.php", "");
        webRequest.SetRequestHeader("Content-Type", "application/xml");

        string playerDataXml = SerializeObjectToXml(playerData);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(playerDataXml);
        webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);

        StartCoroutine(HandleServerResponse(webRequest));
    }

    private IEnumerator HandleServerResponse(UnityWebRequest webRequest)
    {
        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Server response: " + webRequest.downloadHandler.text);
        }
        else
        {
            Debug.Log("Server error: " + webRequest.error);
        }
    }

    private string SerializeObjectToXml(object obj)
    {
        XmlSerializer serializer = new XmlSerializer(obj.GetType());
        StringWriter stringWriter = new StringWriter();
        serializer.Serialize(stringWriter, obj);
        return stringWriter.ToString();
    }
}