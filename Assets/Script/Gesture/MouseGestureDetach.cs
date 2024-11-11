using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class MouseGestureDetach : MonoBehaviour {
    float timer;
    public int Interval = 1;//每1秒检测姿势
    Gesture gesture;
    public Transform transformRoot;
    private Vector2 lastPoint;
    // Use this for initialization
    void Start () {
        gesture = new Gesture();
        gesture.AddGesture("S", "432101234", sMatch);
        gesture.AddGesture("O", "432107654", oMatch);
        gesture.AddGesture("right", "0", rightMatch);
        gesture.GestureMatchEvent += new Gesture.GestureMatchDelegate(gesture_GestureMatchEvent);
        gesture.GestureNoMatchEvent += new Gesture.GestureNoMatchDelegate(gesture_NoGestureMatchEvent);
        gesture.StartCapture(Input.mousePosition.x, Input.mousePosition.y);//鼠标测试
    }

    private void gesture_NoGestureMatchEvent()
    {
        gesture.StartCapture(Input.mousePosition.x, Input.mousePosition.y);//鼠标测试
        Debug.Log("无匹配");
    }

    private void gesture_GestureMatchEvent(GestureEventArgs args)
    {
        Debug.Log(args.Present);
    }

    private int rightMatch(GestureInfos infos)
    {
        return -1;
    }

    private int oMatch(GestureInfos infos)
    {
        if (infos.Data.Rect.height == 0)
            return 10000;
        double py = (infos.Data.LastPoint.y - infos.Data.Rect.y) / (infos.Data.Rect.height);
        return py > 0.3 ? infos.Cost : 10000;
    }

    private int sMatch(GestureInfos infos)
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < infos.Data.Moves.Length; i++)
        {
            sb.Append(infos.Data.Moves[i]);
        }
        string s = sb.ToString();
        int pos = s.IndexOf("111");
        return pos > -1 ? infos.Cost : 10000;
    }

    // Update is called once per frame
    void Update () {
        timer += Time.deltaTime;
        Vector2 currentPoint = Input.mousePosition;
        gesture.Capturing(currentPoint.x, currentPoint.y);
        lastPoint = currentPoint;
        if (timer > Interval)
        {
            gesture.StopCapture();

            timer = 0;
        }
	}
    private void OnApplicationQuit()
    {
        gesture.StopCapture();
    }
}
