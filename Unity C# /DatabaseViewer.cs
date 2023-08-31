using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

[DisallowMultipleComponent]
public class DatabaseViewer : MonoBehaviour
{
    UIControl uiControl;

    string serverURL = "http://127.0.0.1:5000";

    void Awake()
    {
        uiControl = GetComponent<UIControl>();
    }

    void Start()
    {
        UpdateData();
    }

    public void UpdateData()
    {
        ClearText();
        StartCoroutine(GetDataFromServer());
    }

    void ClearText()
    {
        uiControl.Name.text = "";
        uiControl.Surname.text = ""; 
        uiControl.ClientCode.text = "";
        uiControl.ContactAmount.text = "";
        uiControl.email.text = "";
        uiControl.Info.text = "";
    }

    IEnumerator GetDataFromServer()
    {
        string getDataURL = serverURL + "/get_clients";

        using (UnityWebRequest request = UnityWebRequest.Get(getDataURL))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string responseJson = request.downloadHandler.text;

                ClientDataListWrapper wrapper = JsonUtility.FromJson<ClientDataListWrapper>("{\"clients\":" + responseJson + "}");

                foreach (ClientData client in wrapper.clients)
                {
                    Debug.Log("Name: " + client.name + ", Surname: " + client.surname + ", Email: " + client.email + ", Client Code: " + client.client_code + ", No. of Linked Contacts: " + client.num_of_contacts);
                    UpdateUI(client);
                }
            }
            else
            {
                Debug.LogError("Error fetching data from server: " + request.error);
                uiControl.Info.text = "Error fetching data from server: " + request.error;
            }
        }
    }

    void UpdateUI(ClientData client)
    {
        uiControl.Name.text += "Name: " + client.name + "\n";
        uiControl.Surname.text += "Surname: " + client.surname + "\n"; // Update surname text
        uiControl.email.text += "Email: " + client.email + "\n";
        uiControl.ClientCode.text += "Client Code: " + client.client_code + "\n";
        uiControl.ContactAmount.text += "No. of Linked Contacts: " + client.num_of_contacts + "\n";
    }
}

[System.Serializable]
public class ClientDataListWrapper
{
    public List<ClientData> clients;
}

[System.Serializable]
public class ClientData
{
    public string name;
    public string surname;
    public string email;
    public string client_code;
    public int num_of_contacts;
}
