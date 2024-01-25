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

#if ENABLE_INPUT_SYSTEM
        private PlayerInput _playerInput;
#endif
		private StarterAssetsInputs _input;

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

		public void SetInput(PlayerInput inputComponent, StarterAssetsInputs inputAsset)
        {
			_playerInput = inputComponent;
            _input = inputAsset;
		}

        private void Update()
        {
            if(_input && _playerInput)
            {
				CheckGrounded(); // 상시 적용이라 스테이트로 안 가도 됨

                // 얘넨 가도 될 듯
				CheckJump();
				CheckMove();
                CheckItem();
                CheckAttack();
                CheckInteract();
			}
        }

        private void LateUpdate()
        {
            if (_input && _playerInput)
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

            if (_input.look.sqrMagnitude >= _threshold)
            {
                yaw += _input.look.x;
                pitch += _input.look.y;
			}

			onRotate?.Invoke(yaw, pitch);
		}

        public void CheckMove()
        {
            onMove?.Invoke(_input.sprint, _input.analogMovement, _input.move);
        }

        private void CheckJump()
        {
            if (IsGrounded())
            {
                if (_input.jump && IsNotYetJump())
                {
                    onJump?.Invoke();
					_animator.SetBool(_animIDJump, true);
				}
            }
            else
            {
				if (IsFallTimeout())
				{
					_animator.SetBool(_animIDFreeFall, true);
				}

				_input.jump = false;
            }
        }

        private void Swim()
        {
            if (IsInWater() == false)
                return;

			_animator.SetBool(_animIDSwim, true);
		}

        // 플레이어의 아이템 상태를 관리합니다. 즉, 손에 들고 있을 것의 번호
        // 0: 기본, 1: 총, 2: 칼, 3:아이템
        void CheckItem()
        {
            if (_animator)
            {
                if (_input) // 키보드 0 입력시
                {
                    _animator.SetInteger(_animIDItemOffset, 0);
                }
                else if (_input) // 키보드 1 입력시 ...
                {
                    _animator.SetInteger(_animIDItemOffset, 1);
                }
                else if (_input)
                {
                    _animator.SetInteger(_animIDItemOffset, 2);
                }
                else if (_input)
                {
                    _animator.SetInteger(_animIDItemOffset, 3);
                }
            }
        }

        private void CheckInteract()
        {
            if(_input.interact)
            {
                if (IsInteracting?.Invoke() == false)
                {
					onPressInteract?.Invoke();
                    _input.interact = false;
				}
			}
		}

        void CheckAttack()
        {
    //        if (_input.attack)
    //        {
    //            if (_animator)
    //            {
    //                _animator.SetBool(_animIDAttack, true);
    //                _input.attack = false;
				//}
    //        }
        }
    }
}