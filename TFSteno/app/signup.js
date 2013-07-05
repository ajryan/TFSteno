angular.module('TfSteno', ['ui.bootstrap']);

var TfSteno;
(function (TfSteno) {
    var Alert = (function () {
        function Alert(type, msg) {
            this.type = type;
            this.msg = msg;
        }
        return Alert;
    })();
    TfSteno.Alert = Alert;

    var SignupCtrl = (function () {
        function SignupCtrl($scope, $http) {
            $scope.alerts = [];
            $scope.busy = false;

            $scope.closeAlert = function (index) {
                $scope.alerts.splice(index, 1);
            };

            $scope.signUp = function () {
                $scope.alerts = [];
                $scope.busy = true;
                $http.post('/api/Registration', angular.toJson($scope.registration)).then(function (result) {
                    return window.location.href = '/signup/complete?signupEmail=' + $scope.registration.Email;
                }, function (error) {
                    $scope.busy = false;

                    var errorMessage = error.status === 403 ? 'Registration may only be performed using HTTPS. Please visit <a href="https://tfsteno.azurewebsites.net">https://tfsteno.azurewebsites.net</a>' : error.data;
                    $scope.alerts.push(new Alert('error', errorMessage));
                });
            };
        }
        SignupCtrl.$inject = ['$scope', '$http'];
        return SignupCtrl;
    })();
    TfSteno.SignupCtrl = SignupCtrl;
})(TfSteno || (TfSteno = {}));
