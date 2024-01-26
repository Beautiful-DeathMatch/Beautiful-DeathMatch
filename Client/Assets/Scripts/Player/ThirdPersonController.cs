using System;
using System.Collections.Generic;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM 
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
    public class ThirdPersonController : MonoBehaviour
    {
		// animation IDs
        int _animIDGrounded = Animator.StringToHash("Grounded");
        int _animIDJump = Animator.StringToHash("Jump");
        int _animIDFreeFall = Animator.StringToHash("FreeFall");

        int _animIDItemOffset = Animator.StringToHash("ItemOffset");
        int _animIDAttack = Animator.StringToHash("Attack");
        int _animIDSwim = Animator.StringToHash("Swim");

		private PlayerInputAsset inputAsset;

		private Animator _animator;

        private const float _threshold = 0.01f;

        public event Action onPressInteract = null;
        public event Action offPressInteract = null;

        public event Action<int> onClickNumber = null;
        public event Action onClickUse = null;

        public event Action<float, float> onRotate = null;
        public event Action onJump = null;
        public event Action<bool, bool, Vector2> onMove = null;

        public event Func<bool> IsInWater;
        public event Func<bool> IsInteracting;

        public event Func<bool> IsNotYetJump;
        public event Func<bool> IsFallTimeout;
        public event Func<bool> IsGrounded;

        public void SetAnimatorController(RuntimeAnimatorController controller, Avatar avatar)
		{
			_animator = GetComponent<Animator>();
			_animator.runtimeAnimatorController = controller;
            _animator.avatar = avatar;

			CharacterControllerState.Initialize(_animator);
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
				inputAsset.onAttack -= OnInputAttack;
				inputAsset.onInteract -= OnInputInteract;
				inputAsset.onJump -= OnJump;
				inputAsset.onClickNumber -= OnInputNumber;
			}
		}

		private void SetInputActions()
		{
			ClearInputActions();

			if (inputAsset != null)
			{
				inputAsset.onAttack += OnInputAttack;
				inputAsset.onInteract += OnInputInteract;
				inputAsset.onJump += OnJump;
				inputAsset.onClickNumber += OnInputNumber;
			}			
		}

		private void Update()
        {
            if (inputAsset)
            {
				CheckGrounded(); // 상시 적용이라 얘넨 스테이트로 안 가도 됨
				CheckFall();

                // 얘네 가도 됨
				CheckMove();
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
				_animator.SetBool(_animIDFreeFall, false);
			}

			_animator.SetBool(_animIDGrounded, isGrounded);
			_animator.SetBool(_animIDJump, false);
		}

        private void CheckRotation()
        {
            float yaw = 0.0f;
            float pitch = 0.0f;

            if (inputAsset.lookDir.sqrMagnitude >= _threshold)
            {
                yaw += inputAsset.lookDir.x * Time.deltaTime;
                pitch += inputAsset.lookDir.y * Time.deltaTime;
			}

			onRotate?.Invoke(yaw, pitch);
		}

        public void CheckMove()
        {
            onMove?.Invoke(inputAsset.isSprint, inputAsset.analogMovement, inputAsset.moveDir);
        }

        private void OnJump()
        {
			if (IsGrounded())
			{
				if (IsNotYetJump())
				{
					onJump?.Invoke();
					_animator.SetBool(_animIDJump, true);
				}
			}
		}

        private void CheckFall()
        {
			if (IsGrounded() == false)
			{
				if (IsFallTimeout())
				{
					_animator.SetBool(_animIDFreeFall, true);
				}
            }
        }

        private void CheckSwim()
        {
            if (IsInWater() == false)
                return;

			_animator.SetBool(_animIDSwim, true);
		}

        private void OnInputNumber(int number)
        {
			_animator.SetInteger(_animIDItemOffset, number);
		}

        private void OnInputInteract()
        {
			if (IsInteracting?.Invoke() == false)
			{
				onPressInteract?.Invoke();
			}
		}

		private void OnInputAttack()
        {
		    if (_animator)
			{
			    _animator.SetBool(_animIDAttack, true);
			}
		}
	}
}