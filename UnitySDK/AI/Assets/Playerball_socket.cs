using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
public class Playerball_socket : MonoBehaviour
{
    Rigidbody rigid;
    new AudioSource audio;
    public int playerpoint;
    public GameLogic manager;
    bool isjump;

    // Start is called before the first frame update
    private void Awake()
    {
        Debug.Log("새로운 신 등장");
        isjump = false;
        rigid = GetComponent<Rigidbody>();
        audio = GetComponent<AudioSource>();
    }

    [Serializable]
    public class Rewards
    {
        public int value;
    }

    public void PostMinusData()
    {
        Rewards reward_val = new Rewards();
        reward_val.value = -1;
        string json = JsonUtility.ToJson(reward_val);
        StartCoroutine(PostRequest("http://localhost:5000/", json));
    }

    public void PostPlusData()
    {
        Rewards reward_val = new Rewards();
        reward_val.value = 1;
        string json = JsonUtility.ToJson(reward_val);
        StartCoroutine(PostRequest("http://localhost:5000/", json));
    }

    private void Update()
    {
        if (Input.GetButtonDown("Jump") && !isjump)
        {
            isjump = true;
            rigid.AddForce(new Vector3(0, 10, 0), ForceMode.Impulse);
        }

        if (playerpoint == 6)
        {
            PostPlusData();
            SceneManager.LoadScene(0);
        }
    }

    private void FixedUpdate()
    {
        float h = Input.GetAxis("Horizontal") * 0.4f;
        float v = Input.GetAxis("Vertical") * 0.4f;
        rigid.AddForce(new Vector3(h, 0, v), ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Floor")
            isjump = false;
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.tag == "Item")
        {
            playerpoint = playerpoint + 1;
            audio.Play();
            other.gameObject.SetActive(false);
            manager.GetItem(playerpoint);

        }

        if (other.tag == "Fall")
        {
            PostMinusData();
            SceneManager.LoadScene(0);
        }
    }

    IEnumerator PostRequest(string url, string json)
    {
        Debug.Log(json);
        var datas = new UnityWebRequest(url, "POST");
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
        datas.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
        datas.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        datas.SetRequestHeader("Content-Type", "application/json");
        yield return datas.SendWebRequest();

        if (datas.isNetworkError)
        {
            Debug.Log("Error While Sending: " + datas.error);
        }
        else
        {
            Debug.Log("Received: " + datas.downloadHandler.text);
        }
    }
}
