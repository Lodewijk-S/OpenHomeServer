var openHomeServer = function(){
	var self = this;
	self.notificator = new notificator();
};

var notificator = function() {
    var self = this;
    toastr.options.closeButton = true;
    var notificationHub = $.connection.notificationHub;

    self.sendNotification = function (message, link, level) {
        notificationHub.server.notifyAll({ Message: message, Link: link, Level: level ? level : 0 });
    };

    notificationHub.client.onRecieveNotification = function (notification) {
        var message = notification.Message + "<br /><a href='" + notification.Link + "'>Click here for more info</a>";

        switch (notification.Level) {
            case 0:
                toastr.info(message, "Notification");
                break;
            case 1:
                toastr.warning(message, "");
                break;
            case 2:
                toastr.error(message, "");
                break;
            default:
                toastr.error(message + "(unknown level: " + notification.Level + ")", "");
                break;
        }
    };

    $.connection.hub.start()
        .done(function() { console.log('Now connected, connection ID=' + $.connection.hub.id); })
        .fail(function() { console.log('Could not Connect!'); });
};