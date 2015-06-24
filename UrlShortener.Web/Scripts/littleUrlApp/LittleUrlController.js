littleUrlApp.controller('LittleUrlController', [
    '$scope', 'littleUrlService', 'utility', function($scope, littleUrlService, utility) {

        $scope.createLittleUrl = function() {
            var alerter = utility.bootstrapAlerter($('#create .alertArea'));

            if (!$scope.bigUrl) {
                return;
            }

            littleUrlService.createLittleUrl($scope.bigUrl)
                .success(function(data) {
                    alerter.success('Created little url ' + utility.htmlEncode(data.littleUrl) + ' for ' + utility.htmlEncode($scope.bigUrl));
                })
                .error(function(data, status) {
                    switch (status) {
                    case 400:
                        alerter.danger('Sorry "' + utility.htmlEncode($scope.bigUrl) + '" is not a valid url.');
                        break;
                    case 500:
                        alerter.danger("Sorry, was not able to create little url. Please try again later.");
                        break;
                    case 503:
                        alerter.danger("Sorry Little Url service is down. :(");
                        break;
                    default:
                        alerter.danger("Oh no! An unknown error occurred.");
                    }
                });
        }

        $scope.previewLittleUrl = function() {
            var alerter = utility.bootstrapAlerter($('#preview .alertArea'));

            if (!$scope.littleUrl) {
                return;
            }

            littleUrlService.previewLittleUrl($scope.littleUrl)
                .success(function(data) {
                    alerter.success('Little url goes to "' + utility.htmlEncode(data.url) + '".');
                })
                .error(function(data, status) {
                    switch (status) {
                    case 400:
                        alerter.danger('Sorry "' + utility.htmlEncode($scope.littleUrl) + '" is not a little url.');
                        break;
                    case 404:
                        alerter.danger('Sorry, could find little url "' + utility.htmlEncode($scope.littleUrl) + '".');
                        break;
                    case 503:
                        alerter.danger('Sorry Little Url service is down. :(');
                        break;
                    default:
                        alerter.danger('Oh no! An unknown error occurred.');
                    }
                });
        }
    }
]);
