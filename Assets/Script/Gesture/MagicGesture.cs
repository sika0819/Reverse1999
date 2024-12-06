using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class Magicm_gesture : MonoBehaviour {
    [SerializeField]
    float timer;
    public int Interval = 1;//每1秒检测姿势
    public float StartDetectTime = 0.01f;
    

    Gesture m_gesture;
    public GameObject m_trail;
    public GameObject click;
    Animator clickAnimator;
    TrailRenderer trailRender;
 

    private Vector2 m_lastPoint;

    public string showImgPath = "showImg";
    public ShowImage imageChanger;
    Texture[] textures;
    
    void Awake(){
        trailRender = m_trail.GetComponent<TrailRenderer>();
        trailRender.enabled = false;
        trailRender.Clear();
        clickAnimator = click.GetComponent<Animator>();
        textures = Resources.LoadAll<Texture>(showImgPath);
    }
    // Use this for initialization
    void Start()
    {
        m_gesture = new Gesture();
        m_gesture.AddGesture("valentina", "062", wynMatch);
        m_gesture.AddGesture("holfman", "7", wynMatch);
        m_gesture.AddGesture("semmelweis", "6", wynMatch);
        m_gesture.AddGesture("bela", "2", wynMatch);
        m_gesture.AddGesture("kakaniya", "7130", wynMatch);
        m_gesture.AddGesture("kakaniya", "0130", wynMatch);
        m_gesture.AddGesture("kakaniya", "0230", wynMatch);
        m_gesture.AddGesture("isolde", "3", wynMatch);
        m_gesture.AddGesture("macus", "4", wynMatch);
        m_gesture.AddGesture("lorelei", "5", wynMatch);
        m_gesture.AddGesture("Item","1",wynMatch);
        m_gesture.AddGesture("Random","17",randomMatch);
        m_gesture.GestureMatchEvent += new Gesture.GestureMatchDelegate(gesture_GestureMatchEvent);
        m_gesture.GestureNoMatchEvent += new Gesture.GestureNoMatchDelegate(gesture_NoGestureMatchEvent);
       
    }

    private void gesture_NoGestureMatchEvent()
    {
       Debug.Log("gesture_NoGestureMatchEvent");
       imageChanger.SwitchShowerImage(showImgPath+"/2399");
    }
    int randomCost = 0;
    int wynCost = 1;
    private void gesture_GestureMatchEvent(GestureEventArgs args)
    {  
        Debug.LogFormat("Present{0},randomCost{1},wynCost{2}",args.Present,randomCost,wynCost);
        if(randomCost>wynCost){
           int randNum = Random.Range(0,textures.Length);
           imageChanger.SwitchShowerImage(showImgPath+"/"+textures[randNum].name);
        }else{
           Debug.LogFormat("gesture_GestureMatchEvent{0}",args.Present);
           imageChanger.SwitchShowerImage(showImgPath+"/"+args.Present);
        }
    }
    private int wynMatch(GestureInfos infos)
    { 
        wynCost = infos.Cost;
        return infos.Cost;
    }
    private int randomMatch(GestureInfos infos){
        randomCost = infos.Cost;
        return infos.Cost;
    }
    // Update is called once per frame
    void Update()
    {
        SetMousePos();
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began){
               BeginGesture();
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                 DetachGesture();
            }else if(touch.phase == TouchPhase.Ended){
                StopGesture();
            }
            
        }else{
            if (Input.GetMouseButtonDown(0)){
                BeginGesture();
                //触发鼠标姿态检测
            }else  if (Input.GetMouseButton(0))
            {
               DetachGesture();
            }else if(Input.GetMouseButtonUp(0)){
              StopGesture();
            }
        }
    }
    void StopAnim(){
        imageChanger.ResetAnim();
    }
    void BeginGesture(){
        StopAnim();
        trailRender.Clear();
        m_gesture.StartCapture(Input.mousePosition.x, Input.mousePosition.y);//触发触摸手势检测
        click.SetActive(true);
        click.transform.position = SetMousePos();
        clickAnimator.SetTrigger("click");
    }
    void DetachGesture(){
        timer += Time.deltaTime;
        
        if(timer > StartDetectTime){
            trailRender.enabled = true;
            Vector3 currentPoint  = SetMousePos();
            m_gesture.Capturing(currentPoint.x, currentPoint.y);
            m_lastPoint = currentPoint;
            if (timer > Interval)
            {
                m_gesture.StopCapture();
                timer = 0;
            } 
        }   
                                                                        
    }
    Vector3 SetMousePos(){
        Vector3 currentPoint = Vector3.zero;
       
        if(Input.GetMouseButton(0)){
            currentPoint = Input.mousePosition;
        }
        else if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            currentPoint = touch.position;
        }
        currentPoint.z = 10;//z不同,转换的坐标也不同
        m_trail.transform.position = Camera.main.ScreenToWorldPoint(currentPoint);
        return currentPoint;
    }
    void StopGesture(){
        m_gesture.StopCapture();
        timer = 0;
        trailRender.Clear();
        trailRender.enabled = false;
    }
    private void OnApplicationQuit()
    {
        m_gesture.StopCapture();
    }
}
