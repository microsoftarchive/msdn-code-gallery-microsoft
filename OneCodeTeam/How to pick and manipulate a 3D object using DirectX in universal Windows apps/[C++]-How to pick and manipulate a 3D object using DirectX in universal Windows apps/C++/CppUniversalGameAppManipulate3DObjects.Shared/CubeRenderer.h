/****************************** Module Header ******************************\
* Module Name:  CubeRenderer.cpp
* Project:      CppWindowsStoreAppManipulate3DObjects
* Copyright (c) Microsoft Corporation.
*
* This class is responsible for rendering the cube, testing intersects,
* manipulating the cube and testing the bounding.
*
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL
* All other rights reserved.
*
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/
#pragma once

#include "DirectXBase.h"
#include <DirectXCollision.h>

struct cbModelViewProjection
{
	DirectX::XMFLOAT4X4 model;
	DirectX::XMFLOAT4X4 view;
	DirectX::XMFLOAT4X4 projection;
};

struct VertexPositionColor
{
	DirectX::XMFLOAT3 pos;
	DirectX::XMFLOAT3 color;
};
struct VertexPos
{
	DirectX::XMFLOAT3 pos;
};
enum TransformTypeEnum
{
	None,
	Rotate,
	Translate,
	Scale
};
enum IntersectsPlane
{
	NoPlane,
	Left,
	Right,
	Top,
	Bottom,
	Front,
	Back
};
ref class CubeRenderer sealed : public DirectXBase
{
public:
	CubeRenderer();
	
	// DirectXBase methods.
	virtual void CreateDeviceIndependentResources() override;
	virtual void CreateDeviceResources() override;
	virtual void CreateWindowSizeDependentResources() override;
	virtual void Render(float* backgroundColor) override;
	
internal:
	void Update(DirectX::XMFLOAT4X4 transform);
	Windows::Foundation::Point TransformToOrientation(Windows::Foundation::Point point, bool dipsToPixels);

	bool IsIntersectsTriangle(
		float sx,    // press point x
		float sy     // press point y
		);
	
	DirectX::XMFLOAT4X4 TransformWithMouse(
		TransformTypeEnum type,
		float x1,
		float y1, 
		float z1,
		float x2,
		float y2,		
		float z2
		);
	
private:
		
	bool m_loadingComplete;
	Microsoft::WRL::ComPtr<ID3D11InputLayout> m_inputLayout;
	Microsoft::WRL::ComPtr<ID3D11Buffer> m_vertexBuffer;
	Microsoft::WRL::ComPtr<ID3D11Buffer> m_indexBuffer;
	Microsoft::WRL::ComPtr<ID3D11VertexShader> m_vertexShader;
	Microsoft::WRL::ComPtr<ID3D11PixelShader> m_pixelShader;
	Microsoft::WRL::ComPtr<ID3D11Buffer> m_constantBuffer;

	DirectX::BoundingOrientedBox m_boundingCube;
	DirectX::BoundingOrientedBox m_boundingCubeInView;
	DirectX::BoundingFrustum m_boundingFrustum;

	DirectX::XMVECTOR m_nearPlane;
	DirectX::XMVECTOR m_farPlane;
	DirectX::XMVECTOR m_rightPlane;
	DirectX::XMVECTOR m_leftPlane;
	DirectX::XMVECTOR m_topPlane;
	DirectX::XMVECTOR m_bottomPlane;

	DirectX::XMMATRIX m_transToOrigin;

	void UpdateCubeBounding(DirectX::XMFLOAT4X4 transform);
	std::vector<VertexPos> m_vertexPos;	

	uint32 m_indexCount;
	cbModelViewProjection m_cbMVPData;

	std::vector<VertexPositionColor> m_cubeVertices;
	std::vector<short> m_cubeIndices;

	DirectX::XMVECTOR  VectorToLocal(DirectX::XMVECTOR inVec);
	void ScreenToView(_In_ float sx,_In_ float sy,
		_Outptr_ float * vx,
		_Outptr_ float * vy
		);
	DirectX::XMFLOAT3 ScreenToArcBall(float sx, float sy);
	IntersectsPlane IsIntersectsBoxBounding();
};

