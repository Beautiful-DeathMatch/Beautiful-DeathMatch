using System;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
#endif

namespace StarterAssets
{
	public class ThirdPersonController : MonoBehaviour
    {
		// animation IDs
        int GroundedHash = Animator.StringToHash("Grounded");
        int JumpHash = Animator.StringToHash("Jump");
        int FreeFallHash = Animator.StringToHash("FreeFall");
		int CurrentItemIndexHash = Animator.StringToHash("CurrentItemIndex");

		private PlayerInputAsset inputAsset;
		private Animator animator;

        private const float _threshold = 0.01f;

        public event Action onHoldInteract = null;
        public event Action onCancelInteract = null;

        public event Action<int> onClickNumber = null;
        public event Action onClickUse = null;

        public event Action<float, float> onRotate = null;
        public event Action onJump = null;
        public event Action<bool, bool, Vector2> onMove = null;

        public event Func<bool> IsInWater;

        public event Func<bool> IsNotYetJump;
        public event Func<bool> IsFallTimeout;
        public event Func<bool> IsGrounded;
        public event Func<bool> IsUIOpened;

		private bool isInteracting = false;

        public void SetAnimatorController(RuntimeAnimatorController controller, Avatar avatar)
		{
			animator = GetComponent<Animator>();
			animator.runtimeAnimatorController = controller;
            animator.avatar = avatar;

			CharacterControllerState.Initialize(animator);
		}

		public void SetInput(PlayerInputAsset inputAsset)
        {
            this.inputAsset = inputAsset;

			SetInputActions();
		}

		private void OnEnable()
		{
			SetInputActions();
		}

		private void OnDisable()
		{
			ClearInputActions();
		}

		private void ClearInputActions()
		{
			if (inputAsset != null)
			{
				inputAsset.onUseItem -= OnUseItem;
				inputAsset.onJump -= OnJump;
				inputAsset.onClickNumber -= OnInputNumber;
			}
		}

		private void SetInputActions()
		{
			ClearInputActions();

			if (inputAsset != null)
			{
				inputAsset.onUseItem += OnUseItem;
				inputAsset.onJump += OnJump;
				inputAsset.onClickNumber += OnInputNumber;
			}			
		}

		private void Update()
        {
            if (inputAsset)
            {
				// 상시 적용이라 얘넨 스테이트로 안 가도 됨
				CheckGrounded();
				CheckFall();

                // 얘네 가도 됨
				CheckMove();
				CheckInteract();
				CheckSwim();
			}
        }

        private void LateUpdate()
        {
            if (inputAsset)
			{
                // 얘도 가도 됨
				CheckRotation();
			}
		}

        private void CheckGrounded()
        {
            bool isGrounded = IsGrounded();
            if (isGrounded)
            {
				animator.SetBool(FreeFallHash, false);
			}

			animator.SetBool(GroundedHash, isGrounded);
			animator.SetBool(JumpHash, !isGrounded);
		}

        private void CheckRotation()
        {
            float yaw = 0.0f;
            float pitch = 0.0f;

            if (inputAsset.lookDir.sqrMagnitude >= _threshold)
            {
				yaw += inputAsset.lookDir.x; // * Time.deltaTime;
				pitch += inputAsset.lookDir.y; // * Time.deltaTime;
			}

			onRotate?.Invoke(yaw, pitch);
		}

        private void CheckMove()
        {
			if (IsUIOpened())
				return;

            onMove?.Invoke(inputAsset.isSprint, inputAsset.analogMovement, inputAsset.moveDir);
        }

        private void CheckFall()
        {
			if (IsGrounded() == false)
			{
				if (IsFallTimeout())
				{
					animator.SetBool(FreeFallHash, true);
				}
            }
        }

        private void CheckSwim()
        {

		}

		private void CheckInteract()
		{
			if (inputAsset.isInteract && !IsUIOpened())
			{
                onHoldInteract?.Invoke();
                isInteracting = true;
			}
			else if (isInteracting)
			{
				onCancelInteract?.Invoke();
				isInteracting = false;
			}
		}

		// InputSystem에서 Jump 버튼이 입력되면 함수가 실행된다.
		private void OnJump()
		{
			if (IsUIOpened())
				return;

			if (IsGrounded())
			{
				if (IsNotYetJump())
				{
					onJump?.Invoke();
					animator.SetBool(JumpHash, true);
				}
			}
		}

		private void OnInputNumber(int number)
        {
			if (IsUIOpened())
				return;

			onClickNumber?.Invoke(number);
			animator.SetInteger(CurrentItemIndexHash, number);
		}

		/// <summary>
		/// Left Click은 기획상 공격이 아니라 '아이템 사용' 입니다.
		/// Gun, Knife를 장착하고 있을 때 해당 아이템을 사용한다면 총알이 나가는 형태로 로직을 구성합니다.
		/// </summary>
		private void OnUseItem()
        {
			if (IsUIOpened())
				return;

			onClickUse?.Invoke();
		}
	}
} 