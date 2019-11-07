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
#include "pch.h"
#include "CubeRenderer.h"

#include <math.h>
using namespace DirectX;
using namespace Microsoft::WRL;
using namespace Windows::Foundation;
using namespace Windows::UI::Core;
using namespace Windows::Graphics::Display;


CubeRenderer::CubeRenderer() :
	m_loadingComplete(false),
	m_indexCount(0)
	
{	
	m_transToOrigin = XMMatrixIdentity();	
}

void CubeRenderer::CreateDeviceIndependentResources()
{
	DirectXBase::CreateDeviceIndependentResources();
}
void CubeRenderer::CreateDeviceResources()
{
	DirectXBase::CreateDeviceResources();

	// Initialize the cube vertices.
	VertexPositionColor cubeVertices[] =
	{
		{ XMFLOAT3(-0.3f, -0.3f, -0.3f), XMFLOAT3(1.0f, 0.0f, 0.0f) },
		{ XMFLOAT3(-0.3f, 0.3f, -0.3f), XMFLOAT3(0.0f, 0.0f, 1.0f) },
		{ XMFLOAT3(0.3f, 0.3f, -0.3f), XMFLOAT3(0.0f, 1.0f, 0.0f) },
		{ XMFLOAT3(0.3f, -0.3f, -0.3f), XMFLOAT3(0.0f, 1.0f, 1.0f) },

		{ XMFLOAT3(-0.3f, -0.3f, 0.3f), XMFLOAT3(1.0f, 1.0f, 0.0f) },
		{ XMFLOAT3(0.3f, -0.3f, 0.3f), XMFLOAT3(1.0f, 0.0f, 1.0f) },
		{ XMFLOAT3(0.3f, 0.3f, 0.3f), XMFLOAT3(1.0f, 1.0f, 1.0f) },
		{ XMFLOAT3(-0.3f, 0.3f, 0.3f), XMFLOAT3(0.0f, 0.0f, 0.0f) },

		{ XMFLOAT3(-0.3f, 0.3f, -0.3f), XMFLOAT3(0.0f, 0.0f, 1.0f) },
		{ XMFLOAT3(-0.3f, 0.3f, 0.3f), XMFLOAT3(0.0f, 0.0f, 0.0f) },
		{ XMFLOAT3(0.3f, 0.3f, 0.3f), XMFLOAT3(1.0f, 1.0f, 1.0f) },
		{ XMFLOAT3(0.3f, 0.3f, -0.3f), XMFLOAT3(0.0f, 1.0f, 0.0f) },

		{ XMFLOAT3(-0.3f, -0.3f, -0.3f), XMFLOAT3(1.0f, 0.0f, 0.0f) },
		{ XMFLOAT3(0.3f, -0.3f, -0.3f), XMFLOAT3(0.0f, 1.0f, 1.0f) },
		{ XMFLOAT3(0.3f, -0.3f, 0.3f), XMFLOAT3(1.0f, 0.0f, 1.0f) },
		{ XMFLOAT3(-0.3f, -0.3f, 0.3f), XMFLOAT3(1.0f, 1.0f, 0.0f) },

		{ XMFLOAT3(-0.3f, -0.3f, 0.3f), XMFLOAT3(1.0f, 1.0f, 0.0f) },
		{ XMFLOAT3(-0.3f, 0.3f, 0.3f), XMFLOAT3(0.0f, 0.0f, 0.0f) },
		{ XMFLOAT3(-0.3f, 0.3f, -0.3f), XMFLOAT3(0.0f, 0.0f, 1.0f) },
		{ XMFLOAT3(-0.3f, -0.3f, -0.3f), XMFLOAT3(1.0f, 0.0f, 0.0f) },

		{ XMFLOAT3(0.3f, -0.3f, -0.3f), XMFLOAT3(0.0f, 1.0f, 1.0f) },
		{ XMFLOAT3(0.3f, 0.3f, -0.3f), XMFLOAT3(0.0f, 1.0f, 0.0f) },
		{ XMFLOAT3(0.3f, 0.3f, 0.3f), XMFLOAT3(1.0f, 1.0f, 1.0f) },
		{ XMFLOAT3(0.3f, -0.3f, 0.3f), XMFLOAT3(1.0f, 0.0f, 1.0f) },
	};
	m_cubeVertices = std::vector<VertexPositionColor>(cubeVertices, cubeVertices + ARRAYSIZE(cubeVertices));
	
	// Initialize the vertex points for bounding box according to the cube vertices.
	VertexPos vertexPos[8];
	for (int i = 0; i < 8; ++i)
	{
		vertexPos[i].pos = m_cubeVertices[i].pos;
	}
	m_vertexPos = std::vector<VertexPos>(vertexPos, vertexPos + ARRAYSIZE(vertexPos));

	auto loadVSTask = DX::ReadDataAsync("VertexShader.cso");
	auto loadPSTask = DX::ReadDataAsync("PixelShader.cso");

	auto createVSTask = loadVSTask.then([this](Platform::Array<byte>^ fileData) {
		DX::ThrowIfFailed(
			m_d3dDevice->CreateVertexShader(
				fileData->Data,
				fileData->Length,
				nullptr,
				&m_vertexShader
				)
			);

		const D3D11_INPUT_ELEMENT_DESC vertexDesc[] = 
		{
			{ "POSITION", 0, DXGI_FORMAT_R32G32B32_FLOAT, 0, 0,  D3D11_INPUT_PER_VERTEX_DATA, 0 },
			{ "COLOR",    0, DXGI_FORMAT_R32G32B32_FLOAT, 0, 12, D3D11_INPUT_PER_VERTEX_DATA, 0 },
		};

		DX::ThrowIfFailed(
			m_d3dDevice->CreateInputLayout(
				vertexDesc,
				ARRAYSIZE(vertexDesc),
				fileData->Data,
				fileData->Length,
				&m_inputLayout
				)
			);
	});

	auto createPSTask = loadPSTask.then([this](Platform::Array<byte>^ fileData) {
		DX::ThrowIfFailed(
			m_d3dDevice->CreatePixelShader(
				fileData->Data,
				fileData->Length,
				nullptr,
				&m_pixelShader
				)
			);

		CD3D11_BUFFER_DESC constantBufferDesc(sizeof(cbModelViewProjection), D3D11_BIND_CONSTANT_BUFFER);
		DX::ThrowIfFailed(
			m_d3dDevice->CreateBuffer(
				&constantBufferDesc,
				nullptr,
				&m_constantBuffer
				)
			);
	});

	
	auto createCubeTask = (createPSTask && createVSTask).then([this] () {

		D3D11_SUBRESOURCE_DATA vertexBufferData = {0};
		vertexBufferData.pSysMem = &m_cubeVertices[0];
		vertexBufferData.SysMemPitch = 0;
		vertexBufferData.SysMemSlicePitch = 0;
		CD3D11_BUFFER_DESC vertexBufferDesc(m_cubeVertices.size() * sizeof(VertexPositionColor), D3D11_BIND_VERTEX_BUFFER);
		DX::ThrowIfFailed(
			m_d3dDevice->CreateBuffer(
				&vertexBufferDesc,
				&vertexBufferData,
				&m_vertexBuffer
				)
			);

		unsigned short cubeIndices[] = 
		{
			// Front Face
			0,  1,  2,
			0,  2,  3,

			// Back Face
			4,  5,  6,
			4,  6,  7,

			// Top Face
			8,  9, 10,
			8, 10, 11,

			// Bottom Face
			12, 13, 14,
			12, 14, 15,

			// Left Face
			16, 17, 18,
			16, 18, 19,

			// Right Face
			20, 21, 22,
			20, 22, 23
		};
		m_cubeIndices = std::vector<short>(cubeIndices, cubeIndices + ARRAYSIZE(cubeIndices));

		m_indexCount = ARRAYSIZE(cubeIndices);

		D3D11_SUBRESOURCE_DATA indexBufferData = {0};
		indexBufferData.pSysMem = cubeIndices;
		indexBufferData.SysMemPitch = 0;
		indexBufferData.SysMemSlicePitch = 0;
		CD3D11_BUFFER_DESC indexBufferDesc(sizeof(cubeIndices), D3D11_BIND_INDEX_BUFFER);
		DX::ThrowIfFailed(
			m_d3dDevice->CreateBuffer(
				&indexBufferDesc,
				&indexBufferData,
				&m_indexBuffer
				)
			);		
		
	});

	createCubeTask.then([this] () {		
		m_boundingCube.Transform(m_boundingCube, XMLoadFloat4x4(&m_cbMVPData.model));
		m_boundingCube.Transform(m_boundingCubeInView, XMLoadFloat4x4(&m_cbMVPData.view));
		m_loadingComplete = true;
	});	
}

// Update the bounding box's position after the cube was transformed.
void CubeRenderer::UpdateCubeBounding(XMFLOAT4X4 transform)
{
	m_boundingCube.Transform(m_boundingCube, XMLoadFloat4x4(&transform));
	m_boundingCube.Transform(m_boundingCubeInView, XMLoadFloat4x4(&m_cbMVPData.view));	
}

void CubeRenderer::CreateWindowSizeDependentResources()
{
	DirectXBase::CreateWindowSizeDependentResources();

	DirectX::BoundingOrientedBox::CreateFromPoints(m_boundingCube, m_vertexPos.size(), &m_vertexPos[0].pos, sizeof(XMFLOAT3));

	float aspectRatio = m_windowBounds.Width / m_windowBounds.Height;
	float fovAngleY = 70.0f * XM_PI / 180.0f;
	
	XMStoreFloat4x4(
		&m_cbMVPData.projection,			
		XMMatrixPerspectiveFovLH(
			fovAngleY,
			aspectRatio,
			1.0f,
			100.0f
			)		
		);

	DirectX::BoundingFrustum::CreateFromMatrix(m_boundingFrustum, XMLoadFloat4x4(&m_cbMVPData.projection));
	
	m_boundingFrustum.GetPlanes(&m_nearPlane, &m_farPlane, &m_rightPlane, &m_leftPlane, &m_topPlane, &m_bottomPlane);

	XMStoreFloat4x4(
		&m_cbMVPData.projection,
		XMMatrixMultiply(
		XMLoadFloat4x4(&m_cbMVPData.projection),
		XMLoadFloat4x4(&m_orientationTransform3D)
		)
		);

	XMVECTOR eye = XMVectorSet(0.0f, 0.0f, -3.0f, 1.0f);
	XMVECTOR at = XMVectorSet(0.0f, 0.0f, 0.0f, 1.0f);
	XMVECTOR up = XMVectorSet(0.0f, 1.0f, 0.0f, 0.0f);
	
	XMStoreFloat4x4(&m_cbMVPData.view, XMMatrixLookAtLH(eye, at, up));
	
	XMStoreFloat4x4(&m_cbMVPData.model, XMMatrixIdentity());
	UpdateCubeBounding(m_cbMVPData.model);
}

// Transform the current point coordinate to the origin screen space
Point CubeRenderer::TransformToOrientation(Point point, bool dipsToPixels)
{
	Point returnValue;

	switch (m_orientation)
	{
	case DisplayOrientations::Landscape:
		returnValue = point;
		break;
	case DisplayOrientations::Portrait:
		returnValue = Point(point.Y, m_windowBounds.Width - point.X);
		break;
	case DisplayOrientations::PortraitFlipped:
		returnValue = Point(m_windowBounds.Height - point.Y, point.X);
		break;
	case DisplayOrientations::LandscapeFlipped:
		returnValue = Point(m_windowBounds.Width - point.X, m_windowBounds.Height - point.Y);
		break;
	default:
		throw ref new Platform::FailureException();
		break;
	}
	// Convert DIP to Pixels, or not?
	return dipsToPixels ? Point(ConvertDipsToPixels(returnValue.X),
		ConvertDipsToPixels(returnValue.Y))
		: returnValue;
}

void CubeRenderer::Render(float* backgroundColor)
{
	m_d3dContext->ClearRenderTargetView(
		m_d3dRenderTargetView.Get(),
		backgroundColor
		);

	m_d3dContext->ClearDepthStencilView(
		m_d3dDepthStencilView.Get(),
		D3D11_CLEAR_DEPTH | D3D11_CLEAR_STENCIL,
		1.0f,
		0
		);

	// Only draw the cube once it is loaded (loading is asynchronous).
	if (!m_loadingComplete)
	{
		return;
	}
	
	m_d3dContext->OMSetRenderTargets(
		1,
		m_d3dRenderTargetView.GetAddressOf(),
		m_d3dDepthStencilView.Get()
		);

	m_d3dContext->UpdateSubresource(
		m_constantBuffer.Get(),
		0,
		NULL,
		&m_cbMVPData,
		0,
		0
		);	

	UINT stride = sizeof(VertexPositionColor);
	UINT offset = 0;
	m_d3dContext->IASetVertexBuffers(
		0,
		1,
		m_vertexBuffer.GetAddressOf(),
		&stride,
		&offset
		);
	
	m_d3dContext->IASetIndexBuffer(
		m_indexBuffer.Get(),
		DXGI_FORMAT_R16_UINT,
		0
		);

	m_d3dContext->IASetPrimitiveTopology(D3D11_PRIMITIVE_TOPOLOGY_TRIANGLELIST);

	m_d3dContext->IASetInputLayout(m_inputLayout.Get());

	m_d3dContext->VSSetShader(
		m_vertexShader.Get(),
		nullptr,
		0
		);

	m_d3dContext->VSSetConstantBuffers(
		0,
		1,
		m_constantBuffer.GetAddressOf()
		);

	m_d3dContext->PSSetShader(
		m_pixelShader.Get(),
		nullptr,
		0
		);

	m_d3dContext->DrawIndexed(
		m_indexCount,
		0,
		0
		);
}

void CubeRenderer::Update(DirectX::XMFLOAT4X4 transform)
{
	if (!m_loadingComplete)
	{
		return;
	}
	UpdateCubeBounding(transform);

	XMMATRIX trans = XMLoadFloat4x4(&transform);		
	XMStoreFloat4x4(&m_cbMVPData.model, XMLoadFloat4x4(&m_cbMVPData.model) * trans);	
}

// The mathematical way to transform 2D screen coordinate to 3D local scene.
//void CubeRenderer::ScreenToView(
//		_In_ float sx,
//		_In_ float sy,
//		_Outptr_ float * vx,
//		_Outptr_ float * vy
//		)
//{
//	*vx = (2.0f * sx / m_windowBounds.Width - 1.0f) / m_cbMVPData.projection._11;
//	*vy = (-2.0f * sy / m_windowBounds.Height + 1.0f) / m_cbMVPData.projection._22;
//}
//DirectX::XMVECTOR  CubeRenderer::VectorToLocal(XMVECTOR inVec)
//{
//	XMMATRIX viewMx = XMLoadFloat4x4(&m_cbMVPData.view);
//	XMMATRIX modelMx = XMLoadFloat4x4(&m_cbMVPData.model);
//	XMMATRIX invView = XMMatrixInverse(&XMMatrixDeterminant(viewMx), viewMx);
//	XMMATRIX invModel = XMMatrixInverse(&XMMatrixDeterminant(modelMx), modelMx);
//	XMMATRIX toLocal = invView * invModel;
//	XMFLOAT4 inVecF;
//	XMStoreFloat4(&inVecF, inVec);
//	XMVECTOR outVec = XMVectorSet(0.0f, 0.0f, 0.0f, 0.0f);
//	if(1.0f == inVecF.w)//point vector
//	{
//		outVec = XMVector3TransformCoord(inVec, toLocal);
//	}
//	else
//	{
//		outVec = XMVector3TransformNormal(inVec, toLocal);
//		outVec = XMVector3Normalize(outVec);
//	}
//	return outVec;
//}

bool CubeRenderer::IsIntersectsTriangle(
		float sx,    // press point x
		float sy     // press point y
		)
{
	XMVECTOR vector1 = DirectX::XMVector3Unproject(
		XMVectorSet(sx, sy, 0.0f, 1.0f),
		0.0f,
		0.0f,
		m_d3dRenderTargetSize.Width,
		m_d3dRenderTargetSize.Height,
		0.0f,
		1.0f,
		XMLoadFloat4x4(&m_cbMVPData.projection),
		XMLoadFloat4x4(&m_cbMVPData.view),
		XMLoadFloat4x4(&m_cbMVPData.model)
		);
	XMVECTOR vector2 = DirectX::XMVector3Unproject(
		XMVectorSet(sx, sy, 1.0f, 1.0f),
		0.0f,
		0.0f,
		m_d3dRenderTargetSize.Width,
		m_d3dRenderTargetSize.Height,
		0.0f,
		1.0f,
		XMLoadFloat4x4(&m_cbMVPData.projection),
		XMLoadFloat4x4(&m_cbMVPData.view),
		XMLoadFloat4x4(&m_cbMVPData.model)
		);

	// [-or-]
	// The second way to get the ray direction and origin.
	//float vx, vy;
	//ScreenToView(sx, sy, &vx, &vy);
	////Get the ray
	//XMVECTOR rayOrigin = XMVectorSet(0.0f, 0.0f, 0.0f, 1.0f);
	//XMVECTOR rayDir = XMVectorSet(vx, vy, 1.0f, 0.0f);
	//rayOrigin = VectorToLocal(rayOrigin);
	//rayDir = VectorToLocal(rayDir);

	// Test inversects
	for(UINT i = 0; i< m_cubeIndices.size() / 3; ++i)
	{
		short i0 = m_cubeIndices[i * 3 + 0];
		short i1 = m_cubeIndices[i * 3 + 1];
		short i2 = m_cubeIndices[i * 3 + 2];

		XMFLOAT3 pos = m_cubeVertices[i0].pos;
		XMVECTOR v0 = XMVectorSet(pos.x, pos.y, pos.z, 0.0f);
		pos = m_cubeVertices[i1].pos;
		XMVECTOR v1 = XMVectorSet(pos.x, pos.y, pos.z, 0.0f);
		pos = m_cubeVertices[i2].pos;
		XMVECTOR v2 = XMVectorSet(pos.x, pos.y, pos.z, 0.0f);
		
		float t = 0.0f;
		// Set vector1 as ray origin, and (vector2 - vector1) as ray direction.
		// Note to normalize this direction vector.
		if (TriangleTests::Intersects(vector1, XMVector3Normalize(vector2 - vector1), v0, v1, v2, t))
		{
			return true;
		}
		//[-or-]
		// The second solution.
		//if (TriangleTests::Intersects(rayOrigin, rayDir, v0, v1, v2, t))
		//{
		//	return true;
		//}
		
	}
	return false;
}
IntersectsPlane CubeRenderer::IsIntersectsBoxBounding()
{
	if (INTERSECTING == m_boundingCubeInView.Intersects(m_nearPlane))
	{
		return IntersectsPlane::Front;
	}
	else if (INTERSECTING == m_boundingCubeInView.Intersects(m_farPlane))
	{
		return IntersectsPlane::Back;
	}
	else if (INTERSECTING == m_boundingCubeInView.Intersects(m_rightPlane))
	{
		return IntersectsPlane::Right;
	}
	else if (INTERSECTING == m_boundingCubeInView.Intersects(m_leftPlane))
	{
		return IntersectsPlane::Left;
	}
	else if (INTERSECTING == m_boundingCubeInView.Intersects(m_topPlane))
	{
		return IntersectsPlane::Top;
	}
	else if (INTERSECTING == m_boundingCubeInView.Intersects(m_bottomPlane))
	{
		return IntersectsPlane::Bottom;
	}
	else
	{
		return IntersectsPlane::NoPlane;
	}
}
DirectX::XMFLOAT3 CubeRenderer::ScreenToArcBall(
		float sx, 
		float sy
		)
{
	float width = m_windowBounds.Width;
	float height = m_windowBounds.Height;
	float x = ( sx  - width / 2 ) / ( width / 2 );
	float y = -( sy - height / 2 ) / ( height / 2 );
	
	float z = 0.0f;
	float mag = x * x + y * y;

	if (mag > 1.0f)
	{
		float scale = 1.0f / sqrtf(mag);
		x *= scale;
		y *= scale;
	}
	else
		z = -(sqrtf( 1.0f - mag ));

	return XMFLOAT3(x, y, z);
}
XMFLOAT4X4 CubeRenderer::TransformWithMouse(
		TransformTypeEnum type,
		float x1,
		float y1, 
		float z1,
		float x2,
		float y2,		
		float z2
		)
{	
	XMMATRIX transform = XMMatrixIdentity();
	if (!m_loadingComplete)
	{
		XMFLOAT4X4 out;
		XMStoreFloat4x4(&out, transform);
		return out;
	}
	XMFLOAT3 f1 = ScreenToArcBall(x1, y1);
	XMFLOAT3 f2 = ScreenToArcBall(x2, y2);
	
	XMVECTOR p1 = XMLoadFloat3(&f1);
	XMVECTOR p2 = XMLoadFloat3(&f2);
	
	if (IsIntersectsBoxBounding() > 0)
	{
		if (TransformTypeEnum::Translate == type)
		{
			switch (IsIntersectsBoxBounding())
			{
			case IntersectsPlane::Left:
				transform = XMMatrixTranslation(0.01f, 0.0f, 0.0f);
				break;
			case IntersectsPlane::Right:
				transform = XMMatrixTranslation(-0.01f, 0.0f, 0.0f);
				break;
			case IntersectsPlane::Top:
				transform = XMMatrixTranslation(0.0f, -0.01f, 0.0f);
				break;
			case IntersectsPlane::Bottom:
				transform = XMMatrixTranslation(0.0f, 0.01f, 0.0f);
				break;
			default:
				break;
			}
		
		}
		else if (TransformTypeEnum::Rotate == type)
		{
			if (IntersectsPlane::NoPlane != IsIntersectsBoxBounding())
			{
				transform = XMMatrixIdentity();
			}
		}
		else if (TransformTypeEnum::Scale == type)
		{
			if (IntersectsPlane::NoPlane != IsIntersectsBoxBounding())
			{
				transform = XMMatrixScaling(0.99f, 0.99f, 0.99f);
			}
		}
	}	
	else if(TransformTypeEnum::Rotate == type)
	{		
		// Calculate the rotation angle.		
		XMVECTOR axis = XMVector3Cross(p1, p2);
		axis = XMVector3Normalize(axis);

		XMVECTOR angVec = XMVector3Dot(XMVector3Normalize(p1), XMVector3Normalize(p2));
		float angle = acos(*angVec.m128_f32);

		// In order to rotate around the object's center, we should transform it to the origin
		// of world space, then do the rotation job. Finally, we should transform the object
		// back to the position.
		XMFLOAT4X4 transToOrigin = XMFLOAT4X4(
			1.0f, 0.0f, 0.0f, 0.0f,
			0.0f, 1.0f, 0.0f, 0.0f,
			0.0f, 0.0f, 1.0f, 0.0f,
			-m_boundingCube.Center.x, -m_boundingCube.Center.y, -m_boundingCube.Center.z, 1.0f
			);
		XMFLOAT4X4 transBack = XMFLOAT4X4(
			1.0f, 0.0f, 0.0f, 0.0f,
			0.0f, 1.0f, 0.0f, 0.0f,
			0.0f, 0.0f, 1.0f, 0.0f,
			m_boundingCube.Center.x, m_boundingCube.Center.y, m_boundingCube.Center.z, 1.0f
			);
		transform = XMLoadFloat4x4(&transToOrigin) * XMMatrixRotationNormal(axis, 3 * angle) * XMLoadFloat4x4(&transBack);
	}
	else if(TransformTypeEnum::Translate == type)
	{
		float xOffset = (x2 - x1);
		float yOffset = (y1 - y2);		
		transform = XMMatrixTranslation(xOffset / 200, yOffset / 200, 0.0f);
	}
	else if(TransformTypeEnum::Scale == type)
	{
		XMVECTOR dis = *(p2 - p1).m128_f32 > 0 ? XMVector3Length(p2 - p1) : -XMVector3Length(p2 - p1);
		transform = XMMatrixScaling(1 + *dis.m128_f32, 1 + *dis.m128_f32, 1 + *dis.m128_f32);			
	}	
	else
	{
		transform = XMMatrixIdentity();
	}
	XMFLOAT4X4 trans;
	XMStoreFloat4x4(&trans, transform);
	return trans;
}
