using System.Collections.Generic;
using System;

public class WanderState : BasicState
{
    private TrainMachineCenter trainFSM;

    public WanderState(FiniteStateMachineCenter machineCenter, List<Func<BasicState>> stateTransitions) : base(machineCenter, stateTransitions)
    {
        this.trainFSM = (TrainMachineCenter)machineCenter;
    }

    public override void PerformBehavior()
    {
        trainFSM.Follow(trainFSM.carts.First.Value.rb.transform.forward + trainFSM.carts.First.Value.rb.transform.position);
    }

    public override BasicState TransitionState()
    {
        BasicState state = CheckTransitions();
        return state;
    }
}