//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.                        
//                                                                                   
// THIS CODE AND INFORMATION IS PROVIDED 'AS IS' WITHOUT WARRANTY                    
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT                       
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND                          
// FITNESS FOR A PARTICULAR PURPOSE.                                                 
//===================================================================================


#pragma once
#include <UIAnimation.h>
#include "ThumbnailLayoutManager.h"

enum AnimationType
{
    FlyIn,
    MoveAround
};

struct AnimatedThumbnailCell
{
    ThumbnailCell cell;
    D2D1_POINT_2F start;
    D2D1_POINT_2F end;
    D2D1_POINT_2F center;
    bool isCurrent;

    AnimatedThumbnailCell (ThumbnailCell thumbnailCell, D2D1_POINT_2F startPoint, D2D1_POINT_2F endPoint, D2D1_POINT_2F centerPoint, bool isCurrentThumbnailCell) :
        cell (thumbnailCell),
        start (startPoint),
        end (endPoint),
        center (centerPoint),
        isCurrent (isCurrentThumbnailCell)
    {
    }
};

[uuid("C8E9BFCB-107F-4FB7-88F7-BD48DECA0C3C")]
__interface IOrbitAnimation : public IUnknown
{
public:
    HRESULT __stdcall GetEllipse(__out D2D1_ELLIPSE* ellipse, __out_opt double* opacity);
    HRESULT __stdcall Setup(D2D1_ELLIPSE targetOrbit, double targetOpacity, double duration);
};

[uuid("16060EFF-32BF-4830-8D9A-143FA6BA4A78")]
__interface ICarouselThumbnailAnimation : public IUnknown
{
public:
    HRESULT __stdcall GetInfo(__out D2D1_POINT_2F* point, __out_opt double* opacity);
    HRESULT __stdcall Setup(D2D1_POINT_2F targetPoint, double targetOpacity, double duration);
    HRESULT __stdcall Setup(D2D1_POINT_2F keyFramePoint, D2D1_POINT_2F targetPoint, double duration);
};

[uuid("97FBF3BA-0040-4D0C-96AA-EC55E908DD3D")]
__interface ICarouselAnimation : public IUnknown
{
public:
    HRESULT __stdcall GetInfo(__out double* rotation, __out_opt double* thumbnailScale, __out_opt double* thumbnailOpacity);
    HRESULT __stdcall SetupRotation(double targetRotation, double duration);
    HRESULT __stdcall SetupScale(double targetScale, double duration);
    HRESULT __stdcall SetupOpacity(double targetOpacity, double duration);
};

[uuid("5BD2C0FA-B047-426D-A40F-6AD0BCBA4C47")]
__interface IMediaPaneAnimation : public IUnknown
{
public:
    HRESULT __stdcall Setup(const std::vector<ThumbnailCell>& prevThumbnailCells, const std::vector<ThumbnailCell>& currentThumbnailCells, D2D1_SIZE_F viewSize);
    HRESULT __stdcall GetAnimationPosition(__in IThumbnail* const control, __out D2D1_POINT_2F* point);
    HRESULT __stdcall GetAnimatedThumbnailCells(std::vector<AnimatedThumbnailCell>& animatedCells);
};

[uuid("E77D43BE-3CDA-474A-9791-81138F610EB6")]
__interface IPanAnimation : public IUnknown
{
public:
    HRESULT __stdcall GetValue(__out double *currentPosition);
    HRESULT __stdcall Setup(__in const double startPosition, __in const double endPosition, __in const double duration);
};