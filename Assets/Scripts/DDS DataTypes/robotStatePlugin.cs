/*
WARNING: THIS FILE IS AUTO-GENERATED. DO NOT MODIFY.

This file was generated from robotState.idl
using RTI Code Generator (rtiddsgen) version 3.1.0.
The rtiddsgen tool is part of the RTI Connext DDS distribution.
For more information, type 'rtiddsgen -help' at a command shell
or consult the Code Generator User's Manual.
*/

using System;
using System.Runtime.InteropServices;
using Omg.Types;
using Omg.Types.Dynamic;
using Rti.Types;
using Rti.Dds.Core;
using Rti.Types.Dynamic;
using Rti.Dds.NativeInterface.TypePlugin;

namespace Telexistence
{
    public struct RobotStateUnmanaged : Rti.Dds.NativeInterface.TypePlugin.INativeTopicType<RobotState>
    {

        private double J1;
        private double J2;
        private double J3;
        private double J4;
        private double J5;
        private double J6;
        private double X;
        private double Y;
        private double Z;
        private double W;
        private double P;
        private double R;

        public void Destroy(bool optionalsOnly)
        {
        }

        public void FromNative(RobotState sample, bool keysOnly = false)
        {

            sample.J1 = J1;
            sample.J2 = J2;
            sample.J3 = J3;
            sample.J4 = J4;
            sample.J5 = J5;
            sample.J6 = J6;
            sample.X = X;
            sample.Y = Y;
            sample.Z = Z;
            sample.W = W;
            sample.P = P;
            sample.R = R;
        }

        public void Initialize(bool allocatePointers = true, bool allocateMemory = true)
        {
            J1 = (double) 0.0;
            J2 = (double) 0.0;
            J3 = (double) 0.0;
            J4 = (double) 0.0;
            J5 = (double) 0.0;
            J6 = (double) 0.0;
            X = (double) 0.0;
            Y = (double) 0.0;
            Z = (double) 0.0;
            W = (double) 0.0;
            P = (double) 0.0;
            R = (double) 0.0;
        }

        public void ToNative(RobotState sample, bool keysOnly = false)
        {
            J1 = sample.J1;
            J2 = sample.J2;
            J3 = sample.J3;
            J4 = sample.J4;
            J5 = sample.J5;
            J6 = sample.J6;
            X = sample.X;
            Y = sample.Y;
            Z = sample.Z;
            W = sample.W;
            P = sample.P;
            R = sample.R;
        }
    }

    internal class RobotStatePlugin : Rti.Dds.NativeInterface.TypePlugin.InterpretedTypePlugin<RobotState, RobotStateUnmanaged>
    {
        internal RobotStatePlugin() : base("RobotState", isKeyed: false, CreateDynamicType(isPublic: false))
        {
        }

        public static DynamicType CreateDynamicType(bool isPublic = true)
        {
            var dtf = ServiceEnvironment.Instance.Internal.GetTypeFactory(isPublic);
            var tsf = ServiceEnvironment.Instance.Internal.TypeSupportFactory;

            // RobotState struct
            var RobotStateStructMembers = new StructMember[]
            {
                new StructMember("J1", dtf.GetPrimitiveType<double>(), id: 0),
                new StructMember("J2", dtf.GetPrimitiveType<double>(), id: 1),
                new StructMember("J3", dtf.GetPrimitiveType<double>(), id: 2),
                new StructMember("J4", dtf.GetPrimitiveType<double>(), id: 3),
                new StructMember("J5", dtf.GetPrimitiveType<double>(), id: 4),
                new StructMember("J6", dtf.GetPrimitiveType<double>(), id: 5),
                new StructMember("X", dtf.GetPrimitiveType<double>(), id: 6),
                new StructMember("Y", dtf.GetPrimitiveType<double>(), id: 7),
                new StructMember("Z", dtf.GetPrimitiveType<double>(), id: 8),
                new StructMember("W", dtf.GetPrimitiveType<double>(), id: 9),
                new StructMember("P", dtf.GetPrimitiveType<double>(), id: 10),
                new StructMember("R", dtf.GetPrimitiveType<double>(), id: 11)
            };

            return tsf.CreateTypeWithAccessInfo<RobotStateUnmanaged>(
                dtf.BuildStruct()
                .WithExtensibility(ExtensibilityKind.Extensible)
                .WithName("RobotState")
                .AddMembers(RobotStateStructMembers));
        }
    }
}

public class RobotStateSupport : Rti.Dds.Topics.TypeSupport<RobotState>
{
    public RobotStateSupport() : base(
        new Telexistence.RobotStatePlugin(),
        new Lazy<DynamicType>(() => Telexistence.RobotStatePlugin.CreateDynamicType(isPublic: true)))
    {
    }

    public static RobotStateSupport Instance { get; } =
    ServiceEnvironment.Instance.Internal.TypeSupportFactory.CreateTypeSupport<RobotStateSupport, RobotState>();
}

