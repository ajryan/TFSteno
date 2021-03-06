angular.module('TfSteno', ['ui.bootstrap']);

var TfSteno;
(function (TfSteno) {
    var SignupCtrl = (function () {
        function SignupCtrl($scope, $http) {
            $scope.alerts = [];
            $scope.busy = false;

            $scope.closeAlert = function (index) {
                $scope.alerts.splice(index, 1);
            };

            $scope.signUp = function () {
                if ($scope.registrationForm.$invalid) {
                    $scope.alerts = [new TfSteno.Alert('warning', 'Please enter valid values for all fields.')];
                    return;
                }

                $scope.alerts = [];
                $scope.busy = true;
                $http.post('/api/Registration', angular.toJson($scope.registration)).then(function (result) {
                    return window.location.href = '/signup/complete?signupEmail=' + $scope.registration.Email;
                }, function (error) {
                    $scope.busy = false;

                    var errorMessage = error.status === 403 ? 'Registration may only be performed using HTTPS. Please visit https://tfsteno.azurewebsites.net' : error.data;
                    $scope.alerts.push(new TfSteno.Alert('error', errorMessage));
                });
            };
        }
        SignupCtrl.$inject = ['$scope', '$http'];
        return SignupCtrl;
    })();
    TfSteno.SignupCtrl = SignupCtrl;
})(TfSteno || (TfSteno = {}));
