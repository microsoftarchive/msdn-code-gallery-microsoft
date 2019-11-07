//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.                        
//                                                                                   
// THIS CODE AND INFORMATION IS PROVIDED 'AS IS' WITHOUT WARRANTY                    
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT                       
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND                          
// FITNESS FOR A PARTICULAR PURPOSE.                                                 
//===================================================================================


#pragma once
#include "MediaPaneAnimation.h"

// Use the changes of the geometry to represents an animated object
struct FlyerAnimationVar
{
    ComPtr<ID2D1PathGeometry> geometry;
    ComPtr<IUIAnimationVariable> animationLength;
};

// Every thumbnail control has a corresponding animation variable
typedef std::pair<ComPtr<IThumbnail>, FlyerAnimationVar> FlyerAnimationPair;
typedef std::map<ComPtr<IThumbnail>, FlyerAnimationVar> FlyerAnimationMap;

// FlyerAnimation class
class FlyerAnimation: public MediaPaneAnimation
{
private:
    // Variables
    ComPtr<ID2D1Factory> m_d2dFactory;
    FlyerAnimationMap m_geometryAnimationVariables;
    std::vector<AnimatedThumbnailCell> m_animatedThumbnailCells;

    // const animation params
    static const double m_acceleration;

    // Operations
    HRESULT CreatePath();
    HRESULT AddTransition(IUIAnimationStoryboard* storyboard, IUIAnimationVariable* primaryVariable, double primaryValue);

protected:
    // Constructor/ Destructor
    FlyerAnimation();
    virtual ~FlyerAnimation();

    HRESULT BuildStoryboard();
    HRESULT CreateAnimatedThumbnailCells(const std::vector<ThumbnailCell>& prevThumbnailCells, const std::vector<ThumbnailCell>& currentThumbnailCells, D2D1_SIZE_F viewSize);

public:
    // Implementation
    HRESULT __stdcall GetAnimatedThumbnailCells(std::vector<AnimatedThumbnailCell>& animatedCells);
    HRESULT __stdcall GetAnimationPosition(__in IThumbnail* const control, __out D2D1_POINT_2F* position);
};

