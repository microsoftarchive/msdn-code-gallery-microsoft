function insert(item, user, request) {
    var insertInQueue = function() {
        var azure = require('azure');
        var serviceBusService = azure.createServiceBusService('{namespace name}', '{namespace key}');

        serviceBusService.createQueueIfNotExists('orders', function(error) {
            if (!error) {
                serviceBusService.sendQueueMessage('orders', JSON.stringify(item), function(error) {
                    if (!error) {
                        console.log('sent message: ' + item.id);
                    }
                });
            }
        });
    };

    request.execute({
        success: function () {
            insertInQueue();
            request.respond();
        }
    });
}
