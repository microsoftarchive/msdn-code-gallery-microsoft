function processOrder() {

    serviceBusService.createQueueIfNotExists('orders', function(error) { 
        if (!error) {
            serviceBusService.receiveQueueMessage('orders', receiveMessageCallback);
        }
        else {
            console.error(error);
        } 
    });
}

var azure = require('azure'); 
var serviceBusService = azure.createServiceBusService('{namespace name}', '{namespace key}'); 
var receivedMessageCount = 0;
var notificationSent = false;

function receiveMessageCallback(error, receivedMessage) {

    if (!error){
        receivedMessageCount++;

        var order = JSON.parse(receivedMessage.body);

        var deliveryOrderTable = tables.getTable('DeliveryOrder');
        deliveryOrderTable.insert({ 
            product: order.product,
            quantity: order.quantity,
            customer: order.customer,
            delivered: false                    
        }, {
            success: function() {
                if (!notificationSent) {
                    sendPushNotification('/Images/Logo.png', 'New orders', 'One or more orders have been placed');
                    notificationSent = true;
                }

                if (receivedMessageCount < 10) {
                    // continue receiving messages until we process a batch of 10 messages
                    serviceBusService.receiveQueueMessage('orders', receiveMessageCallback);
                }
            },
            error: errorHandler
        });
    }
}

function sendPushNotification(imagesrc, title, line1) {
    var channelTable = tables.getTable('Channel');
    channelTable.read({
        success: function(channels) {
            channels.forEach(function (channel) {
               push.wns.sendToastImageAndText02(channel.uri, {
                    image1src: imagesrc,
                    text1: title,
                    text2: line1
                });
            });
        }
    });
}

function errorHandler(error){
     console.error(error);
}
