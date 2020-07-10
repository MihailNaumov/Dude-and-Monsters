using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	private float gravity;
	public float Gravity { get { return gravity; } set { gravity = value; } }
	private float walkGravity = 100f;
	private float airGravity = 20f;
	private bool isWalkGrav = false;
	private Vector2 gravityNormal;
	

	[Range(0f, 5f)]
	[SerializeField] private float fallMultiplier;
	public float FallMultiplier { get { return fallMultiplier; } set { fallMultiplier = value; } }
	[Range(0f, 5f)]
	[SerializeField] private float lowJumpMultiplier;

	public bool IsGrounded { get { return isGrounded; } set { isGrounded = value; } }

	[SerializeField] public bool isGrounded = false;
	[HideInInspector] public bool isCeiling = false;
	
	private Rigidbody2D rb;
	public LayerMask whatIsGround;

	[Header("Scripts")]
	[SerializeField] private PlayerCollisions controller;
	[SerializeField] private Jetpack jetpack;
	[SerializeField] private PlayerStates playerStates;
	[SerializeField] private PlayerMovement playerMovement;
	[SerializeField]private PlayerSlopeMove playerSlopeMove;
	[SerializeField] private PlayerJump playerJump;

	[HideInInspector] public Vector2 newVelocity;
	public Vector2 NewVelocity
	{ 
		get 
		{
			return newVelocity;
		}
		set 
		{
			rb.velocity = value;
			newVelocity = value;
		} 
	}

	
	void Start () {
		rb = GetComponent<Rigidbody2D>();


		playerStates = PlayerStates.Instance;
		playerMovement = GetComponent<PlayerMovement>();
		playerSlopeMove = GetComponent<PlayerSlopeMove>();
		jetpack = GetComponent<Jetpack>();
		playerJump = GetComponent<PlayerJump>();
		controller = GetComponent<PlayerCollisions>();
	}

	void FixedUpdate()
	{
		IsOnGround();
		IsCeiling();
		playerSlopeMove.SlopeCheck();
		
		jetpack.Fly(jetpack.FlySpeed);
		jetpack.FlyRotation(jetpack.RotationSpeed, jetpack.NormalizeRotationSpeed);
		jetpack.Fuel = jetpack.SetFuel(jetpack.Fuel);
		ChooseMoveType();
		ChooseState();

		NewVelocity = playerMovement.Move(rb, playerSlopeMove.slopeNormalPerp);
		playerJump.Jump();

		if (playerStates.curMoveType == PlayerStates.MoveType.SlopeMove && Mathf.Abs(rb.velocity.x) > 1f)
        {
			PlayerGravity(playerSlopeMove.hitGround);
		}
		else
        {
			PlayerGravity(Vector2.up);
		}		
	}


	void ChooseMoveType()
    {
		if (jetpack.isFly)
		{
			playerStates.curMoveType = PlayerStates.MoveType.FlyMove;
		}
		else if (playerJump.IsJump || !isGrounded)
		{
			playerStates.curMoveType = PlayerStates.MoveType.AirMove;
		}
		else if (playerSlopeMove.isOnSlope)
		{
			playerStates.curMoveType = PlayerStates.MoveType.SlopeMove;
		}
		else if (isGrounded)
		{
			playerStates.curMoveType = PlayerStates.MoveType.GroundMove;
		}
	}
	void ChooseState()
    {
		if (jetpack.isFly)
		{
			playerStates.curState = PlayerStates.State.Fly;
		}
		else if(playerJump.IsJump)
        {
			playerStates.curState = PlayerStates.State.Jump;
		}
		else if(isGrounded)
        {
			playerStates.curState = PlayerStates.State.Grounded;
		}
		else
        {
			playerStates.curState = PlayerStates.State.InAir;
		}
	}		



	public void PlayerGravity(Vector2 direction)
	{
		if (isWalkGrav)
		{
			gravity = walkGravity;
		}
		else
		{
			gravity = airGravity;
		}
		if (rb.velocity.y <= 0)
		{
			rb.AddForce(direction * -Mathf.Pow(gravity, 2) * fallMultiplier * Time.deltaTime, ForceMode2D.Impulse);
		}
		else if (rb.velocity.y > 0 && !playerJump.IsJump)
		{
			rb.AddForce(direction * -Mathf.Pow(gravity, 2)  * lowJumpMultiplier * Time.deltaTime, ForceMode2D.Impulse);
		}

	}

	/// <summary>
	/// Возвращает true, если герой коснулся земли
	/// </summary>
	/// <returns></returns>
	
	public bool IsOnGround()
	{
		bool ground = false;
		bool walkGrav = false;

		for (int i = 0; i < controller.verticalRayCount; i++)
		{
			float directionX = Mathf.Sign(rb.velocity.x);
			Vector2 rayOrigin = (directionX == -1) ? controller.raycastOrigins.bottomLeft : controller.raycastOrigins.bottomRight;
			rayOrigin += Vector2.right * (controller.verticalRaySpacing * -directionX * i );
			RaycastHit2D hit = Physics2D.Raycast(rayOrigin, -Vector2.up, 30f, whatIsGround);

			if (hit.distance <= 0.1f)
			{
				ground = true;
			}

			if(!playerJump.IsJump)
            {
				if (i == controller.verticalRayCount - 2 && isGrounded)
				{
					if (hit.distance <= 2f)
					{
						walkGrav = true;
					}
					else
					{
						walkGrav = false;
					}
				}
			}
			else
            {
				walkGrav = false;
            }
			
		}
		isWalkGrav = walkGrav;
		return IsGrounded = ground;
	}

	/// <summary>
	/// Возвращает true, если герой коснулся потолка
	/// </summary>
	/// <returns></returns>
	public bool IsCeiling()
	{
		bool celling = false;
		for (int i = 0; i < controller.verticalRayCount; i++)
		{
			Vector2 rayOrigin = controller.raycastOrigins.topLeft;
			rayOrigin += Vector2.right * (controller.verticalRaySpacing * i + (rb.velocity.x * Time.deltaTime));
			RaycastHit2D hit = Physics2D.Raycast(rayOrigin, -Vector2.up, 10f, whatIsGround);
			if (hit.distance <= 0.05f)
			{
				celling = hit;
			}

		}
		return isCeiling = celling;
	}
	
	
}
