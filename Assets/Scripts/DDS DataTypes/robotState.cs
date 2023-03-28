
/*
WARNING: THIS FILE IS AUTO-GENERATED. DO NOT MODIFY.

This file was generated from robotState.idl
using RTI Code Generator (rtiddsgen) version 3.1.0.
The rtiddsgen tool is part of the RTI Connext DDS distribution.
For more information, type 'rtiddsgen -help' at a command shell
or consult the Code Generator User's Manual.
*/

using System;
using System.Reflection;
using System.Collections.Generic;
using Rti.Types;
using System.Linq;
using Omg.Types;

public class RobotState :  IEquatable<RobotState>
{
    public double J1 { get; set; }

    public double J2 { get; set; }

    public double J3 { get; set; }

    public double J4 { get; set; }

    public double J5 { get; set; }

    public double J6 { get; set; }

    public double X { get; set; }

    public double Y { get; set; }

    public double Z { get; set; }

    public double W { get; set; }

    public double P { get; set; }

    public double R { get; set; }

    public RobotState()
    {
    }

    public RobotState(double  J1, double  J2, double  J3, double  J4, double  J5, double  J6, double  X, double  Y, double  Z, double  W, double  P, double  R)
    {
        this.J1 = J1;
        this.J2 = J2;
        this.J3 = J3;
        this.J4 = J4;
        this.J5 = J5;
        this.J6 = J6;
        this.X = X;
        this.Y = Y;
        this.Z = Z;
        this.W = W;
        this.P = P;
        this.R = R;
    }

    public RobotState(RobotState other)
    {
        if (other == null)
        {
            return;
        }

        this.J1 = other.J1;
        this.J2 = other.J2;
        this.J3 = other.J3;
        this.J4 = other.J4;
        this.J5 = other.J5;
        this.J6 = other.J6;
        this.X = other.X;
        this.Y = other.Y;
        this.Z = other.Z;
        this.W = other.W;
        this.P = other.P;
        this.R = other.R;

    }

    public override int GetHashCode()
    {
        var hash = new HashCode();

        hash.Add(J1);
        hash.Add(J2);
        hash.Add(J3);
        hash.Add(J4);
        hash.Add(J5);
        hash.Add(J6);
        hash.Add(X);
        hash.Add(Y);
        hash.Add(Z);
        hash.Add(W);
        hash.Add(P);
        hash.Add(R);

        return hash.ToHashCode();
    }

    public bool Equals(RobotState other)
    {
        if (other == null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return this.J1.Equals(other.J1) &&
        this.J2.Equals(other.J2) &&
        this.J3.Equals(other.J3) &&
        this.J4.Equals(other.J4) &&
        this.J5.Equals(other.J5) &&
        this.J6.Equals(other.J6) &&
        this.X.Equals(other.X) &&
        this.Y.Equals(other.Y) &&
        this.Z.Equals(other.Z) &&
        this.W.Equals(other.W) &&
        this.P.Equals(other.P) &&
        this.R.Equals(other.R);
    }

    public override bool Equals(object obj) => this.Equals(obj as RobotState);

    public override string ToString() => RobotStateSupport.Instance.ToString(this);
}

