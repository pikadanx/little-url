littleUrlApp.factory('littleUrlService', [
    '$http', function($http) {
        return {
            createLittleUrl: function(url) {
                return $http.post('./api/create', { url: url });
            },
            previewLittleUrl: function(littleUrl) {
                return $http.get('./api/preview?littleUrl=' + encodeURIComponent(littleUrl));
            }
        }
    }
]);