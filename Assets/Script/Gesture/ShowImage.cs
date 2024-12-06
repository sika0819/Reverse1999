using UnityEngine;
using UnityEngine.UI;
public class ShowImage : MonoBehaviour
{   
    RawImage imageShower;
    RectTransform rectTranform;
    AspectRatioFitter aspectRatioFitter;
    public  Animator m_Animator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        imageShower = GetComponent<RawImage>();
        rectTranform = GetComponent<RectTransform>();
        aspectRatioFitter = GetComponent<AspectRatioFitter>();
        m_Animator = GetComponent<Animator>();
        aspectRatioFitter.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ResetAnim(){
        m_Animator.SetBool("Show",false);
    }
    public void SwitchShowerImage(string name){
        
        m_Animator.SetBool("Show",true);
        aspectRatioFitter.enabled = false;
        Texture texture = Resources.Load<Texture>(name);
        if(texture == null){
            return;
        }
        rectTranform.anchoredPosition =new Vector2(0f,0f);
        imageShower.texture = texture;
        imageShower.SetNativeSize();
        aspectRatioFitter.enabled = true;
        aspectRatioFitter.aspectRatio =  (float)(imageShower.texture.width)/ imageShower.texture.height;
        aspectRatioFitter.aspectMode = AspectRatioFitter.AspectMode.WidthControlsHeight;
        Debug.LogError(name);
        if(name!="2399"){
            rectTranform.sizeDelta= new Vector2(752,rectTranform.sizeDelta.y);
            rectTranform.transform.localScale = Vector3.one * 0.5f;
        }else{
            rectTranform.sizeDelta= new Vector2(800,rectTranform.sizeDelta.y);
            rectTranform.transform.localScale = Vector3.one;
        }
        
        if(rectTranform.sizeDelta.y * 0.5f <800){
            rectTranform.anchorMin = new Vector2(0.5f,0.5f);
            rectTranform.anchorMax = new Vector2(0.5f,0.5f);
            rectTranform.pivot = new Vector2(0.5f,0.5f);
        }
        else{
            rectTranform.anchorMin = new Vector2(0.5f,1f);
            rectTranform.anchorMax = new Vector2(0.5f,1f);
            rectTranform.pivot = new Vector2(0.5f,1f);
        }
    }
}
