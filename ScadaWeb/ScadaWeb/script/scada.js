﻿// Окно календаря для выбора даты
var calendarWin = null;

// Получить значение параметра запроса
function GetParamVal(paramName, url) {
    if (paramName) {
        var queryString = url ? url : unescape(window.location);
        var begInd = queryString.indexOf("?");
        if (begInd > 0)
            queryString = "&" + queryString.substring(begInd + 1);

        paramName = "&" + paramName + "=";
        begInd = queryString.indexOf(paramName);

        if (begInd >= 0) {
            begInd += paramName.length;
            var endInd = queryString.indexOf("&", begInd);
            if (endInd < 0)
                endInd = queryString.length;

            return queryString.substring(begInd, endInd);
        }
    }

    return "";
}

// Открыть дочернее окно
function OpenWin(url, winName, width, height) {
    var features = "";

    if (width) {
        features += "width=" + width + ",";
        features += "left=" + (screen.width - width) / 2 + ",";
    }

    if (height) {
        features += "height=" + height + ",";
        features += "top=" + (screen.height - height) / 2 + ",";
    }

    return window.open(url, winName, features + "toolbar=no,status=no,scrollbars=yes,resizable=yes");
}

// Открыть календарь для выбора даты в заданное текстовое поле
function ShowCalendar(txtID, date, path) {
    CloseCalendar();
    calendarWin = window.open(path + "Calendar.aspx?txtID=" + txtID + "&date=" + date, "CalendarWin",
        "width=200,height=180,left=" + event.screenX + ",top=" + event.screenY +
        ",toolbar=no,status=no,scrollbars=no,resizable=no");
}

// Закрыть календарь
function CloseCalendar() {
    if (calendarWin && !calendarWin.closed)
        calendarWin.close();
}

// Получить значение cookie по имени
function GetCookie(name) {
    var cookie = " " + window.document.cookie;
    var search = " " + name + "=";
    var offset = cookie.indexOf(search);

    if (offset >= 0) {
        offset += search.length;
        var end = cookie.indexOf(";", offset)

        if (end < 0)
            end = cookie.length;

        return cookie.substring(offset, end);
    } else {
        return null;
    }
}

// Установить cookie
function SetCookie(name, value, daysCount) {
    var expireDate = new Date();
    expireDate.setDate(expireDate.getDate() + daysCount);
    window.document.cookie = name + "=" + value + "; expires=" + expireDate.toGMTString();
}

// Определить количество дней в месяце
function DaysInMonth(year, month) {
    return new Date(year, month, 0).getDate();
}

// Получить адрес страницы графика, начиная с файла веб страницы
function GetDiagUrl(viewSet, view, year, month, day, period, cnlNum) {
    return "Diag.aspx?viewSet=" + viewSet + "&view=" + view +
        "&year=" + year + "&month=" + month + "&day=" + day +
        (period > 1 ? "&period=" + period : "") + "&cnlNum=" + cnlNum;
}

// Открыть форму графика входного канала
function ShowDiag(viewSet, view, year, month, day, cnlNum, path) {
    window.open((path ? path : "") + "diag/" + GetDiagUrl(viewSet, view, year, month, day, 1, cnlNum));
}

// Открыть форму отправки команды телеуправления
function SendCmd(viewSet, view, ctrlCnlNum, path) {
    OpenWin((path ? path : "") + "CmdSend.aspx?viewSet=" + viewSet + "&view=" + view + "&ctrlCnlNum=" + ctrlCnlNum,
        "CmdSendWin", 400, 350);
}

// Открыть форму квитирования события
function CheckEvent(viewSet, view, year, month, day, evNum) {
    OpenWin("EvCheck.aspx?viewSet=" + viewSet + "&view=" + view + "&year=" + year + 
    "&month=" + month + "&day=" + day + "&evNum=" + evNum, "EvCheckWin", 400, 300);
}