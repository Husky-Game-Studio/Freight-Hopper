using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatsTransitions : MonoBehaviour
{
    public class toRunTransition : StateTransition {
        public bool shouldTransition(FiniteStateMachineCenter machineCenter) {
            PlayerMachineCenter playerMachine = (PlayerMachineCenter)machineCenter;
            return UserInput.Input.Move() != Vector3.zero && playerMachine.abilities.movementBehavior.Unlocked;
        }
    }

    public class toIdleTransition : StateTransition {
        public bool shouldTransition(FiniteStateMachineCenter machineCenter) {
            PlayerMachineCenter playerMachine = (PlayerMachineCenter)machineCenter;
            return UserInput.Input.Move() == Vector3.zero;
        }
    }

    public class toJumpTransition : StateTransition {
        public bool shouldTransition(FiniteStateMachineCenter machineCenter) {
            PlayerMachineCenter playerMachine = (PlayerMachineCenter)machineCenter;
            return false;//(jumpPressed || playerMachine.jumpBufferTimer.TimerActive()) 
                    //&& !playerMachine.abilities.jumpBehavior.Consumed && playerMachine.abilities.jumpBehavior.Unlocked);
        }
    }

    public class toFallTransition : StateTransition {
        public bool shouldTransition(FiniteStateMachineCenter machineCenter) {
            PlayerMachineCenter playerMachine = (PlayerMachineCenter)machineCenter;
            return UserInput.Input.Move() != Vector3.zero && playerMachine.abilities.movementBehavior.Unlocked;
        }
    }
}
