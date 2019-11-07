#include "pch.h"
#include "HeartRateService.h"

#include "MainPage.xaml.h"

using namespace concurrency;
using namespace Platform;
using namespace Platform::Collections;
using namespace Windows::Devices::Enumeration;
using namespace Windows::Devices::Enumeration::Pnp;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::Storage::Streams;
using namespace Windows::UI::Xaml;

using namespace Windows::Devices::Bluetooth::GenericAttributeProfile;

using namespace SDKSample::BluetoothGattHeartRate;

HeartRateService^ HeartRateService::instance = nullptr;

HeartRateService::HeartRateService()
{
    datapoints = ref new Vector<HeartRateMeasurement^>();
    App::Current->Suspending += ref new SuspendingEventHandler(this, &HeartRateService::App_Suspending);
    App::Current->Resuming += ref new EventHandler<Platform::Object^>(this, &HeartRateService::App_Resuming);
}


HeartRateService::~HeartRateService()
{
}

void HeartRateService::App_Suspending(Object^ sender, Windows::ApplicationModel::SuspendingEventArgs^ e)
{
    isServiceInitialized = false;

    // Obtain RAII lock from dataLock
    reader_writer_lock::scoped_lock lock(dataLock);

    // This is an appropriate place to save to persistent storage any datapoint the application cares about.
    // For the purpose of this sample we just discard any values.
    datapoints->Clear();

    // Allow the GattDeviceService to get cleaned up by the Windows Runtime.
    // The Windows runtime will clean up resources used by the GattDeviceService object when the application gets 
    // suspended. The GattDeviceService object will be invalid once the app resumes, which is why it must be marked as
    // invalid, and reinitalized when the application resumes.
    if (service != nullptr)
    {
        delete service;
        service = nullptr;
    }

    if (characteristic != nullptr)
    {
        characteristic = nullptr;
    }

    if (watcher != nullptr)
    {
        watcher->Stop();
        watcher = nullptr;
    }
}

void HeartRateService::App_Resuming(Object^ sender, Object^ e)
{
    // Since the Windows Runtime will close resources to the device when the app is suspended,
    // the device needs to be reinitialized when the app is resumed.
}

IAsyncAction^ HeartRateService::InitializeHeartRateServicesAsync(DeviceInformation^ device)
{
    return create_async([this, device]()
    {
        auto containerId = device->Properties->Lookup("System.Devices.ContainerId");
        this->deviceContainerId = containerId->ToString();

        return create_task(GattDeviceService::FromIdAsync(device->Id))
            .then([this, device] (GattDeviceService^ service)
        {
            if (service != nullptr)
            {
                this->service = service;
                IsServiceInitialized = true;
                return ConfigureServiceForNotificationsAsync(); 
            }
            else
            {
                MainPage::Current->NotifyUser("Access to the device is denied, because the application was not " +
                    "granted access, or the device is currently in use by another application.",
                    NotifyType::StatusMessage);
            }
            // The lambda must return a continuation, but the prerequisites to call
            // WriteClientCharacteristicConfigurationDescriptorAsync were not met, and its continuation could not
            // be returned, so a placeholder continuation must be returned instead.
            return create_task([] {});
        });
    });
}

/// <summary>
/// Configure the Bluetooth device to send notifications whenever the Characteristic value changes
/// </summary>
task<void> HeartRateService::ConfigureServiceForNotificationsAsync()
{
    // The Heart Rate profile states that there must be one HeartRateMeasurement characteristic for the service
    characteristic = service->GetCharacteristics(GattCharacteristicUuids::HeartRateMeasurement)->GetAt(0);

    // While encryption is not required by all devices, if encryption is supported by the device,
    // it can be enabled by setting the ProtectionLevel property of the Characteristic object.
    // All subsequent operations on the characteristic will work over an encrypted link.
    this->characteristic->ProtectionLevel = GattProtectionLevel::EncryptionRequired;

    // Register the event handler for receiving device notification data
    this->characteristic->ValueChanged +=
        ref new TypedEventHandler<GattCharacteristic^, GattValueChangedEventArgs^>(
        this,
        &HeartRateService::Characteristic_ValueChanged);

    // In order to avoid unnecessary communication with the device, determine if the device is already 
    // correctly configured to send notifications.
    // By default ReadClientCharacteristicConfigurationDescriptorAsync will attempt to get the current
    // value from the system cache and communication with the device is not typically required.
    return create_task(this->characteristic->ReadClientCharacteristicConfigurationDescriptorAsync())
        .then([this](GattReadClientCharacteristicConfigurationDescriptorResult^ currentDescriptorValue)
    {

        if ((currentDescriptorValue->Status != GattCommunicationStatus::Success) ||
            (currentDescriptorValue->ClientCharacteristicConfigurationDescriptor != 
                GattClientCharacteristicConfigurationDescriptorValue::Notify))
        {
            // Set the Client Characteristic Configuration Descriptor to enable the device to send notifications
            // when the Characteristic value changes
            return create_task(this->characteristic->WriteClientCharacteristicConfigurationDescriptorAsync(
                GattClientCharacteristicConfigurationDescriptorValue::Notify))
                .then([this](GattCommunicationStatus status)
            {
                if (status == GattCommunicationStatus::Unreachable)
                {
                    // Register a PnpObjectWatcher to detect when a connection to the device is established,
                    // such that the application can retry device configuration.
                    StartDeviceConnectionWatcher();
                }
            });
        }
        // The lambda must return a continuation, but the prerequisites to call
        // WriteClientCharacteristicConfigurationDescriptorAsync were not met, and its continuation could not
        // be returned, so a placeholder continuation must be returned instead.
        return create_task([] {});
    });
}

/// <summary>
/// Register to be notified when a connection is established to the Bluetooth device
/// </summary>
void HeartRateService::StartDeviceConnectionWatcher()
{
    Vector<String^>^ additionalProperties = ref new Vector<String^>();
    additionalProperties->Append("System.Devices.Connected");

    watcher = PnpObject::CreateWatcher(PnpObjectType::DeviceContainer, additionalProperties, "");
    watcher->Updated += ref new TypedEventHandler<PnpObjectWatcher ^, PnpObjectUpdate ^>
        (this, &HeartRateService::OnDeviceConnectionUpdated);
    watcher->Start();
}

/// <summary>
/// Invoked when a connection is established to the Bluetooth device
/// </summary>
/// <param name="sender">The watcher object that sent the notification</param>
/// <param name="args">The updated device object properties</param>
void HeartRateService::OnDeviceConnectionUpdated(PnpObjectWatcher ^ /* sender */, PnpObjectUpdate^ e)
{
    auto connectedValue = reinterpret_cast<IPropertyValue^>(e->Properties->Lookup("System.Devices.Connected"));
    boolean isConnected = connectedValue->GetBoolean();
    if (deviceContainerId->Equals(e->Id) && isConnected)
    {
        // Set the Client Characteristic Configuration descriptor on the device, 
        // registering for Characteristic Value Changed notifications.
        create_task(characteristic->WriteClientCharacteristicConfigurationDescriptorAsync(
            GattClientCharacteristicConfigurationDescriptorValue::Notify))
            .then([this, isConnected](GattCommunicationStatus result)
        {
            if (result == GattCommunicationStatus::Success)
            {
                isServiceInitialized = true;

                // Once the Client Characteristic Configuration is set, the watcher is no longer required
                watcher->Stop();
                watcher = nullptr;
            }

            DeviceConnectionUpdated(isConnected);
        });
    }
}

/// <summary>
/// Invoked when Windows receives data from your Bluetooth device.
/// </summary>
/// <param name="sender">The GattCharacteristic object whose value is received.</param>
/// <param name="args">The new characteristic value sent by the device.</param>
void HeartRateService::Characteristic_ValueChanged(
    GattCharacteristic^ sender,
    GattValueChangedEventArgs^ args)
{
    auto heartRateMeasurementData = ref new Array<unsigned char>(args->CharacteristicValue->Length);

    DataReader::FromBuffer(args->CharacteristicValue)->ReadBytes(heartRateMeasurementData);

    auto heartRateValue = ProcessHeartRateMeasurementData(heartRateMeasurementData);

    // if correct data was received from the device, update the value
    if (heartRateValue != nullptr)
    {
        heartRateValue->Timestamp = args->Timestamp;

        {
            // Obtain RAII lock from dataLock
            reader_writer_lock::scoped_lock lock(dataLock);

            datapoints->Append(heartRateValue);
        }

        ValueChangeCompleted(heartRateValue);
    }
}

/// <summary>
/// Process the raw data received from the device into application usable data, according to the Heart Rate Profile.
/// </summary>
/// <param name="heartRateMeasurementData">Raw data received from the heart rate monitor.</param>
/// <returns>The heart rate measurement value.</returns>
HeartRateMeasurement^ HeartRateService::ProcessHeartRateMeasurementData(Array<unsigned char>^ heartRateMeasurementData)
{
    // Heart Rate Profile defined flags
    const unsigned char HEART_RATE_VALUE_FORMAT = 0x01;
    const unsigned char ENERGY_EXPENDED_STATUS = 0x08;

    try
    {
        unsigned char currentOffset = 0;
        unsigned char flags = heartRateMeasurementData[currentOffset];
        bool isHeartRateValueSizeLong = ((flags & HEART_RATE_VALUE_FORMAT) != 0);
        bool hasExpendedEnergy = ((flags & ENERGY_EXPENDED_STATUS) != 0);

        currentOffset++;

        uint16 heartRateMeasurementValue = 0;

        if (isHeartRateValueSizeLong)
        {
            heartRateMeasurementValue = (uint16) ((heartRateMeasurementData[currentOffset + 1] << 8) +
                heartRateMeasurementData[currentOffset]);
            currentOffset += 2;
        }
        else
        {
            heartRateMeasurementValue = heartRateMeasurementData[currentOffset];
            currentOffset++;
        }

        uint16 expendedEnergyValue = 0;

        if (hasExpendedEnergy)
        {
            expendedEnergyValue = (uint16) ((heartRateMeasurementData[currentOffset + 1] << 8) +
                heartRateMeasurementData[currentOffset]);
            currentOffset += 2;
        }

        // The Bluetooth Heart Rate profile can also contain sensor contact status information, and R-Wave interval
        // measurements, which can also be processed here. For the purpose of this sample, we don't need to interpret
        // that data.

        auto retval = ref new HeartRateMeasurement();
        retval->HeartRateValue = heartRateMeasurementValue;
        retval->HasExpendedEnergy = hasExpendedEnergy;
        retval->ExpendedEnergy = expendedEnergyValue;

        return retval;
    }
    catch (OutOfBoundsException^ e)
    {
        MainPage::Current->NotifyUser("Received malformed data from the device, which cannot be interpreted",
            NotifyType::ErrorMessage);

        return nullptr;
    }
}

/// <summary>
/// Process the raw data read from the device into an application usable string, according to the Bluetooth
/// Heart Rate Profile.
/// </summary>
/// <param name="bodySensorLocationData">Raw data read from the heart rate monitor.</param>
/// <returns>The textual representation of the Body Sensor Location.</returns>
String^ HeartRateService::ProcessBodySensorLocationData(const Array<unsigned char>^ bodySensorLocationData)
{
    // The Bluetooth Heart Rate Profile specifies that the Body Sensor Location characteristic value has
    // a single byte of data
    unsigned char bodySensorLocationValue = bodySensorLocationData[0];
    String^ retval = "";
    switch (bodySensorLocationValue)
    {
    case 0:
        retval += "Other";
        break;
    case 1:
        retval += "Chest";
        break;
    case 2:
        retval += "Wrist";
        break;
    case 3:
        retval += "Finger";
        break;
    case 4:
        retval += "Hand";
        break;
    case 5:
        retval += "Ear Lobe";
        break;
    case 6:
        retval += "Foot";
        break;
    default:
        // By default we simply return an empty string
        break;
    }
    return retval;
}
