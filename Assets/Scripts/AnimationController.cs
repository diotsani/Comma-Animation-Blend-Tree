using System.Collections;
using UnityEngine;

public enum AnimationsState
{
    Idle,
    Move,
    EndMove,
    Interact,
}
public class AnimationController : MonoBehaviour
{
    public Move move;
}