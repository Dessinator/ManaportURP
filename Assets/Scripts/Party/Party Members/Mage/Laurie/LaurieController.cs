using UnityEngine;
using Manapotion.Utilities;

namespace Manapotion.PartySystem.LaurieCharacter
{
    public class LaurieController : CharacterController
    {
        private Laurie _laurie;
        // private LaurieCasting _casting;
        private LaurieAbilities _abilities;

        // private LauriePartyInput _partyInput;
        // private LauriePlayerInput _playerInput;

        public LaurieController(Laurie laurie)
        {
            _laurie = laurie;
            
            rb = _laurie.GetComponent<Rigidbody2D>();
            gameManager = GameStateManager.Instance;
            
            // _casting = _laurie.laurieCasting;
            _abilities = _laurie.laurieAbilities;

            // _partyInput = new LauriePartyInput(_laurie);
            // _playerInput = new LauriePlayerInput(_laurie);

            provider = _laurie.inputProvider;

            provider.OnPrimary += OnPrimary_PrimaryCast;
            provider.OnSecondary += OnSecondary_SecondaryCast;
            provider.OnAux += OnAuxMove_AuxillaryMovement;

            Party.OnPartyLeaderChanged += () => 
            {
                if (Party.Instance.previousLeader == _laurie.gameObject) 
                {
                    movementDirection = Vector2.zero; 
                }
            };
        }

        public void Update()
        {
            // _partyInput.Update();
            movementDirection = provider.inputState.movementDirection;
            _isSprinting = provider.inputState.isSprinting;

            Move(Time.fixedDeltaTime);
        }

        private void Move(float d)
        {
            if (gameManager.state != GameState.Main)
            {
                return;
            }

            if (movementDirection.Equals(Vector2.zero)) 
            {
                _laurie.movementState = MovementState.Idle;
                sprintDuration = 0f;
                isDashing = false;
                return;
            }

            if (_isSprinting)
            {
                if (sprintDuration >= _laurie.dashThreshold) 
                {
                    _laurie.movementState = MovementState.Dash;
                    isDashing = true;
                    movementSp = ManaMath.DexCalc_MoveSp(_laurie.statsManagerScriptableObject.GetStat(Stats.StatID.DEX).value.modifiedValue) * ManaMath.DexCalc_DshMod(_laurie.statsManagerScriptableObject.GetStat(Stats.StatID.DEX).value.modifiedValue);
                }
                else
                {
                    _laurie.movementState = MovementState.Sprint;
                    movementSp = ManaMath.DexCalc_MoveSp(_laurie.statsManagerScriptableObject.GetStat(Stats.StatID.DEX).value.modifiedValue) * ManaMath.DexCalc_SprMod(_laurie.statsManagerScriptableObject.GetStat(Stats.StatID.DEX).value.modifiedValue);
                    isDashing = false;
                }
                sprintDuration += Time.deltaTime;
            }
            else
            {
                _laurie.movementState = MovementState.Walk;
                movementSp = ManaMath.DexCalc_MoveSp(_laurie.statsManagerScriptableObject.GetStat(Stats.StatID.DEX).value.modifiedValue);
                Debug.Log(movementSp);
                sprintDuration = 0f;
            }

            position = _laurie.gameObject.transform.position;

            angle = (float)(Mathf.Atan2(movementDirection.y, movementDirection.x));

            reconstructedMovement = new Vector2(Mathf.Cos(angle) * movementSp, Mathf.Sin(angle) * movementSp);
            
            rb.MovePosition(new Vector2(position.x, position.y) + ((reconstructedMovement * movementSp) * d));
            resultPosition = _laurie.transform.position;

            targetPosition = new Vector2(position.x, position.y) + ((reconstructedMovement * movementSp) * d);

            targetDelta = targetPosition - initialPosition;
            actualDelta = resultPosition - initialPosition;

            initialPosition = _laurie.transform.position;
        }

        public void OnPrimary_PrimaryCast() 
        {
            if (gameManager.state != GameState.Main)
            {
                return;
            }

            _laurie.PerformMainAction(0);
        }

        public void OnSecondary_SecondaryCast() 
        {
            if (gameManager.state != GameState.Main)
            {
                return;
            }

            _laurie.PerformMainAction(1);
        }

        public void OnAuxMove_AuxillaryMovement() 
        {
            if (gameManager.state != GameState.Main)
            {
                return;
            }

            _abilities.AuxMove();
        }
    }
}

