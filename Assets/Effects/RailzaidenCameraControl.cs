using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RailzaidenCameraControl : MonoBehaviour
{
    [SerializeField] private RawImage rawImage;
    [SerializeField] private RawImage rawImage2;
    // Start is called before the first frame update
    void Start()
    {
        WebCamTexture webCamTexture = GameObject.Find("CameraManager").GetComponent<CameraControl>().GetWebCamTexture();
        rawImage.texture = webCamTexture;
        rawImage2.texture = webCamTexture;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
