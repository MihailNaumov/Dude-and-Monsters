using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStates : MonoBehaviour
{
    public enum MoveType { Stop, GroundMove, SlopeMove, AirMove, FlyMove }
    public MoveType curMoveType;
    public enum State { Grounded, Jump, InAir, Fly }
    public State curState;

    public static PlayerStates Instance;
    private void Awake()
    {
        Instance = this;
    }

}
