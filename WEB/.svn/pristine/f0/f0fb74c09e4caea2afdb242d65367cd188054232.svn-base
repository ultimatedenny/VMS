$.prototype._getDateStr = function () {
    var d = this.datepicker("getDate");
    if (d == null)
    {
        return "";
    }
    return d._toISO()._left(10);
};

Array.prototype._indexOf = function (obj) {
    var search = -1;

    this._loop(function (i, v) {
        if (v === obj) {
            search = parseInt(i);
            return false;
        }
    });

    return search;
};

Array.prototype._loop = function (callback) {
    var i = 0;
    for (i = 0; i < this.length; i++) {
        var result = callback(i, this[i]);
        if (result !== false) {
            result = true;
        }
        if (!result) {
            break;
        }
    }
};

Array.prototype._remove = function (obj) {
    var i = this._indexOf(obj);
    this._removeAt(i);
}

Array.prototype._removeAll = function (obj) {
    var i = this._indexOf(obj);

    while (i >= 0)
    {
        this._removeAt(i);
        i = this.indexOf(obj);
    }
}

Array.prototype._removeAt = function (i) {
    if (i < this.length && i >= 0) {
        this.splice(i, 1);
    }
};

Date._weekDayStr = ["SUN", "MON", "TUE", "WED", "THU", "FRI", "SAT"];

Date.prototype._toISO = function () {
    return this.getFullYear() + "-" + ("0" + (this.getMonth() + 1))._right(2) + "-" + ("0" + this.getDate())._right(2) + "T" + ("0" + this.getHours())._right(2) + ":" + ("0" + this.getMinutes())._right(2) + ":" + ("0" + this.getSeconds())._right(2);
};

Date.prototype._weekDayStr = function () {
    return Date._weekDayStr[this.getDay()];
};

String.prototype._format = function () {
    var str = this.valueOf();
    var i = 0;
    for (i = 0; i < arguments.length; i++) {
        str = str.replace(new RegExp("\\{" + i + "\\}", "g"), arguments[i] + "");
    }
    return str;
};

String.prototype._left = function (n) {
    var str = this.valueOf();
    var strLength = str.length;

    if (n >= strLength) {
        return str;
    }
    else {
        return str.substr(0, n);
    }
};

String.prototype._right = function (n) {
    var str = this.valueOf();
    var strLength = str.length;

    if (n >= strLength) {
        return str;
    }
    else {
        return str.substr(strLength - n, n);
    }
};