/// <reference path="../Scripts/typings/angularjs/angular.d.ts" />

angular.module('TfSteno', ['ui.bootstrap']);

module TfSteno {
    export class Alert {
        constructor(public type: string, public msg: string) { }
    }

    export interface Registration {
        Email: string;
        TfsUrl: string;
        TfsUsername: string;
        TfsPassword: string;
    }

    export interface SignupScope extends ng.IScope {
        alerts: Alert[];
        busy: boolean;
        closeAlert(index: number): void;
        signUp(): void;
        registration: Registration;
    }

    export class SignupCtrl {
        public static $inject = ['$scope', '$http'];

        constructor ($scope: SignupScope, $http: ng.IHttpService) {
            $scope.alerts = [];
            $scope.busy = false;

            $scope.closeAlert = (index: number) => {
                $scope.alerts.splice(index, 1);
            };

            $scope.signUp = () => {
                $scope.alerts = [];
                $scope.busy = true;
                $http.post('/api/Registration',
                    angular.toJson($scope.registration))
                .then(
                    result => window.location.href = '/signup/complete?signupEmail=' + $scope.registration.Email,
                    error => {
                        $scope.busy = false;

                        var errorMessage = error.status === 403
                            ? 'Registration may only be performed using HTTPS. Please visit <a href="https://tfsteno.azurewebsites.net">https://tfsteno.azurewebsites.net</a>'
                            : error.data;
                        $scope.alerts.push(new Alert('error', errorMessage));
                    });
            };
        }
    }
}
