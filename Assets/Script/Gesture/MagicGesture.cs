using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class Magicm_gesture : MonoBehaviour {
    [SerializeField]
    float timer;
    public int Interval = 1;//每1秒检测姿势

    Gesture m_gesture;

    private Vector2 m_lastPoint;
    // Use this for initialization
    void Start()
    {
        m_gesture = new Gesture();
        m_gesture.AddGesture("-1", "062", wynMatch);
        m_gesture.AddGesture("0", "0", wynMatch);
        m_gesture.AddGesture("0", "713560", wynMatch);
        m_gesture.AddGesture("1", "6", wynMatch);
        m_gesture.AddGesture("1", "2", wynMatch);
        m_gesture.AddGesture("2", "7130", wynMatch);
        m_gesture.AddGesture("3", "713135", wynMatch);
        m_gesture.AddGesture("4", "3052", wynMatch);
        m_gesture.AddGesture("4", "3062", wynMatch);
        m_gesture.AddGesture("5", "272340", wynMatch);
        m_gesture.AddGesture("9","4",randomMatch);
        m_gesture.GestureMatchEvent += new Gesture.GestureMatchDelegate(gesture_GestureMatchEvent);
        m_gesture.GestureNoMatchEvent += new Gesture.GestureNoMatchDelegate(gesture_NoGestureMatchEvent);
       
    }

    private void gesture_NoGestureMatchEvent()
    {
        Debug.Log("无匹配");
    }

    private void gesture_GestureMatchEvent(GestureEventArgs args)
    {
        Debug.Log(args.Present);
    }
    private int wynMatch(GestureInfos infos)
    {
        //Debug.Log(infos.Present);
        return infos.Cost;
    }
    private int randomMatch(GestureInfos infos){
        return infos.Cost;
    }
    // Update is called once per frame
    void Update()
    {
       
        if (Input.touchCount > 0)
        {
        
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began){
                m_gesture.StartCapture(Input.mousePosition.x, Input.mousePosition.y);//触发触摸手势检测
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                Debug.Log("Touch is moving");
                timer += Time.deltaTime;
                Vector2 currentPoint = touch.position;
                m_gesture.Capturing(currentPoint.x, currentPoint.y);
                m_lastPoint = currentPoint;
                if (timer > Interval)
                {
                    m_gesture.StopCapture();

                    timer = 0;
                }
            }
            
        }else{
            if (Input.GetMouseButtonDown(0)){
                m_gesture.StartCapture(Input.mousePosition.x, Input.mousePosition.y);//触发鼠标姿态检测
            }else  if (Input.GetMouseButton(0))
            {
                timer += Time.deltaTime;
                Debug.Log("Pressed left-click.");
                Vector2 currentPoint = Input.mousePosition;
                m_gesture.Capturing(currentPoint.x, currentPoint.y);
                m_lastPoint = currentPoint;
                if (timer > Interval)
                {
                    m_gesture.StopCapture();

                    timer = 0;
                }
            }else if(Input.GetMouseButtonUp(0)){
                m_gesture.StopCapture();
                timer = 0;
            }
        }
    }
    private void OnApplicationQuit()
    {
        m_gesture.StopCapture();
    }
}
