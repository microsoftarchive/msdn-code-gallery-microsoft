// AppTileUpdater.cpp
#include "pch.h"
#include "AppTileUpdater.h"

using namespace BackgroundTaskCX;
using namespace Platform;
using namespace concurrency;
using namespace Windows::ApplicationModel::Background;
using namespace Windows::ApplicationModel;
using namespace Windows::Foundation;
using namespace Windows::UI::Notifications;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Markup;
using namespace Windows::UI::Xaml::Media;
using namespace Windows::UI::Xaml::Media::Imaging;
using namespace Windows::Graphics::Display;
using namespace Windows::Storage::Streams;
using namespace Windows::Data::Xml::Dom;
using namespace Windows::Globalization;
using namespace Windows::Globalization::DateTimeFormatting;
using namespace Windows::UI;

AppTileUpdater::AppTileUpdater()
{
}

// When the trigger for which this backgroudn task is fired, the OnRun method will be called. 
// Since this background task is a XamlRenderingBackgroundTask, we can render XAML and use that 
// image to update the app tile of this sample. 
void AppTileUpdater::OnRun(IBackgroundTaskInstance^ taskInstance)
{
	Agile<BackgroundTaskDeferral^> deferral = Agile<BackgroundTaskDeferral^>(taskInstance->GetDeferral());

		create_task(GenerateHighResTileImageUpdateAsync("customTile.xml", "updatedTile.png", Size(150, 150)))
			.then([this, deferral]()
		{
			UpdateTile("updatedTile.png");

			// Complete the deferral so that the system knows we're done.
			deferral->Complete();
		});
}

task<void> AppTileUpdater::GenerateHighResTileImageUpdateAsync(String^ inputMarkupFilename, String^ outputImageFilename, Size size)
{
	return create_task(Package::Current->InstalledLocation->GetFolderAsync("Assets"))
		.then([inputMarkupFilename](StorageFolder^ assetsFolder) ->IAsyncOperation<StorageFile^>^ {
		return assetsFolder->GetFileAsync(inputMarkupFilename);
	}).then([](StorageFile^ markupStorageFile)  ->IAsyncOperation<Platform::String^>^ {
		return FileIO::ReadTextAsync(markupStorageFile);
	}).then([this, outputImageFilename, size](Platform::String^ markupContent){

		// Parse our custom tile XAML and create the object tree for this XAML
		Border^ border = (Border^) XamlReader::Load(markupContent);

		// Create a new semi-transparent backgroud brush for the border, with a randomly chosen color.
		// This sample is setting a color for illustration purposes. 
		// Transparency is also supported in tiles. 
		// To make the tile transparent, so that the start screen background picture that your user selected shows through the
		// tile, set Background color property in the Package.appxmanifest to "transparent".
		border->Background = GetRandomBackgroundBrush();

		// Change the text of the Timestamp TextBlock.
		Grid^ grid = (Grid^)border->Child;
		TextBlock^ timeStampText = (TextBlock^)grid->FindName("Timestamp");
		timeStampText->Text = GetCurrentTime();

		// Change the text of the TimeZone TextBlock.
		TextBlock^ timeZoneText = (TextBlock^) grid->FindName("TimeZone");
		timeZoneText->Text = GetCurrentTimezone();

		// Set the source for the image that is displayed on the tile. 
		Image^ logoImage = (Image^)grid->FindName("LogoImage");
		logoImage->Source = ref new BitmapImage(ref new Uri("ms-appx:///Assets/acorn.png"));

		// Render the XAML, in this example a Border and its content, to a bitmap and save the bitmap to a file. 
		return RenderAndSaveToFileAsync(border, outputImageFilename, (unsigned int) size.Width, (unsigned int) size.Height);
	});
}

task<void> AppTileUpdater::RenderAndSaveToFileAsync(UIElement^ uiElement, String^ outputImageFilename, uint32 width, uint32 height)
{
	RenderTargetBitmap^ rtb = ref new RenderTargetBitmap();
	return create_task(rtb->RenderAsync(uiElement, width, height))
		.then([this, rtb]() -> IAsyncOperation<IBuffer^>^ {
		this->pixelWidth = (uint32) rtb->PixelWidth;
		this->pixelHeight = (uint32) rtb->PixelHeight;
		return rtb->GetPixelsAsync();
	}).then([this, rtb, outputImageFilename](IBuffer^ buffer){

		StorePixelsFromBuffer(buffer);
		return WriteBufferToFile(outputImageFilename);
	});
}

Array<unsigned char>^ AppTileUpdater::GetArrayFromBuffer(IBuffer^ buffer)
{
	Streams::DataReader^ dataReader = Streams::DataReader::FromBuffer(buffer);
	Array<unsigned char>^ data = ref new Array<unsigned char>(buffer->Length);
	dataReader->ReadBytes(data);
	return data;
}

void AppTileUpdater::StorePixelsFromBuffer(IBuffer^ buffer)
{
	this->pixelData = GetArrayFromBuffer(buffer);
}

task<void> AppTileUpdater::WriteBufferToFile(String^ outputImageFilename)
{
	auto resultStorageFolder = Windows::ApplicationModel::Package::Current->InstalledLocation;

	return create_task(resultStorageFolder->CreateFileAsync(outputImageFilename, CreationCollisionOption::ReplaceExisting)).
		then([](StorageFile^ outputStorageFile) ->IAsyncOperation<IRandomAccessStream^>^{
		return outputStorageFile->OpenAsync(FileAccessMode::ReadWrite);
	}).then([](IRandomAccessStream^ outputFileStream) ->IAsyncOperation<BitmapEncoder^>^ {
		return BitmapEncoder::CreateAsync(BitmapEncoder::PngEncoderId, outputFileStream);
	}).then([this](BitmapEncoder^ encoder)->IAsyncAction^ {
		encoder->SetPixelData(BitmapPixelFormat::Bgra8, BitmapAlphaMode::Premultiplied, this->pixelWidth, this->pixelHeight, 96, 96, this->pixelData);
		return encoder->FlushAsync();
	}).then([this](){
		this->pixelData = nullptr;
		return;
	});
}

// Send a tile notification with the new tile payload. 
void AppTileUpdater::UpdateTile(String^ tileUpdateImagePath)
{
	auto tileUpdater = TileUpdateManager::CreateTileUpdaterForApplication();
	tileUpdater->Clear();
	auto tileTemplate = TileUpdateManager::GetTemplateContent(TileTemplateType::TileSquare150x150Image);
	auto tileImageAttributes = tileTemplate->GetElementsByTagName("image");
	static_cast<XmlElement^>(tileImageAttributes->Item(0))->SetAttribute("src", tileUpdateImagePath);
	auto notification = ref new TileNotification(tileTemplate);
	tileUpdater->Update(notification);
}

// Helper method to get the current time to be used when updating the tile in this sample.
String^ AppTileUpdater::GetCurrentTime()
{
	Calendar^ calendar = ref new Calendar();
	DateTime datetime = calendar->GetDateTime();
	DateTimeFormatter^ datetimeFormat = DateTimeFormatter::ShortTime::get();
	return datetimeFormat->Format(datetime);
}

// Helper method to get the current timezone to be used when updating the tile in this sample.
String^ AppTileUpdater::GetCurrentTimezone()
{
	Calendar^ calendar = ref new Calendar();
	return calendar->GetTimeZone();
}

// Helper method to get a random background color to be used when updating the tile in this sample.
Brush^ AppTileUpdater::GetRandomBackgroundBrush()
{
	// Seed our random number generator.
	srand((unsigned int)time(0));

	return ref new SolidColorBrush(Windows::UI::ColorHelper::FromArgb(255, 0, (byte) rand() % 255, (byte) rand() % 255));
}

