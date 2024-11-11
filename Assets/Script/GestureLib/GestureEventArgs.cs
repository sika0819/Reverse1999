using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 手势事件参数
/// </summary>
public class GestureEventArgs : EventArgs
{
    /// <summary>
    /// 包含的数据
    /// </summary>
    public string Present { get; set; }
    /// <summary>
    /// 有效性
    /// </summary>
    public int Fiability { get; set; }
}
