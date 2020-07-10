using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class PlayerMovement : MonoBehaviour
{
	[HideInInspector] public bool isMove;

	[Header ("Stats")]
	[SerializeField] private float speed;
	[SerializeField] private float startMoveTime;
	[SerializeField] private float stopMoveTime;
	private float movement;
	[Space]
	[SerializeField] private float airStartMoveTime;
	[SerializeField] private float airStopMoveTime;
	[SerializeField] private float airMoveCof;
	[Space]
	[SerializeField] private float jetpackStartMoveTime;
	[SerializeField] private float jetpackStopMoveTime;
	private enum LastKey { Left, Right };


	[SerializeField]  private LastKey lastKey;

	[Header("Scripts")]
	[SerializeField] private PlayerStates playerStates;
	private void Start()
    {
		playerStates = PlayerStates.Instance;
    }
    float Movement(float startTime, float stopTime )
	{
		float move = movement;

		float h = Input.GetAxis("Horizontal");
		float left = Input.GetAxisRaw("Left");
		float right = Input.GetAxisRaw("Right");

		float posMove = Mathf.Lerp(move, 1, startTime / 100); // движение вправо
		float negMove = Mathf.Lerp(move, -1, startTime / 100); // движение влево
		float endMove = Mathf.Lerp(move, 0, stopTime / 100); // остановка


        if (left != 0 && right == 0)
        {
            lastKey = LastKey.Left;
			movement = negMove;
		}
        else if (left == 0 && right != 0)
        {
            lastKey = LastKey.Right;
			movement = posMove;
		}
		 
		if (lastKey == LastKey.Left && right != 0)
		{
			movement = posMove;
		}
		else if (lastKey == LastKey.Right && left != 0)
		{
			movement = negMove;
		}


		if (h == 0)
		{
			isMove = false;

			if (movement <= 0.1f && movement >= -0.1f)
			{
				movement = 0;
			}
			else
			{
				movement = endMove;
			}
		}
		else
        {
			isMove = true;
		}
		return movement;
	}

	public Vector2 Move(Rigidbody2D rb, Vector2 slopeNormalPerp)
	{
		Vector2 rbVelocity = rb.velocity;

		float movement;
		float startTime = 0;
		float stopTime = 0;
		var curMove = playerStates.curMoveType;


		if (curMove == PlayerStates.MoveType.GroundMove || curMove == PlayerStates.MoveType.SlopeMove)
		{
			startTime = startMoveTime;
			stopTime = stopMoveTime;
		}
		else if (curMove == PlayerStates.MoveType.FlyMove)
		{
			startTime = jetpackStartMoveTime;
			stopTime = jetpackStopMoveTime;
		}
		else if (curMove == PlayerStates.MoveType.AirMove)
		{
			startTime = airStartMoveTime;
			stopTime = airStopMoveTime;
		}


		movement = Movement(startTime, stopTime);

		rbVelocity = new Vector2(movement * speed, rbVelocity.y);

		if (curMove == PlayerStates.MoveType.GroundMove)
		{
			rbVelocity = new Vector2(movement * speed, 0);
		}
		else if (curMove == PlayerStates.MoveType.SlopeMove)
		{
			rbVelocity = new Vector2 (speed * slopeNormalPerp.x * -movement, speed * slopeNormalPerp.y * -movement);
			if(movement == 0)
            {
				rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
			}
			else
            {
				rb.constraints = RigidbodyConstraints2D.FreezeRotation;
			}
		}
		else if (curMove == PlayerStates.MoveType.AirMove)
		{
			rbVelocity = new Vector2(movement * airMoveCof * speed, rbVelocity.y);
		}

		if(curMove != PlayerStates.MoveType.SlopeMove)
		{
			rb.constraints = RigidbodyConstraints2D.FreezeRotation;
		}

		return rbVelocity;
	}
}