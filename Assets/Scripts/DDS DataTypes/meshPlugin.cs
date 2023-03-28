/*
WARNING: THIS FILE IS AUTO-GENERATED. DO NOT MODIFY.

This file was generated from mesh.idl
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
    public struct Vector3Unmanaged : Rti.Dds.NativeInterface.TypePlugin.INativeTopicType<Vector3DDS>
    {

        private float x;
        private float y;
        private float z;

        public void Destroy(bool optionalsOnly)
        {
        }

        public void FromNative(Vector3DDS sample, bool keysOnly = false)
        {

            sample.x = x;
            sample.y = y;
            sample.z = z;
        }

        public void Initialize(bool allocatePointers = true, bool allocateMemory = true)
        {
            x = (float) 0.0f;
            y = (float) 0.0f;
            z = (float) 0.0f;
        }

        public void ToNative(Vector3DDS sample, bool keysOnly = false)
        {
            x = sample.x;
            y = sample.y;
            z = sample.z;
        }
    }

    internal class Vector3Plugin : Rti.Dds.NativeInterface.TypePlugin.InterpretedTypePlugin<Vector3DDS, Vector3Unmanaged>
    {
        internal Vector3Plugin() : base("Vector3", isKeyed: false, CreateDynamicType(isPublic: false))
        {
        }

        public static DynamicType CreateDynamicType(bool isPublic = true)
        {
            var dtf = ServiceEnvironment.Instance.Internal.GetTypeFactory(isPublic);
            var tsf = ServiceEnvironment.Instance.Internal.TypeSupportFactory;

            // Vector3 struct
            var Vector3StructMembers = new StructMember[]
            {
                new StructMember("x", dtf.GetPrimitiveType<float>(), id: 0),
                new StructMember("y", dtf.GetPrimitiveType<float>(), id: 1),
                new StructMember("z", dtf.GetPrimitiveType<float>(), id: 2)
            };

            return tsf.CreateTypeWithAccessInfo<Vector3Unmanaged>(
                dtf.BuildStruct()
                .WithExtensibility(ExtensibilityKind.Extensible)
                .WithName("Vector3")
                .AddMembers(Vector3StructMembers));
        }
    }
}

public class Vector3Support : Rti.Dds.Topics.TypeSupport<Vector3DDS>
{
    public Vector3Support() : base(
        new Telexistence.Vector3Plugin(),
        new Lazy<DynamicType>(() => Telexistence.Vector3Plugin.CreateDynamicType(isPublic: true)))
    {
    }

    public static Vector3Support Instance { get; } =
    ServiceEnvironment.Instance.Internal.TypeSupportFactory.CreateTypeSupport<Vector3Support, Vector3DDS>();
}

namespace Telexistence
{
    public struct MeshDataUnmanaged : Rti.Dds.NativeInterface.TypePlugin.INativeTopicType<MeshData>
    {

        private NativeSeq Vertices;
        private NativeSeq Triangles;
        private NativeSeq TextureBytes;

        public void Destroy(bool optionalsOnly)
        {
            if (optionalsOnly)
            {
                return;
            }
            Vertices.Destroy<Vector3DDS, global::Telexistence.Vector3Unmanaged>(optionalsOnly);
            Triangles.Destroy(optionalsOnly);
            TextureBytes.Destroy(optionalsOnly);
        }

        public void FromNative(MeshData sample, bool keysOnly = false)
        {

            Vertices.FromNative<Vector3DDS, global::Telexistence.Vector3Unmanaged>(sample.Vertices);
            Triangles.FromNative((Sequence<int>) sample.Triangles);
            TextureBytes.FromNative((Sequence<byte>) sample.TextureBytes);
        }

        public void Initialize(bool allocatePointers = true, bool allocateMemory = true)
        {
            Vertices.Initialize<Vector3DDS , global::Telexistence.Vector3Unmanaged >(max: ((int)30000000), absoluteMax: ((int)30000000), allocateMemory: allocateMemory);
            Triangles.Initialize<int >(max: ((int)30000000), absoluteMax: ((int)30000000), allocateMemory: allocateMemory);
            TextureBytes.Initialize<byte >(max: ((int)30000000), absoluteMax: ((int)30000000), allocateMemory: allocateMemory);
        }

        public void ToNative(MeshData sample, bool keysOnly = false)
        {
            Vertices.ToNative<Vector3DDS, global::Telexistence.Vector3Unmanaged>(sample.Vertices);
            Triangles.ToNative((Sequence<int>) sample.Triangles);
            TextureBytes.ToNative((Sequence<byte>) sample.TextureBytes);
        }
    }

    internal class MeshDataPlugin : Rti.Dds.NativeInterface.TypePlugin.InterpretedTypePlugin<MeshData, MeshDataUnmanaged>
    {
        internal MeshDataPlugin() : base("MeshData", isKeyed: false, CreateDynamicType(isPublic: false))
        {
        }

        public static DynamicType CreateDynamicType(bool isPublic = true)
        {
            var dtf = ServiceEnvironment.Instance.Internal.GetTypeFactory(isPublic);
            var tsf = ServiceEnvironment.Instance.Internal.TypeSupportFactory;

            // MeshData struct
            var MeshDataStructMembers = new StructMember[]
            {
                new StructMember("Vertices", tsf.CreateSequenceWithAccessInfo(dtf, Vector3Support.Instance.GetDynamicTypeInternal(isPublic), ((int) 30000000)), id: 0),
                new StructMember("Triangles", tsf.CreateSequenceWithAccessInfo(dtf,  dtf.GetPrimitiveType<int>(), ((int) 30000000)), id: 1),
                new StructMember("TextureBytes", tsf.CreateSequenceWithAccessInfo(dtf,  dtf.GetPrimitiveType<byte>(), ((int) 30000000)), id: 2)
            };

            return tsf.CreateTypeWithAccessInfo<MeshDataUnmanaged>(
                dtf.BuildStruct()
                .WithExtensibility(ExtensibilityKind.Extensible)
                .WithName("MeshData")
                .AddMembers(MeshDataStructMembers));
        }
    }
}

public class MeshDataSupport : Rti.Dds.Topics.TypeSupport<MeshData>
{
    public MeshDataSupport() : base(
        new Telexistence.MeshDataPlugin(),
        new Lazy<DynamicType>(() => Telexistence.MeshDataPlugin.CreateDynamicType(isPublic: true)))
    {
    }

    public static MeshDataSupport Instance { get; } =
    ServiceEnvironment.Instance.Internal.TypeSupportFactory.CreateTypeSupport<MeshDataSupport, MeshData>();
}