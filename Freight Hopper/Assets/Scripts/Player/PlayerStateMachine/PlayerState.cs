using System;
using System.Collections.Generic;

public abstract class PlayerState : BasicState
{
    protected PlayerMachineCenter playerMachineCenter;

    public PlayerState(PlayerMachineCenter playerMachineCenter, List<Func<BasicState>> myTransitions) : base(playerMachineCenter, myTransitions)

    {
        this.playerMachineCenter = playerMachineCenter;
    }
}