using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Windows.Kinect;

public class JointTolerance
{

    public JointType jointType;
    public double toleranceX;
    public double toleranceY;
    public double toleranceZ;

    public JointTolerance(JointType _jointType, double _toleranceX = 0, double _toleranceY = 0, double _toleranceZ = 0)
    {
        jointType = _jointType;
        toleranceX = _toleranceX;
        toleranceY = _toleranceY;
        toleranceZ = _toleranceZ;
    }



}
