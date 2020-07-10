using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSlopeMove : MonoBehaviour
{
	[SerializeField] private float slopeCheckDistance;
	private float slopeSideAngle;
	private float slopeDownAngle;
	private float slopeDownAngleOld;

	[HideInInspector] public bool isOnSlope;

	[HideInInspector] public Vector2 slopeNormalPerp;
	private Vector2 colliderSize;

	public LayerMask whatIsGround;

	private new BoxCollider2D collider;
	[HideInInspector] public Vector2 hitGround;

	[Header ("Scripts")]
	[SerializeField] private PlayerController playerController;
	[SerializeField] private PlayerStates playerStates;

	private void Start()
    {
		playerController = GetComponent<PlayerController>();
		playerStates = PlayerStates.Instance;
		collider = GetComponent<BoxCollider2D>();
		colliderSize = collider.size;
	}

    /// <summary>
    /// Поведение героя на наклонных поверхностях
    /// </summary>
    /// <returns></returns>
    /// 
    public void SlopeCheck()
	{
		Vector2 checkPos = transform.position - new Vector3(0.0f, (colliderSize.y) / 2 + -PlayerCollisions.skinWidth);
		SlopeCheckVertical(checkPos);
		SlopeCheckHorizontal(checkPos);
	}
	int lastMove;
	void SlopeCheckVertical(Vector2 checkPos)
	{
		float skinWidth;
		int moveX = Math.Sign(playerController.newVelocity.x);


		if (moveX == 0)
		{
			skinWidth = PlayerCollisions.skinWidth;
		}
		else
		{
			skinWidth = PlayerCollisions.skinWidth * 2;
		}

		RaycastHit2D hitMiddle = Physics2D.Raycast(checkPos, Vector2.down, slopeCheckDistance, whatIsGround);
		RaycastHit2D hitLeft = Physics2D.Raycast(checkPos - new Vector2(colliderSize.x / 2 + skinWidth, -skinWidth * 4), Vector2.down, slopeCheckDistance, whatIsGround);
		RaycastHit2D hitRight = Physics2D.Raycast(checkPos + new Vector2(colliderSize.x / 2 + skinWidth, skinWidth * 4), Vector2.down, slopeCheckDistance, whatIsGround);
		RaycastHit2D hit;
		
		

		lastMove = moveX != 0 ? moveX : lastMove;

		hit = (lastMove > 0 && hitRight) ? hitRight : hitLeft;

		if (!hit)
		{
			hit = hitMiddle;
		}

		hitGround = hit.normal;
		slopeNormalPerp = Vector2.Perpendicular(hit.normal).normalized;
		slopeDownAngle = Vector2.Angle(hit.normal, Vector2.up);
		if (slopeDownAngle != slopeDownAngleOld)
		{
			isOnSlope = true;
		}

		slopeDownAngleOld = slopeDownAngle;

		Debug.DrawRay(hit.point, hit.normal, Color.yellow);
		Debug.DrawRay(hit.point, slopeNormalPerp, Color.red);

	}

	void SlopeCheckHorizontal(Vector2 checkPos)
	{
		RaycastHit2D slopeHitFront = Physics2D.Raycast(checkPos, transform.right, slopeCheckDistance, whatIsGround);
		RaycastHit2D slopeHitBack = Physics2D.Raycast(checkPos, -transform.right, slopeCheckDistance, whatIsGround);
		if (slopeHitFront)
		{
			isOnSlope = true;
			slopeSideAngle = Vector2.Angle(slopeHitFront.normal, Vector2.up);
		}
		else if (slopeHitBack)
		{
			isOnSlope = true;
			slopeSideAngle = Vector2.Angle(slopeHitBack.normal, Vector2.up);
		}
		else
		{
			slopeSideAngle = 0.0f;
			isOnSlope = false;
		}
	}
}