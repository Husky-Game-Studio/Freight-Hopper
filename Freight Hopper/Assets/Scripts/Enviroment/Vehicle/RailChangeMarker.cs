using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This is for when a train is switching paths and still needs to keep its carts on the preivous paths
public class RailChangeMarker
{
    private Queue<Cart> carts = new Queue<Cart>();
    private TrainRailLinker previousLinker;
    private TrainRailLinker nextLinker;
    private int nextLinkerIndexEntry;

    public bool Completed => carts.Count < 1;

    public RailChangeMarker(LinkedList<Cart> carts, TrainRailLinker previousLinker, TrainRailLinker nextLinker)
    {
        foreach (Cart cart in carts)
        {
            this.carts.Enqueue(cart);
        }
        this.previousLinker = previousLinker;
        this.nextLinker = nextLinker;
    }

    public void UpdateMarker()
    {
        CheckIfEnteringNextLinker();
    }

    private void CheckIfEnteringNextLinker()
    {
        if (carts.Count < 1)
        {
            return;
        }
        Cart cart = carts.Peek();
        //Debug.Log("running rail change marker");
        if (cart == null || cart.rb == null)
        {
            carts.Dequeue();
            return;
        }

        if (previousLinker.WithinFollowDistance(previousLinker.pathCreator.path.localPoints.Length - 1, cart.rb.position)
            || nextLinker.WithinFollowDistance(0, cart.rb.position))
        {
            previousLinker.RemoveLink(cart.rb);
            //Debug.Log("removed link");
            if (nextLinker != previousLinker)
            {
                nextLinker.Link(cart.rb);
            }

            carts.Dequeue();
        }
    }
}