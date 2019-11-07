//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

class Triangle
{
public:
    XMFLOAT3 A;
    XMFLOAT3 B;
    XMFLOAT3 C;
    XMFLOAT4 plane;

    Triangle(const Triangle& rhs)
    {
        A = rhs.A;
        B = rhs.B;
        C = rhs.C;
        plane = rhs.plane;
    }

    Triangle(const XMFLOAT3& V0, const XMFLOAT3& V1, const XMFLOAT3& V2) :
        A(V0),
        B(V1),
        C(V2)
    {
        XMStoreFloat4(&plane, XMPlaneFromPoints(XMLoadFloat3(&V0), XMLoadFloat3(&V1), XMLoadFloat3(&V2)));
    }

    // Returns true if the triangle is degenerate.
    FORCEINLINE BOOL IsDegenerate() const
    {
        XMVECTOR planeNormal = XMLoadFloat3((XMFLOAT3*)&plane);
        return XMVector3Equal(planeNormal, XMVectorZero());
    }

    // Tests this triangle against another one to see if it a) shares any edges, and b) is coplanar
    FORCEINLINE void CheckSharesVertsOrCoplanar(
        FXMVECTOR V0,
        FXMVECTOR V1,
        FXMVECTOR V2,
        CXMVECTOR planeIn,
        BOOL& sharesEdgeOut,
        BOOL& coplanar
        ) const
    {
        const XMVECTOR VA = XMLoadFloat3(&A);
        XMVECTOR sharesEdge = XMVectorOrInt(
            XMVectorOrInt(
                XMVectorEqual(V0, VA),
                XMVectorEqual(V1, VA)
                ),
            XMVectorEqual(V2, VA)
            );
        const XMVECTOR VB = XMLoadFloat3(&B);
        sharesEdge = XMVectorOrInt(
            sharesEdge,
            XMVectorOrInt(
                XMVectorOrInt(
                    XMVectorEqual(V0, VB),
                    XMVectorEqual(V1, VB)
                    ),
                XMVectorEqual(V2, VB)
                )
            );
        const XMVECTOR VC = XMLoadFloat3(&C);
        sharesEdge = XMVectorOrInt(
            sharesEdge,
            XMVectorOrInt(
                XMVectorOrInt(
                    XMVectorEqual(V0, VC),
                    XMVectorEqual(V1, VC)
                    ),
                XMVectorEqual(V2, VC)
                )
            );

        sharesEdgeOut = XMVector3EqualInt(sharesEdge, XMVectorTrueInt());

        const XMVECTOR VP = XMLoadFloat3((XMFLOAT3*)&plane);
        coplanar = XMVector3Equal(VP, planeIn);
    }

    // Returns the unit normal of the triangle surface..
    FORCEINLINE XMVECTOR Normal() const
    {
        return XMLoadFloat3((XMFLOAT3*)&plane);
    }
};

// Simple sphere wrapper. We store the sphere as a 16-byte aligned FLOAT4, so that we can perform
// fast unaligned reads. All of the parameters are packed into a single FLOAT4.
struct Sphere
{
    XMFLOAT4 center;

    FORCEINLINE float Radius() const
    {
        return center.w;
    }

    Sphere()
    {
    }

    explicit Sphere(const XMFLOAT3& centerIn, FLOAT radius)
    {
        center = XMFLOAT4(centerIn.x, centerIn.y, centerIn.z, radius);
    }

    explicit Sphere(float x, float y, float z, float radius)
    {
        center = XMFLOAT4(x, y, z, radius);
    }
};
