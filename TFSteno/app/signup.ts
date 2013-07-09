/// <reference path="../Scripts/typings/angularjs/angular.d.ts" />
/// <reference path="models.ts" />

angular.module('TfSteno', ['ui.bootstrap']);

module TfSteno {

    export class SignupCtrl {
        public static $inject = ['$scope', '$http'];

        constructor ($scope: SignupScope, $http: ng.IHttpService) {
            $scope.alerts = [];
            $scope.busy = false;

            $scope.closeAlert = (index: number) => {
                $scope.alerts.splice(index, 1);
            };

            $scope.signUp = () => {
                if ($scope.registrationForm.$invalid) {
                    $scope.alerts = [new Alert('warning','Please enter valid values for all fields.')];
                    return;
                }

                $scope.alerts = [];
                $scope.busy = true;
                $http.post('/api/Registration',
                    angular.toJson($scope.registration))
                .then(
                    result => window.location.href = '/signup/complete?signupEmail=' + $scope.registration.Email,
                    error => {
                        $scope.busy = false;

                        var errorMessage = error.status === 403
                            ? 'Registration may only be performed using HTTPS. Please visit https://tfsteno.azurewebsites.net'
                            : error.data;
                        $scope.alerts.push(new Alert('error', errorMessage));
                    });
            };
        }
    }
}
