//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/scenario10.html", {
        ready: function (element, options) {
            document.getElementById("RunSample").addEventListener("click", RunSample, false);
        }
    });

    function RunSample() {
        var descriptor = tbDescriptor.value;
        Scenario10Output.textContent = "";

        Scenario10Output.textContent += "*** Sample Stream Data Protection for " + descriptor + " ***\n";

        var data = Windows.Security.Cryptography.CryptographicBuffer.generateRandom(10000);
        var reader1;
        var reader2;
        var buff1;
        var buff2;

        var Provider = new Windows.Security.Cryptography.DataProtection.DataProtectionProvider(descriptor);
        var originalData = new Windows.Storage.Streams.InMemoryRandomAccessStream();

        //Populate the new memory stream
        var outputStream = originalData.getOutputStreamAt(0);
        var writer = new Windows.Storage.Streams.DataWriter(outputStream);
        writer.writeBuffer(data);
        writer.storeAsync().done (function () {
            outputStream.flushAsync().done (function () {
                writer.close();
                //open new memory stream for read
                var source = originalData.getInputStreamAt(0);

                //Open the output memory stream
                var protectedData = new Windows.Storage.Streams.InMemoryRandomAccessStream();
                var dest = protectedData.getOutputStreamAt(0);

                // Protect
                Provider.protectStreamAsync(source, dest).done(function () {
                    //Flush the output
                    dest.flushAsync().done(function () {
                        Scenario10Output.textContent += "    Protected output was successfully flushed\n";

                        //Verify the protected data does not match the original
                        reader1 = new Windows.Storage.Streams.DataReader(originalData.getInputStreamAt(0));
                        reader2 = new Windows.Storage.Streams.DataReader(protectedData.getInputStreamAt(0));

                        reader1.loadAsync(originalData.size).done (function () {
                            reader2.loadAsync(protectedData.size).done (function () {
                                Scenario10Output.textContent += "    Size of original stream:  " + originalData.size + "\n";
                                Scenario10Output.textContent += "    Size of protected stream:  " + protectedData.size + "\n";

                                if (originalData.size === protectedData.size)
                                {
                                    buff1 = reader1.readBuffer(originalData.size);
                                    buff2 = reader2.readBuffer(protectedData.size);
                                    if (CryptographicBuffer.compare(buff1, buff2))
                                    {
                                        Scenario10Output.textContent += "ProtectStreamAsync returned unprotected data";
                                        return;
                                    }
                                }

                                Scenario10Output.textContent += "    Stream Compare completed.  Streams did not match.\n";

                                source = protectedData.getInputStreamAt(0);

                                var unprotectedData = new Windows.Storage.Streams.InMemoryRandomAccessStream();
                                dest = unprotectedData.getOutputStreamAt(0);

                                // Unprotect
                                var Provider2 = new Windows.Security.Cryptography.DataProtection.DataProtectionProvider();
                                Provider2.unprotectStreamAsync(source, dest).done(function () {
                                    dest.flushAsync().done(function () {
                                        Scenario10Output.textContent += "    Unprotected output was successfully flushed\n";

                                        //Verify the unprotected data does match the original
                                        reader1 = new Windows.Storage.Streams.DataReader(originalData.getInputStreamAt(0));
                                        reader2 = new Windows.Storage.Streams.DataReader(unprotectedData.getInputStreamAt(0));

                                        reader1.loadAsync(originalData.size).done(function () {
                                            reader2.loadAsync(unprotectedData.size).done(function () {

                                                Scenario10Output.textContent += "    Size of original stream:  " + originalData.size + "\n";
                                                Scenario10Output.textContent += "    Size of unprotected stream:  " + unprotectedData.size + "\n";

                                                buff1 = reader1.readBuffer(originalData.size);
                                                buff2 = reader2.readBuffer(unprotectedData.size);
                                                if (!Windows.Security.Cryptography.CryptographicBuffer.compare(buff1, buff2)) {
                                                    Scenario10Output.textContent += "UnrotectStreamAsync did not return expected data";
                                                    return;
                                                }

                                                originalData.close();
                                                protectedData.close();
                                                unprotectedData.close();
                                                reader1.close();
                                                reader2.close();

                                                Scenario10Output.textContent += "*** Done!\n";
                                            }, function() {
                                                originalData.close();
                                                protectedData.close();
                                                unprotectedData.close();
                                                reader1.close();
                                                reader2.close();
                                            });
                                        }, function() {
                                            originalData.close();
                                            protectedData.close();
                                            unprotectedData.close();
                                            reader1.close();
                                            reader2.close();
                                        });
                                    }, function() {
                                        originalData.close();
                                        protectedData.close();
                                        unprotectedData.close();
                                        reader1.close();
                                        reader2.close();
                                    });
                                }, function() {
                                    originalData.close();
                                    protectedData.close();
                                    unprotectedData.close();
                                    reader1.close();
                                    reader2.close();
                                });
                            }, function() {
                                originalData.close();
                                protectedData.close();
                                reader1.close();
                                reader2.close();
                            });
                        }, function() {
                            originalData.close();
                            protectedData.close();
                            reader1.close();
                            reader2.close();
                        });
                    }, function() {
                        originalData.close();
                        protectedData.close();
                    });
                }, function() {
                    originalData.close();
                    protectedData.close();
                });
            }, function() {
                originalData.close();
            });
        }, function() {
            originalData.close();
            writer.close();
        });
    }
})();
