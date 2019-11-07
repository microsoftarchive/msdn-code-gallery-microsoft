// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
#include "pch.h"
#include "LocalResourceLoader.h"
#include "WideFiveImageTile.h"
#include "LocalResourceLoader.h"

using namespace Hilo;
using namespace std;
using namespace Platform;
using namespace Windows::UI::Notifications;
using namespace Windows::Data::Xml::Dom;

const unsigned int MaxTemplateImages = 5;

WideFiveImageTile::WideFiveImageTile()
{
    m_loader = ref new LocalResourceLoader();
}

WideFiveImageTile::WideFiveImageTile(Hilo::IResourceLoader^ loader) : m_loader(loader)
{
}

TileNotification^ WideFiveImageTile::GetTileNotification()
{
    auto content = TileUpdateManager::GetTemplateContent(TileTemplateType::TileWideImageCollection);

    UpdateContentWithValues(content);

    return ref new TileNotification(content);
}

void WideFiveImageTile::SetImageFilePaths(const vector<wstring>& fileNames)
{
    if (fileNames.size() > MaxTemplateImages)
    {
        throw ref new FailureException(m_loader->GetString("ErrorWideTileTooBig"));
    }

    m_fileNames = fileNames;
}

void WideFiveImageTile::UpdateContentWithValues(XmlDocument^ content)
{
    if (m_fileNames.size() == 0) return;

    // Update wide tile template with the selected images.
    for(unsigned int image = 0; image < m_fileNames.size(); image++)
    {
        IXmlNode^ tileImage = content->GetElementsByTagName("image")->GetAt(image);
        tileImage->Attributes->GetNamedItem("src")->InnerText = ref new String(
            m_fileNames[image].c_str());
    }

    // Update square tile template with the first image.
    TileTemplateType squareTileTemplate = TileTemplateType::TileSquareImage;
    XmlDocument^ squareTileXml = TileUpdateManager::GetTemplateContent(squareTileTemplate);

    IXmlNode^ tileImage = squareTileXml->GetElementsByTagName("image")->First()->Current;
    tileImage->Attributes->GetNamedItem("src")->InnerText = ref new String(
        m_fileNames[0].c_str());

    auto node = content->ImportNode(squareTileXml->GetElementsByTagName("binding")->First()->Current, true);
    content->GetElementsByTagName("visual")->First()->Current->AppendChild(node);
}
