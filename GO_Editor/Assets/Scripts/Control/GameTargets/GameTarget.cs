using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTarget : MonoBehaviour
{
    public virtual bool TargetComplete() { return false; }
    public virtual void Initialize() { }
}
