using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text.RegularExpressions;

[DisallowMultipleComponent]
public class DataCapture : MonoBehaviour
{
    UIControl uiControl;

    const string ServerURL = "http://127.0.0.1:5000";

    void Awake()
    {
        uiControl = GetComponent<UIControl>();
    }

    void Start()
    {
        uiControl.Info.text = "";
    }

    public void Submit()
    {
        string name = uiControl.nameField.text.Trim();
        string surname = uiControl.surnameField.text.Trim();
        string email = uiControl.emailField.text.Trim();

        if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(surname) && !string.IsNullOrEmpty(email))
        {
            if (IsValidEmail(email))
            {
                CaptureData(name, surname, email);
                ClearInputs();
                uiControl.Info.text = "";
            }
            else
            {
                uiControl.Info.text = "Invalid email format.";
                Debug.LogError("Invalid email format.");
            }
        }
        else
        {
            uiControl.Info.text = "Name, surname, and email cannot be empty.";
            Debug.LogError("Name, surname, and email cannot be empty.");
        }
    }

    bool IsValidEmail(string email)
    {
        string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$";
        return Regex.IsMatch(email, pattern);
    }

    void ClearInputs()
    {
        uiControl.nameField.text = "";
        uiControl.surnameField.text = "";
        uiControl.emailField.text = "";
    }

    public void CaptureData(string name, string surname, string email)
    {
        StartCoroutine(SendDataToServer(name, surname, email));
    }

    IEnumerator SendDataToServer(string name, string surname, string email)
    {
        WWWForm form = new WWWForm();
        form.AddField("name", name);
        form.AddField("surname", surname); // Add the surname field
        form.AddField("email", email);

        using (UnityWebRequest www = UnityWebRequest.Post(ServerURL, form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Data captured and sent successfully.");
            }
            else
            {
                Debug.LogError("Error sending data to the server: " + www.error);
            }
        }
    }
}