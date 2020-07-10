using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJump : MonoBehaviour
{
	[Header("Stats")]
	[SerializeField] private float jumpPower;
	[SerializeField] private float jumpHeight;
	[SerializeField] private float minJumpHeight;
	private float needJumpHeight;
	private float needMinJumpHeight;
	[Range(0f, 0.2f)]
	[SerializeField] private float coyoteTime;
	private float coyoteTimer;
	private bool canJump = false;
	public bool CanJump { get { return canJump; } set { canJump = value; } }
	private bool isJump = false;
	public bool IsJump { get { return isJump; } set { isJump = value; } }

	[Header("Scripts")]
	private PlayerController playerController;

	private Rigidbody2D rb;
    private void Start()
    {
		rb = GetComponent<Rigidbody2D>();
		playerController = GetComponent<PlayerController>();
		coyoteTimer = coyoteTime;
	}
    public void Jump()
	{

		Vector2 jumpForce = new Vector2(0, jumpPower);

		if (playerController.IsGrounded)
		{
			canJump = true;
			coyoteTimer = coyoteTime;
			needMinJumpHeight = transform.position.y + minJumpHeight;
			needJumpHeight = transform.position.y + jumpHeight;
		}
		if (!playerController.IsGrounded)
		{
			if (coyoteTimer > 0)
			{
				coyoteTimer -= Time.deltaTime;
			}

			else if (!isJump && coyoteTimer <= 0)
			{
				canJump = false;
			}
		}


		if (transform.position.y < needMinJumpHeight - Mathf.Epsilon && isJump && !playerController.isCeiling /*&& !jetpack.isFly*/) // прыгать в любом случае пока герой не достиг минимальной высоты 
		{
			rb.AddForce(jumpForce, ForceMode2D.Impulse);
			isJump = true;
		}
		else
		{
			isJump = false;
		}

		if (Input.GetButton("Jump") && canJump)
		{
			if (transform.position.y < needJumpHeight - Mathf.Epsilon && !playerController.isCeiling) // прыгать пока герой не достиг максимальной высоты
			{
				isJump = true;
				if (transform.position.y >= needMinJumpHeight - Mathf.Epsilon)
				{
					rb.AddForce(jumpForce, ForceMode2D.Impulse);
				}
			}

			else
			{
				isJump = false;
				canJump = false;
			}
		}

		else if (Input.GetButtonUp("Jump"))
		{
			canJump = false;
		}

	}
}
