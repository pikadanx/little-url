littleUrlApp.controller('LittleUrlController', [
    '$scope', 'littleUrlService', function($scope, littleUrlService) {

        $scope.createLittleUrl = function() {
            littleUrlService.createLittleUrl($scope.bigUrl)
                .success(function() {
                    alert(JSON.stringify(arguments));
                })
                .error(function() {
                    alert(JSON.stringify(arguments));
                });
        }

        $scope.previewLittleUrl = function() {
            littleUrlService.previewLittleUrl($scope.littleUrl)
                .success(function() {
                    alert(JSON.stringify(arguments));
                })
                .error(function() {
                    alert(JSON.stringify(arguments));
                });
        }
    }
]);