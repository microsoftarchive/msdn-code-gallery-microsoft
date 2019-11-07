var SendGrid = require('sendgrid').SendGrid;

function insert(item, user, request) {    
     request.execute({
          success: function() {
                // After the record has been inserted, send the response immediately to the client
                request.respond();
                // Send the email in the background
                sendEmail(item);
          }
     });

     function sendEmail(item) {
          var sendgrid = new SendGrid('{username}', '{password}');

          sendgrid.send({
                to: '{email-address}',
                from: 'feedback@mydomain.com',
                subject: 'Feedback Submitted',
                text: 'A new feedback has been submitted: ' + item.text
          }, function(success, message) {
                // If the email failed to send, log it as an error so we can investigate
                if (!success) {
                     console.error(message);
                }
          });
     }
}
