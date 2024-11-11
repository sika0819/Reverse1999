using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 手势数据
/// </summary>
public class GestureData
{
    private int[] moves;
    private Vector2[] points;
    private Vector2 lastPoint;
    private Rect rect;

    public GestureData(int[] moves, Vector2[] points, Vector2 lastPoint, Rect rect)
    {
        this.moves = moves;
        this.points = points;
        this.lastPoint = lastPoint;
        this.rect = rect;
    }
    /// <summary>
    /// 获取或设置鼠标手势
    /// </summary>
    public int[] Moves
    {
        get
        {
            return this.moves;
        }
        set
        {
            this.moves = value;
        }
    }
    /// <summary>
    /// 获取或设置鼠标点
    /// </summary>
    public Vector2[] Points
    {
        get
        {
            return this.points;
        }
        set
        {
            this.points = value;
        }
    }
    /// <summary>
    /// 获取或设置最后一次鼠标位置
    /// </summary>
    public Vector2 LastPoint
    {
        get
        {
            return this.lastPoint;
        }
        set
        {
            this.lastPoint = value;
        }
    }
    /// <summary>
    /// 获取或设置手势轨迹大小
    /// </summary>
    public Rect Rect
    {
        get
        {
            return this.rect;
        }
        set
        {
            this.rect = value;
        }
    }
}