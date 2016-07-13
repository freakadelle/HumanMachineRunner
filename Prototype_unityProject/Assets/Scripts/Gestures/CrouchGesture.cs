using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Windows.Kinect;
using Assets.Scripts;

public class CrouchGesture : MyGesture {

    public CrouchGesture()
    {
        MinIntervalCap = 15;
        name = "crouch";

        //Add some jointTolerances
        tolerances = new List<JointTolerance>();
        tolerances.Add(new JointTolerance(JointType.Head, 0, -0.15, 0));
    }

    public override bool validate(Body _act, Body _ref)
    {
        
        for (int i = 0; i < tolerances.Count; i++)
        {
            JointType jointType = tolerances[i].jointType;

            if (_act.Joints[jointType].Position.Y - _ref.Joints[jointType].Position.Y < tolerances[i].toleranceY)
            {
                return true;
            }
        }

        MinInterval--;
        return false;
    }

    public override void trigger()
    {
        MinInterval = MinIntervalCap;
        //AvatarStateMachine.AvatarMoveState = AvatarStateMachine.AvatarMove.Ducking;
        //Debug.Log("Gesture: " + name + " triggered!");
    }

}
