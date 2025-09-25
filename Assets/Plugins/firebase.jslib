mergeInto(LibraryManager.library, {

  GetJson: function (path, objectName, callback, fallback) {
    var parsedPath = UTF8ToString(path);
    var parsedObjectName = UTF8ToString(objectName);
    var parsedCallback = UTF8ToString(callback);
    var parsedFallback = UTF8ToString(fallback);

    window.alert("Hello, world!");

    try {
        firebase.database().ref(parsedPath).once('value').then(function(snapshot) {
        window.unityInstance.sendMessage(parsedObjectName, parsedCallback, JJSON.stringify(snapshot.val()));
        });
    } 
    catch (error) {
        firebase.database().ref(parsedPath).once('value').then(function(snapshot) {
        window.unityInstance.sendMessage(parsedObjectName, parsedFallback, "There was an error: " + error.message);
        });
    }
  }
});