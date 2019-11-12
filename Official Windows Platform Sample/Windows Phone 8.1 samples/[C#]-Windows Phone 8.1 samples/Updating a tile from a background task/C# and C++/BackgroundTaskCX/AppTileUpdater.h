#pragma once
using namespace Platform;
using namespace Windows::ApplicationModel::Background;
using namespace Windows::Graphics::Imaging;
using namespace Windows::Storage;
using namespace Windows::UI::Notifications;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Markup;
using namespace Windows::UI::Xaml::Media;
using namespace Windows::UI::Xaml::Media::Imaging;
using namespace concurrency;
using namespace Windows::Storage::Streams;
using namespace Windows::Foundation;
using namespace Windows::UI::Xaml;

namespace BackgroundTaskCX
{
	public ref class AppTileUpdater sealed : XamlRenderingBackgroundTask
    {
	private:
		// Helper methods
		task<void> WriteBufferToFile(String^ outputImageFilename);
		Array<unsigned char>^ GetArrayFromBuffer(IBuffer^ buffer);
		void StorePixelsFromBuffer(IBuffer^ buffer);

		// Custom tile helpers
		String^ GetCurrentTime();
		String^ GetCurrentTimezone();
		Brush^ GetRandomBackgroundBrush();

		// RenderTargetBitmap
		task<void> GenerateHighResTileImageUpdateAsync(String^ inputMarkupFilename, String^ outputImageFilename, Size size);
		task<void> RenderAndSaveToFileAsync(UIElement^ uiElement, String^ outputImageFilename, uint32 width = 0, uint32 height = 0);

		// RenderTargetBitmap Pixel Data
		unsigned int pixelWidth;
		unsigned int pixelHeight;
		Array<unsigned char>^ pixelData;

		// Tile Updating
		void UpdateTile(String^ tileUpdateImagePath);

	protected:
		void OnRun(IBackgroundTaskInstance^ taskInstance) override;
    public:
		AppTileUpdater();
    };
}