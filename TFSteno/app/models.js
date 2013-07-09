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
})(TfSteno || (TfSteno = {}));
