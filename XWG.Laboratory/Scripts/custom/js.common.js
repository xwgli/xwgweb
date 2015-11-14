/*
 作者：李宏
 时间：2014-01-14
 说明：将json串格式的日期转化成javascript的日期对象
*/
function ConvertJSONDateToJSDate(jsonDateString) {
    //利用正则表达式清除字符串中其余部分，剩下的是秒数
    var date = new Date(parseInt(jsonDateString.replace("/Date(", "").replace(")/", ""), 10));
    return date;
}

/*
 收集：李宏
 时间：2014-02-14
 来源：网络
 说明：自定义时间对象处理
 有参方法：
	date.format（自定义格式化时间，返回字符串）
*/
// 自定义格式化时间，返回字符串
// 月(M)、日(d)、小时(h)、分(m)、秒(s)、季度(q) 可以用 1-2 个占位符
// 年(y)可以用 1-4 个占位符，毫秒(S)只能用 1 个占位符(是 1-3 位的数字)
Date.prototype.format = function (format) {
    var date = {
        "M+": this.getMonth() + 1,
        "d+": this.getDate(),
        "h+": this.getHours(),
        "m+": this.getMinutes(),
        "s+": this.getSeconds(),
        "q+": Math.floor((this.getMonth() + 3) / 3),
        "S+": this.getMilliseconds()
    };
    if (/(y+)/i.test(format)) {
        format = format.replace(RegExp.$1, (this.getFullYear() + '').substr(4 - RegExp.$1.length));
    }
    for (var k in date) {
        if (new RegExp("(" + k + ")").test(format)) {
            format = format.replace(RegExp.$1, RegExp.$1.length == 1
                   ? date[k] : ("00" + date[k]).substr(("" + date[k]).length));
        }
    }
    return format;
}