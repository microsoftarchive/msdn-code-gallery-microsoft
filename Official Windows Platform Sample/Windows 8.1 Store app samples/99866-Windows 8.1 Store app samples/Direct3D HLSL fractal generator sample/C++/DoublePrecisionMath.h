//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

typedef Vector2<double> double2;

typedef Matrix4x4<double> double4x4;

inline double4x4 identityDouble()
{
    double4x4 mOut;

    mOut._11 = 1.0; mOut._12 = 0.0; mOut._13 = 0.0; mOut._14 = 0.0;
    mOut._21 = 0.0; mOut._22 = 1.0; mOut._23 = 0.0; mOut._24 = 0.0;
    mOut._31 = 0.0; mOut._32 = 0.0; mOut._33 = 1.0; mOut._34 = 0.0;
    mOut._41 = 0.0; mOut._42 = 0.0; mOut._43 = 0.0; mOut._44 = 1.0;

    return mOut;
}

inline double4x4 translationDouble(double x, double y, double z)
{
    double4x4 mOut;

    mOut._11 = 1.0; mOut._12 = 0.0; mOut._13 = 0.0; mOut._14 = x;
    mOut._21 = 0.0; mOut._22 = 1.0; mOut._23 = 0.0; mOut._24 = y;
    mOut._31 = 0.0; mOut._32 = 0.0; mOut._33 = 1.0; mOut._34 = z;
    mOut._41 = 0.0; mOut._42 = 0.0; mOut._43 = 0.0; mOut._44 = 1.0;

    return mOut;
}

inline double4x4 scaleDouble(double x, double y, double z)
{
    double4x4 mOut;

    mOut._11 = x;   mOut._12 = 0.0; mOut._13 = 0.0; mOut._14 = 0.0;
    mOut._21 = 0.0; mOut._22 = y;   mOut._23 = 0.0; mOut._24 = 0.0;
    mOut._31 = 0.0; mOut._32 = 0.0; mOut._33 = z;   mOut._34 = 0.0;
    mOut._41 = 0.0; mOut._42 = 0.0; mOut._43 = 0.0; mOut._44 = 1.0;

    return mOut;
}

inline double4x4 rotationZDouble(double degreeZ)
{
    double angleInRadians = degreeZ * (M_PI / 180.0);

    double sinAngle = sin(angleInRadians);
    double cosAngle = cos(angleInRadians);

    double4x4 mOut;

    mOut._11 = cosAngle; mOut._12 = -sinAngle; mOut._13 = 0.0; mOut._14 = 0.0;
    mOut._21 = sinAngle; mOut._22 = cosAngle;  mOut._23 = 0.0; mOut._24 = 0.0;
    mOut._31 = 0.0;      mOut._32 = 0.0;       mOut._33 = 1.0; mOut._34 = 0.0;
    mOut._41 = 0.0;      mOut._42 = 0.0;       mOut._43 = 0.0; mOut._44 = 1.0;

    return mOut;
}

inline double4x4 mulDouble(double4x4 m1, double4x4 m2)
{
    double4x4 mr;
    mr._11 = m1._11 * m2._11 + m1._12 * m2._21 + m1._13 * m2._31 + m1._14 * m2._41;
    mr._12 = m1._11 * m2._12 + m1._12 * m2._22 + m1._13 * m2._32 + m1._14 * m2._42;
    mr._13 = m1._11 * m2._13 + m1._12 * m2._23 + m1._13 * m2._33 + m1._14 * m2._43;
    mr._14 = m1._11 * m2._14 + m1._12 * m2._24 + m1._13 * m2._34 + m1._14 * m2._44;
    mr._21 = m1._21 * m2._11 + m1._22 * m2._21 + m1._23 * m2._31 + m1._24 * m2._41;
    mr._22 = m1._21 * m2._12 + m1._22 * m2._22 + m1._23 * m2._32 + m1._24 * m2._42;
    mr._23 = m1._21 * m2._13 + m1._22 * m2._23 + m1._23 * m2._33 + m1._24 * m2._43;
    mr._24 = m1._21 * m2._14 + m1._22 * m2._24 + m1._23 * m2._34 + m1._24 * m2._44;
    mr._31 = m1._31 * m2._11 + m1._32 * m2._21 + m1._33 * m2._31 + m1._34 * m2._41;
    mr._32 = m1._31 * m2._12 + m1._32 * m2._22 + m1._33 * m2._32 + m1._34 * m2._42;
    mr._33 = m1._31 * m2._13 + m1._32 * m2._23 + m1._33 * m2._33 + m1._34 * m2._43;
    mr._34 = m1._31 * m2._14 + m1._32 * m2._24 + m1._33 * m2._34 + m1._34 * m2._44;
    mr._41 = m1._41 * m2._11 + m1._42 * m2._21 + m1._43 * m2._31 + m1._44 * m2._41;
    mr._42 = m1._41 * m2._12 + m1._42 * m2._22 + m1._43 * m2._32 + m1._44 * m2._42;
    mr._43 = m1._41 * m2._13 + m1._42 * m2._23 + m1._43 * m2._33 + m1._44 * m2._43;
    mr._44 = m1._41 * m2._14 + m1._42 * m2._24 + m1._43 * m2._34 + m1._44 * m2._44;
    return mr;
}

inline double2 mulDouble(double2 v, double4x4 m)
{
    return double2(
        v.x * m._11 + v.y * m._12 + m._14,
        v.x * m._21 + v.y * m._22 + m._24
    );
}

inline float4x4 convert(double4x4 m)
{
    float4x4 mr;
    mr._11 = static_cast<float>(m._11); mr._12 = static_cast<float>(m._12); mr._13 = static_cast<float>(m._13); mr._14 = static_cast<float>(m._14);
    mr._21 = static_cast<float>(m._21); mr._22 = static_cast<float>(m._22); mr._23 = static_cast<float>(m._23); mr._24 = static_cast<float>(m._24);
    mr._31 = static_cast<float>(m._31); mr._32 = static_cast<float>(m._32); mr._33 = static_cast<float>(m._33); mr._34 = static_cast<float>(m._34);
    mr._41 = static_cast<float>(m._41); mr._42 = static_cast<float>(m._42); mr._43 = static_cast<float>(m._43); mr._44 = static_cast<float>(m._44);
    return mr;
}