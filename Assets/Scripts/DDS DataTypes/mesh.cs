
/*
WARNING: THIS FILE IS AUTO-GENERATED. DO NOT MODIFY.

This file was generated from mesh.idl
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

public class Vector3DDS :  IEquatable<Vector3DDS>
{
    public float x { get; set; }

    public float y { get; set; }

    public float z { get; set; }

    public Vector3DDS()
    {
    }

    public Vector3DDS(float  x, float  y, float  z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public Vector3DDS(Vector3DDS other)
    {
        if (other == null)
        {
            return;
        }

        this.x = other.x;
        this.y = other.y;
        this.z = other.z;

    }

    public override int GetHashCode()
    {
        var hash = new HashCode();

        hash.Add(x);
        hash.Add(y);
        hash.Add(z);

        return hash.ToHashCode();
    }

    public bool Equals(Vector3DDS other)
    {
        if (other == null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return this.x.Equals(other.x) &&
        this.y.Equals(other.y) &&
        this.z.Equals(other.z);
    }

    public override bool Equals(object obj) => this.Equals(obj as Vector3DDS);

    public override string ToString() => Vector3Support.Instance.ToString(this);
}

public class MeshData :  IEquatable<MeshData>
{
    [Bound(100)]
    public ISequence<Vector3DDS> Vertices { get; }

    [Bound(100)]
    public ISequence<int> Triangles { get; }

    [Bound(100)]
    public ISequence<byte> TextureBytes { get; }

    public MeshData()
    {
        Vertices = new Rti.Types.Sequence<Vector3DDS>();
        Triangles = new Rti.Types.Sequence<int>();
        TextureBytes = new Rti.Types.Sequence<byte>();
    }

    public MeshData(ISequence<Vector3DDS>Vertices, ISequence<int>Triangles, ISequence<byte>TextureBytes)
    {
        this.Vertices = Vertices;
        this.Triangles = Triangles;
        this.TextureBytes = TextureBytes;
    }

    public MeshData(MeshData other)
    {
        if (other == null)
        {
            return;
        }

        this.Vertices = new Rti.Types.Sequence<Vector3DDS>(other.Vertices);
        this.Triangles = new Rti.Types.Sequence<int>(other.Triangles);
        this.TextureBytes = new Rti.Types.Sequence<byte>(other.TextureBytes);

    }

    public override int GetHashCode()
    {
        var hash = new HashCode();

        hash.Add(Vertices.Count);
        hash.Add(Triangles.Count);
        hash.Add(TextureBytes.Count);

        return hash.ToHashCode();
    }

    public bool Equals(MeshData other)
    {
        if (other == null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return this.Vertices.SequenceEqual(other.Vertices) &&
        this.Triangles.SequenceEqual(other.Triangles) &&
        this.TextureBytes.SequenceEqual(other.TextureBytes);
    }

    public override bool Equals(object obj) => this.Equals(obj as MeshData);

    public override string ToString() => MeshDataSupport.Instance.ToString(this);
}

