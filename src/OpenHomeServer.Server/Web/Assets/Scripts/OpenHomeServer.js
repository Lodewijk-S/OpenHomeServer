var openHomeServer = function(){
	me = this;
	me.notificator = new notificator();	
};

var notificator = function(){
	me = this;
	toastr.options.closeButton = true;
	var notificationHub = $.connection.notificationHub;
	
	me.sendNotification = function(message, link) {
        notificationHub.server.notifyAll({ Message: message, Link: link });
    }
     
    notificationHub.client.onRecieveNotification = function (notification) {
        toastr.info(notification.Message + "<br /><a href='"+notification.Link+"'>Click here for more info</a>", "Notification");
    };
	 
    $.connection.hub.start()
        .done(function () { console.log('Now connected, connection ID=' + $.connection.hub.id); })
        .fail(function () { console.log('Could not Connect!'); });     
}