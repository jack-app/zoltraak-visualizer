using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraControl : MonoBehaviour
{
    [SerializeField] private RawImage rawImage;
    [SerializeField] private Dropdown dropDown;

    WebCamTexture webCamTexture;
    WebCamDevice[] webCamDevices;

    void Start()
    {
        string name;
        this.webCamDevices = WebCamTexture.devices;
        for (int i = 0; i < this.webCamDevices.Length; i++){
            name = this.webCamDevices[i].name.ToString();
            dropDown.options.Add(new Dropdown.OptionData { text = name });
            Debug.Log(this.webCamDevices[i].name);
        }

        if (this.webCamDevices.Length > 0){
            webCamTexture = new WebCamTexture(webCamDevices[0].name, 1920, 1080, 30);
            rawImage.texture = webCamTexture;

            webCamTexture.Play();
            dropDown.value = 0;
        }
    }

    public void onDeviceChanged () {
        if (webCamTexture != null){
            webCamTexture.Stop();
        }
        webCamTexture = new WebCamTexture(webCamDevices[dropDown.value].name, 1920, 1080, 30);
        rawImage.texture = webCamTexture;

        webCamTexture.Play();
    }
    public WebCamTexture GetWebCamTexture()
    {
        return webCamTexture;
    }
}
