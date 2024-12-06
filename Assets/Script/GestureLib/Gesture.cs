using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
public class Gesture
{
    #region 常量
    /// <summary>
    /// 鼠标方向扇区
    /// </summary>
    public const int DEFAULT_SECTORS = 8;
    /// <summary>
    /// 最小精确度,以像素表示
    /// </summary>
    public const int DEFAULT_PRECISION = 8;
    /// <summary>
    /// 有效等级,越小越精确,但是对于画出来的图形相似度要求较高
    /// </summary>
    public const int DEFAULT_FIABILITY = 80;
    #endregion

    #region 成员变量
    private List<float> moves;//手势列表     int
    private Vector2 lastPoint;//最后位置
    private List<GestureProperties> gestures;//需要匹配的手势  GestureProperties
    private Rect rect;//手势轨迹大小
    private List<Vector2> points;//轨迹点   Point

    private double sectorRad;//一个扇区的弧度
    private List<int> anglesMap;//弧度向量表  int
    #endregion

    #region 事件
    public delegate void GestureMatchDelegate(GestureEventArgs args);
    public event GestureMatchDelegate GestureMatchEvent;

    public delegate void GestureNoMatchDelegate();
    public event GestureNoMatchDelegate GestureNoMatchEvent;
    #endregion

    public Gesture()
    {
        gestures = new List<GestureProperties>();
        moves = new List<float>();
        points = new List<Vector2>();
        BuildAngleMap();
    }
    /// <summary>
    /// 添加定义的手势
    /// </summary>
    /// <param name="present">手势代表的字符</param>
    /// <param name="gesture">手势数据，8个方向，从右开始为0，顺时针指定</param>
    /// <param name="match">匹配的回调,可以为null</param>
    public void AddGesture(string present, string gesture, MatchHandler.matchHandler match)
    {
        int[] g = new int[gesture.Length];
        char[] gestureArray = gesture.ToCharArray();
        for (int i = 0; i < gesture.Length; i++)
        {
            g[i] = gestureArray[i].ToString().Equals(".") ? -1 : int.Parse(gestureArray[i].ToString());
        }
        gestures.Add(new GestureProperties(present, g, match));
    }
    /// <summary>
    /// 建立弧度表
    /// </summary>
    private void BuildAngleMap()
    {
        sectorRad = Mathf.PI * 2 / DEFAULT_SECTORS;
        anglesMap = new List<int>();

        //精度步进，100
        double step = Mathf.PI * 2 / 100;

        int sector;
        for (double i = -sectorRad / 2; i <= Mathf.PI * 2 - sectorRad / 2; i += step)
        {
            sector = (int)Mathf.Floor((float)((i + sectorRad / 2) / sectorRad));
            anglesMap.Add(sector);
        }
    }
    /// <summary>
    /// 开始手势捕获
    /// </summary>
    /// <param name="mx">鼠标相对于绘图区的X坐标</param>
    /// <param name="my">鼠标相对于绘图区的Y坐标</param>
    public void StartCapture(float mx, float my)
    {
        moves = new List<float>();
        points = new List<Vector2>();

        rect = new Rect(int.MaxValue, int.MinValue, int.MaxValue, int.MinValue);

        lastPoint = new Vector2(mx, my);
    }
    /// <summary>
    /// 鼠标手势捕获中
    /// </summary>
    /// <param name="mx">鼠标相对于绘图区的X坐标</param>
    /// <param name="my">鼠标相对于绘图区的Y坐标</param>
    public void Capturing(float mx, float my)
    {
        float difx = mx - lastPoint.x;
        float dify = my - lastPoint.y;

        float sqDist = difx * difx + dify * dify;
        int sqPrec = (int)(DEFAULT_PRECISION * DEFAULT_PRECISION);
        if (sqDist > sqPrec)
        {
            points.Add(new Vector2(mx, my));
            AddMove(difx, dify);
            lastPoint.x = mx;
            lastPoint.y = my;

            if (mx < rect.xMin)
                rect.xMin = mx;
            if (mx > rect.xMax)
                rect.xMax = mx;
            if (my < rect.yMin)
                rect.yMin = my;
            if (my > rect.yMax)
                rect.yMax = my;
        }
    }
    /// <summary>
    /// 添加手势数据
    /// </summary>
    /// <param name="dx"></param>
    /// <param name="dy"></param>
    private void AddMove(float dx, float dy)
    {
        double angle = Mathf.Atan2(dy, dx) + sectorRad / 2;
        if (angle < 0)
            angle += Mathf.PI * 2;
        int no = (int)Mathf.Floor((float)angle / (Mathf.PI * 2) * 100);//计算在弧度表中对应的向量编号
        moves.Add(anglesMap[no]);
    }
    public void ClearData()
    {
        points.Clear();
    }
    /// <summary>
    /// 停止鼠标手势捕获
    /// </summary>
    public void StopCapture()
    {
        MatchGesture();
    }
    private void MatchGesture()
    {
        int bestCost = 50;
        int cost = 0;
        int[] gest;
        if(moves.Count == 0){
            return;
        }
        int[] imoves = new int[moves.Count];
        for (int i = 0; i < imoves.Length; i++)
        {
            imoves[i] = (int)moves[i];
        }
        Vector2[] ppoints = new Vector2[points.Count];
        for (int i = 0; i < ppoints.Length; i++)
        {
            ppoints[i] = (Vector2)points[i];
        }
        string bestGesture = string.Empty;
        Rect irect = new Rect(rect.xMin, rect.yMin, rect.xMax - rect.xMin, rect.yMax - rect.yMin);
        GestureInfos infos = new GestureInfos(new GestureData(imoves, ppoints, lastPoint, irect));
        GestureProperties bestProperty = gestures[0];
        for (int i = 0; i < gestures.Count; i++)
        {
            gest = (gestures[i] as GestureProperties).Moves;
            infos.Present = (gestures[i] as GestureProperties).Present;
            cost = CostLeven(gest, imoves);
           
            if (cost <= DEFAULT_FIABILITY)
            {
                if ((gestures[i] as GestureProperties).match != null)
                {
                    infos.Cost = cost;
                    cost = (gestures[i] as GestureProperties).match(infos);
                }
                if (cost < bestCost)
                {
                    bestCost = cost;
                    bestProperty = gestures[i] as GestureProperties;
                    bestGesture = (gestures[i] as GestureProperties).Present;
                }
            }
        }
        if(bestProperty!=null){
            StringBuilder stringBuilder = new StringBuilder();
            for(int i=0;i<bestProperty.Moves.Length;i++){
                stringBuilder.Append(bestProperty.Moves[i]);
                stringBuilder.Append(",");
            }
            Debug.LogFormat("stringBuilder:{0}",stringBuilder.ToString());
        }
    
        if (!string.IsNullOrEmpty(bestGesture))
        {
            GestureEventArgs args = new GestureEventArgs();
            args.Present = bestGesture;
            args.Fiability = cost;
            if (GestureMatchEvent != null){
                GestureMatchEvent(args);
            }
            ClearData();
        }
        else
        {
            if (GestureNoMatchEvent != null)
                GestureNoMatchEvent();
            ClearData();
        }
    }
    private int DifAngle(int a, int b)
    {
        int dif = (int)Mathf.Abs(a - b);
        if (dif > DEFAULT_SECTORS / 2)
            dif = DEFAULT_SECTORS - dif;
        return dif;
    }
    /// <summary>
    /// 采用Levenshtein算法计算数组相似度  http://www.merriampark.com/ld.htm
    /// </summary>
    /// <param name="cgest"></param>
    /// <param name="cmoves"></param>
    /// <returns></returns>
    private int CostLeven(int[] cgest, int[] cmoves)
    {
        if (cgest[0] == -1)
        {
            return (int)(cmoves.Length == 0 ? 0 : 100000);
        }
        int[,] d = new int[cgest.Length + 1, cmoves.Length + 1];
        int[,] w = new int[cgest.Length + 1, cmoves.Length + 1];
        int x = 0, y = 0;
        for (x = 1; x <= cgest.Length; x++)
        {
            for (y = 1; y < cmoves.Length; y++)
            {
                d[x, y] = DifAngle((int)cgest[x - 1], (int)cmoves[y - 1]);
            }
        }

        for (y = 1; y <= cmoves.Length; y++)
            w[0, y] = 100000;
        for (x = 1; x <= cgest.Length; x++)
            w[x, 0] = 100000;
        w[0, 0] = 0;

        int cost = 0;
        int above;
        int left;
        int diag;

        for (x = 1; x <= cgest.Length; x++)
        {
            for (y = 1; y < cmoves.Length; y++)
            {
                cost = d[x, y];
                above = w[x - 1, y] + cost;
                left = w[x, y - 1] + cost;
                diag = w[x - 1, y - 1] + cost;
                w[x, y] = Mathf.Min(Mathf.Min(above, left), diag);
            }
        }
        return w[x - 1, y - 1];
    }
}

