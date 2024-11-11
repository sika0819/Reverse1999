using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 定义的手势数据属性
/// </summary>
class GestureProperties
{
    private string present;
    private int[] moves;
    private MatchHandler.matchHandler handler;

    public GestureProperties(string present, int[] moves, MatchHandler.matchHandler handler)
    {
        this.present = present;
        this.moves = moves;
        this.handler = handler;
    }
    /// <summary>
    /// 鼠标手势显示
    /// </summary>
    public string Present
    {
        get
        {
            return this.present;
        }
        set
        {
            this.present = value;
        }
    }
    /// <summary>
    /// 手势路径
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
    /// 手势条件满足时的回调
    /// </summary>
    public MatchHandler.matchHandler match
    {
        get
        {
            return this.handler;
        }
        set
        {
            this.handler = value;
        }
    }

}
