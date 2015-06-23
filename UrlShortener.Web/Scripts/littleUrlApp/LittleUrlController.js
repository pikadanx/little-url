littleUrlApp.controller('LittleUrlController', [
    '$scope', 'littleUrlService', function($scope, littleUrlService) {

        $scope.createLittleUrl = function () {
            var alerter = bootstrapAlerter($('#create .alertArea'));

            if (!$scope.bigUrl) {
                return;
            }

            littleUrlService.createLittleUrl($scope.bigUrl)
                .success(function (data) {
                    alerter.success('Created little url ' + htmlEncode(data.littleUrl) + ' for ' + htmlEncode($scope.bigUrl));
                })
                .error(function(data, status) {
                    switch (status) {
                    case 400:
                        alerter.danger('Sorry "' + htmlEncode($scope.bigUrl) + '" is not a valid url.');
                        break;
                    case 503:
                        alerter.danger("Sorry Little Url service is down. :(");
                        break;
                    default:
                        alerter.danger("Oh no! An unknown error occurred.");
                    }
                });
        }

        $scope.previewLittleUrl = function () {
            var alerter = bootstrapAlerter($('#preview .alertArea'));

            if (!$scope.littleUrl) {
                return;
            }

            littleUrlService.previewLittleUrl($scope.littleUrl)
                .success(function(data) {
                    alerter.success('Little url goes to "' + htmlEncode(data.url) + '".');
                })
                .error(function(data, status) {
                    switch (status) {
                    case 400:
                        alerter.danger('Sorry "' + htmlEncode($scope.littleUrl) + '" is not a little url.');
                        break;
                    case 404:
                        alerter.danger('Sorry, could find little url "' + htmlEncode($scope.littleUrl) + '".');
                        break;
                    case 503:
                        alerter.danger('Sorry Little Url service is down. :(');
                        break;
                    default:
                        alerter.danger('Oh no! An unknown error occurred.');
                    }
                });
        }

        function bootstrapAlerter(area) {
            var $alertArea = area;

            function alert(type, message) {
                $alertArea.empty();
                $alertArea.append(getBootstrapAlertHtml(type, message));
                $alertArea.children('.alert').addClass('in');
            }

            function getBootstrapAlertHtml(type, message) {
                var $element = $('<div class="alert alert-' + type + ' alert-dismissible fade" role="alert">\
  <button type="button" class="close" data-dismiss="alert" aria-label="Close"><span aria-hidden="true">&times;</span></button>\
</div>');
                $element.append(message);
                return $element;
            }

            return {
                danger: function(message) {
                    alert('danger', message);
                },
                success : function(message) {
                    alert('success', message);
                }
            };
        }

        function htmlEncode(value) {
            return $('<div/>').text(value).html();
        }
    }
]);
