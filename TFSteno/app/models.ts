/// <reference path="../Scripts/typings/angularjs/angular.d.ts" />

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
        registrationForm: any;
        registration: Registration;
    }
}