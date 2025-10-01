/* jslint esversion: 6  */

import {
  setup
} from './app';

import {
  dict
} from './dictionary';

import {
  getLinkStatus
} from './dbSys';

const
  config = {
    pageIdPattern: /[A-Za-z]{3,3}[0-9]{4,4}/,
    userInfoTag: 'userInfo',
    startOfWeek: 0,               // 0=Sunday
    dateFormatISO: 'yyyy-MM-dd',
    timeFormatISO: 'HH:mm:ss',
    timePeriodAM: 'AM',
    timePeriodPM: 'PM',
    timePeriodA: 'A',
    timePeriodP: 'P',
    spellCheck: false,
    adviceWidth: 300,
    closeModalOnBackButton: true,
    buttonScheme: 'mono',
    msgEntryRequired: 'Entry is required.',
    msgEntryRejected: 'Entry rejected.',
    msgMinLength: 'Minimum length is ',
    msgMinValue: 'Minimum value is ',
    msgMaxValue: 'Maximum value is ',

    get loaderScheme () { return 'warning'; }, // primary, info, success, warning, danger, white
    get loaderIcon () { return 'spinner'; },

    get themeId () { return 'info'; },         // primary, info, success, warning, danger, dark
    get tableBodyColor () { return ''; },      // global color override for tables (all themes)
    get theme () { return getTheme(this.themeId); },
    get themes () { return ['primary', 'info', 'success', 'warning', 'danger', 'dark']; }
  },

  numKeys = ['1','2','3','4','5','6','7','8','9','0'],
  editKeys = ['Escape','Home','End','ArrowRight','ArrowLeft','Tab','Enter','Backspace','Delete','F5'],
  dateSepKeys = ['/','.','-',' '],
  intKeys = numKeys.concat(editKeys),
  decKeys = intKeys.concat(['.']),
  dateKeys = intKeys.concat(dateSepKeys),

  sessionDb = {
    bin: sessionStorage,
    get (key) {
      const v = this.bin.getItem(key);
      if (typeof v === 'string') {
        try { return JSON.parse(v); }
        catch (e) { return v || null; }
      } else {
        return v;
      }
    },
    set (key, value) {
      let v = value;
      if ( typeof v === 'object' ) {
        v = JSON.stringify(v);
      }
      this.bin.setItem(key, v);
    },

    remove (key) {
      this.bin.removeItem(key);
    },

    clear () {
      this.bin.clear();
    }
  },

  localDb = Object.assign({}, sessionDb, { bin: localStorage }),

  sessionMgr = {
    _page: null,
    _modal: null,

    setCurrentPage (pageInstance) {
      this._page = pageInstance;
    },

    setCurrentModal (modalInstance) {
      this._modal = modalInstance;
    },

    get currentPage () { return this._page; },
    get currentPageElement () { return this._page ? this._page.$el : null; },
    get currentPageMeta () { return this._page ? this._page.meta : null; },

    get currentModal () { return this._modal; },
    get currentModalElement () { return this._modal ? this._modal.$el : null; },
    get currentMOdalMeta () { return this._modal ? this._modal.meta : null; },
  },

  store = {},

  regExYearLast = /^(\d{1,2})\/(\d{1,2})\/(\d{0,4})$/,  // m/d/y or d/m/y
  regExYearFirst = /^(\d{4})\/(\d{1,2})\/(\d{1,2})$/,   // y/m/d

  // https://stackoverflow.com/questions/7536755/regular-expression-for-matching-hhmm-time-format/7536768
  regEx12Hours = /((1[0-2]|0?[1-9])[:.]([0-5][0-9]) ?([AaPp][Mm]))/,
  regEx24Hours = /^(0[0-9]|1[0-9]|2[0-3])[0-5][0-9]$/,

  // https://stackoverflow.com/questions/46155/how-to-validate-an-email-address-in-javascript
  regExEmail = /^([a-zA-Z0-9_.-])+@(([a-zA-Z0-9-])+\.)+([a-zA-Z0-9]{2,4})+$/,

  dayNamesShort = getDayNames(),
  dayNamesLong = getDayNames('long'),
  monthNamesShort = getMonthNames(),
  monthNamesLong = getMonthNames('long');

let router = null;

export function init (vueRouter) {
  router = vueRouter;

  window.scrollToTop = () => {
    let currentScroll = document.documentElement.scrollTop || document.body.scrollTop;
    if (currentScroll > 0) {
      window.requestAnimationFrame(window.scrollToTop);
      window.scrollTo(0, Math.floor(currentScroll - (currentScroll / 5)));
    }
  };

}

function parseDate (dateText, isoFormat = false) {
  let
    y = 0,
    m = 0,
    d = 0,
    text = dateText ? dateText.trim().substr(0, 10).replace(/\./g,'/').replace(/ /g,'/').replace(/-/g,'/') : '',
    fmt = (isoFormat ? config.dateFormatISO.replace(/-/g,'/') : setup.dateFormat).toLowerCase(),
    rx = fmt === 'yyyy/mm/dd' ? regExYearFirst : regExYearLast,
    arr;

  if (text.length > 2 && text.length < 7) {
    let year = DateTime.today().year;
    arr = (text.endsWith('/') ? text + year : text + '/' + year).match(rx);
  } else {
    arr = text.match(rx);
  }

  if (!arr) { return null; }

  switch (fmt) {
    case 'mm/dd/yyyy':
      y = arr[3];
      m = arr[1];
      d = arr[2];
      break;

    case 'dd/mm/yyyy': {
      y = arr[3];
      m = arr[2];
      d = arr[1];
      break;
    }

    default: {
      y = arr[1];
      m = arr[2];
      d = arr[3];
    }
  }

  if (+d > daysInMonth(+y, +m) || +d < 1) { return null; }   // + converts to number
  if (+m < 1 || +m > 12) { return null; }
  return { year: +y, month: +m, day: +d };
}

function parseTime (timeText, isoFormat = false) {
  let
    h,
    m,
    result;

  if (!timeText) {
    return null;
  }

  if (isoFormat) {
    if (timeText.length > 5) {
      timeText = timeText.substr(0, 5).replace(':','');
    }
    result = timeText.match(regEx24Hours);
    // 16 May 2025 - EMT
    if (!result || (result[0] !== timeText)) {
      return null;
    }
    h = Number(timeText.substr(0, 2));
    m = Number(timeText.substr(2, 2));
    if (h === 24) {
      return null;
    }
  } else {
    result = timeText.match(regEx12Hours);
    if (!result || (result[0] !== timeText)) {
      return null;
    }
    h = Number(result[2]);
    m = Number(result[3]);

    if (result[4].toLowerCase() === 'pm' && (h < 12)) {
      h = h + 12;
    }
  }
  return { hour: h, minute: m };
}

export function getDayNames (format = 'short') {
  let start = config.startOfWeek;
  if (start < 0) { start = 0; }
  if (start > 6) { start = 1; }
  let locale = setup ? setup.locale : 'en-US';
  let names = [];
  for (let day = 4 + start; day <= 10 + start; day++) {
    names.push(
      new Date(1970, 0, day).toLocaleString(locale, { weekday: format })
    );
  }
  return names;
}

export function getMonthNames (format = 'short') {
  let
    date = new Date(1970, 0, 1),
    locale = setup ? setup.locale : 'en-US',
    names = [];

  names.push('');

  for (let m = 0; m < 12; m++) {
    date.setMonth(m);
    names.push(
      date.toLocaleString(locale, { month: format })
    );
  }
  return names;
}

export function getCurrentYear () {
  return (new DateTime()).year;
}

export function getCurrentMonth () {
  return (new DateTime()).month;
}

export function getMonthName (month) {
  return monthNamesLong[month];
}

export function getMonthShortName (month) {
  return monthNamesShort[month];
}

export function daysInMonth (year, month) {
  return new Date(year, month, 0).getDate();
}

function toDateTime (date) {
  return new DateTime(date.getFullYear(), date.getMonth() + 1, date.getDate());
}

export function isLeapYear (year) {
  return ((year % 4 === 0) && (year % 100 !== 0)) || (year % 400 === 0);
}

function div (a, b) {
  return Math.floor(a / b);
}

function pad (n) {
  return (n < 10 ? '0' : '') + n;
}

export function getScrollbarWidth () {
  let div = document.createElement("div");
  div.className = "scrollbar-measure";
  document.body.appendChild(div);

  let width = div.offsetWidth - div.clientWidth;
  document.body.removeChild(div);
  return width - 2;
}

// constructor
function DateTime (...a) {
  let
    y = 0,
    mo = 0,
    d = 0,
    h = 0,
    m = 0,
    s = 0,
    ms = 0,
    me = this;

  switch (a.length) {
    case 0: {
      const n = new Date();
      y = n.getFullYear();
      mo = n.getMonth() + 1;
      d = n.getDate();
      h = n.getHours();
      m = n.getMinutes();
      s = n.getSeconds();
      ms = n.getMilliseconds();
      break;
    }

    case 1: {
      let p = a[0];
      if (typeof p === 'string') {
        let n = parseDate(p);
        if (n) {
          y = n.year;
          mo = n.month;
          d = n.day;
          if (p.includes(":")) {
            let dt = new Date(p);
            h = dt.getHours();
            m = dt.getMinutes();
            s = dt.getSeconds();
            ms = dt.getMilliseconds();
          }
        }
      } else {
        ms = p;
      }
      break;
    }

    case 2: {
      let
        p = a[0],
        b = a[1];

      if (typeof p === 'string' && typeof b === 'boolean') {
        let n = parseDate(p, b);
        if (n) {
          y = n.year;
          mo = n.month;
          d = n.day;
          if (p.includes(":")) {
            let dt = new Date(p);
            h = dt.getHours();
            m = dt.getMinutes();
            s = dt.getSeconds();
            ms = dt.getMilliseconds();
          }
        }
      } else {
        y = p;
        mo = b;
        d = 1;
      }

      break;
      }
    case 3:
      y = a[0];
      mo = a[1];
      d = a[2];
      break;

    case 6:
      y = a[0];
      mo = a[1];
      d = a[2];
      h = a[3];
      m = a[4];
      s = a[5];
      break;

    case 7:
      y = a[0];
      mo = a[1];
      d = a[2];
      h = a[3];
      m = a[4];
      s = a[5];
      ms = a[6];
      break;

    default:
      throw('No DateTime constructor supports ' + a.length + ' arguments');
  }

  // check for date rollover
  let date = new Date(y, mo-1, d, h, m, s, ms);
  if (date.getFullYear() !== y || date.getMonth() + 1 !== mo) {
    y = 0;
    mo = 0;
    d = 0;
  }

  let days;
  if (!y && !mo && !d) {
    days = 0;
  } else {
    days = Math.round(me.absoluteDays(y, mo, d));
  }

  me.span = new TimeSpan(days, h, m, s, ms);

  let
    _y,
    _m,
    _d,
    _dw,
    _dy,
    _td,
    dp = Object.defineProperty;

 if (days) {
    _y = y;
    _m = mo;
    _d = d;
  } else {
    _y = me.fromSpan('year');
    _m = me.fromSpan('month');
    _d = me.fromSpan('day');
  }

  _dw = (me.span.days + 1) % 7;
  _dy = me.fromSpan('dayyear');
  _td = new TimeSpan(me.span.ms % 86400000);

  // Properties
  dp(me, 'year', { get() { return _y; } });
  dp(me, 'month', { get() { return _m; } });
  dp(me, 'day', { get() { return _d; } });
  dp(me, 'hour', { get() { return me.span.hours; } });
  dp(me, 'minute', { get() { return me.span.minutes; } });
  dp(me, 'second', { get() { return me.span.seconds; } });
  dp(me, 'millisecond', { get() { return me.span.milliseconds; } });
  dp(me, 'dayOfWeek', { get() { return _dw; } });
  dp(me, 'dayOfYear', { get() { return _dy; } });
  dp(me, 'timeOfDay', { get() { return _td; } });
}

DateTime.prototype = {

  toJSON () {
    return this.toDateFormatISO();
  },

  toString () {
    if (this.year === 1) {
      return 'Invalid date';
    } else {
      return this.format("yyyy-MM-dd ddd");
    }
  },

  toDateFormat (format = setup.dateFormat) {
    return this.format(format);
  },

  toDateDisplayFormat () {
    return this.format(setup.dateDisplayFormat);
  },

  toDateTimeDisplayFormat () {
    return this.format(setup.dateDisplayFormat + ' ● h:mm tt');
  },

  toDateFormatISO () {
    return this.format(config.dateFormatISO);
  },

  endOfMonth () {
    return DateTime.endOfMonth(this.year, this.month);
  },

  absoluteDays (year, month, day) {
    let arr = isLeapYear(year) ? new Array(0, 31, 60, 91, 121, 152, 182, 213, 244, 274, 305, 335, 366) :  new Array(0, 31, 59, 90, 120, 151, 181, 212, 243, 273, 304, 334, 365);
    let y = year - 1;
    let days = (y * 365) + div(y, 4) - div(y, 100) + div(y,400) + arr[month - 1] + day - 1;
    return days;
  },

  add (span) {
    return new DateTime(this.span.ms + span.ms);
  },

  addDays (days) {
    return new DateTime(this.span.ms + (days * 86400000));
  },

  addMonths (months) {
    let day = this.day;
    let month = this.month + ( months % 12 );
    let year = this.year + Math.round(months / 12);

    if (month < 1){
      month = 12 + month;
      year--;
    } else if (month > 12) {
      month -=12;
      year++;
    }

    let days = daysInMonth(year, month);
    if (day > days) { day = days; }

    let time = new DateTime(year, month, day);
    return time.add(this.timeOfDay);
  },

  addYears (years) {
    return this.addMonths(years * 12);
  },

  addHours (hours) {
    return new DateTime(this.span.ms + (hours * 3600000));
  },

  addMinutes (minutes) {
    return new DateTime(this.span.ms + (minutes * 60000));
  },

  addSeconds (seconds) {
    return new DateTime(this.span.ms + (seconds * 1000));
  },

  addMilliseconds (ms) {
    return new DateTime(this.span.ms + ms);
  },

  compare (datetime) {
    return DateTime.compare(this, datetime);
  },

  equals (datetime) {
    return DateTime.equals(this, datetime);
  },

  empty () {
    return new DateTime(1,1,1);
  },

  between (minDate, maxDate) {
    return DateTime.between(this, minDate, maxDate);
  },

  isEmpty () {
    return (this.year + this.month + this.day === 3);
  },

  subtractDate (datetime) {
    return new TimeSpan(this.span.ms - datetime.span.ms);
  },

  subtractTime (span) {
    return new DateTime(this.span.ms - span.ms);
  },

  fromSpan (w) {
    let n2 = this.span.totalDays;
    let n3 = div(n2, 146097); // days in 4 centuries
    n2 -= n3 * 146097;
    let n4 = div(n2, 36524);  // days in 1 century
    if (n4 === 4) {
      n4 = 3;
    }
    n2 -= n4 * 36524;
    let n5 = div(n2, 1461);   // days in 4 years (365.25 per year)
    n2 -= n5 * 1461;
    let n6 = div(n2, 365);    // days in 1 year
    if (n6 === 4) {
      n6 = 3;
    }
    if (w === 'year') {
      return (n3 * 400) + (n4 * 100) + (n5 * 4) + n6 + 1;
    }
    n2 -= n6 * 365;
    if (w === 'dayyear') {
      return (n2 + 1);
    }
    let arr = (n6 === 3) && ((n5 !== 24) || (n4 === 3)) ? new Array(0, 31, 60, 91, 121, 152, 182, 213, 244, 274, 305, 335, 366) :  new Array(0, 31, 59, 90, 120, 151, 181, 212, 243, 273, 304, 334, 365);
    let index = n2 >> 6;
    while (n2 >= arr[index]) {
      index++;
    }
    if (w === 'month') {
      return index;
    }
    return parseInt((n2 - arr[index -1]) + 1);
  },

  format (format = setup.dateFormat) {
    let
      me = this,
      d = me.day,
      dw = me.dayOfWeek,
      ms = me.millisecond,
      h = me.hour,
      m = me.minute,
      s = me.second,
      mo = me.month,
      y = me.year,
      f = {};

    f.dddd = dayNamesLong[dw];
    f.ddd = dayNamesShort[dw];
    f.dd = pad(d);
    f.d = d;
    f.fff = ms;
    f.ff = Math.round(ms / 10);
    f.f = Math.round(ms / 100);
    f.hh = pad(h > 12 ? h - 12 : h);
    f.h = h > 12 ? h - 12 : ( h === 0 ? 12 : h);
    f.HH = pad(h);
    f.H = h;
    f.mm = pad(m);
    f.m = m;
    f.MMMM = monthNamesLong[mo];
    f.MMM = monthNamesShort[mo];
    f.MM = pad(mo);
    f.M = mo;
    f.ss = pad(s);
    f.s = s;
    f.tt = h >= 12 ? config.timePeriodPM : config.timePeriodAM;
    f.t = h >= 12 ? config.timePeriodP : config.timePeriodA;
    f.yyyy = padLeft(y, 4, '0');
    f.yyy = y;
    f.yy = padLeft(y, 2, '0');
    f.y = y;
    f[":"] = setup.timeSeparator;
    f["/"] = setup.dateSeparator;

    let
      out = '',
      fmt = format.split(/(dddd|ddd|dd|d|fff|ff|f|hh|h|HH|H||mm|m|MMMM|MMM|MM|M|ss|s|tt|t|yyyy|yyy|yy|y)?/);

    for (let i = 0; i < fmt.length; i++) {
      if (fmt[i]) {
        if (f[fmt[i]]) {
          out += f[fmt[i]];
        } else {
          out += fmt[i];
        }
      }
    }
    return out;
  }

};

DateTime.now = () => {
  return new DateTime();
};

DateTime.utcNow = () => {
	let d = new Date();
	return new DateTime(d.getUTCFullYear(), d.getUTCMonth(), d.getUTCDate(), d.getUTCHours(), d.getUTCMinutes(), d.getUTCSeconds(), d.getUTCMilliseconds());
};

DateTime.today = () => {
  return toDateTime(new Date());
};

DateTime.endOfMonth = (year, month) => {
  return toDateTime(new Date(new Date().setFullYear(year, month, 0)));
};

DateTime.equals = (datetime1, datetime2) => {
  if (!datetime1 && !datetime2) { return true; }
  if (datetime1 && !datetime2) { return false; }
  if (!datetime1 && datetime2) { return false; }
  return datetime1.span.equals(datetime2.span);
};

DateTime.compare = (datetime1, datetime2) => {
  return datetime1.span.compare(datetime2.span);
};

DateTime.between = (datetime, minDate, maxDate) => {
  return minDate.span.ms <= datetime.span.ms && datetime.span.ms <= maxDate.span.ms;
};

DateTime.isLeapYear = isLeapYear;
DateTime.daysInMonth = daysInMonth;
DateTime.parse = parseDate;

// constructor
function TimeSpan (...a) {
  let
    me = this,
    d = 0,
    h = 0,
    m = 0,
    s = 0,
    ms = 0;

  switch (a.length) {
    case 0:
     break;

    case 1:
      ms = a[0];
      break;

    case 2:
      d = a[0];
      h = a[1];
      break;

    case 3:
      h = a[0];
      m = a[1];
      s = a[2];
      break;

    case 4:
      d = a[0];
      h = a[1];
      m = a[2];
      s = a[3];
      break;

    case 5:
      d = a[0];
      h = a[1];
      m = a[2];
      s = a[3];
      ms = a[4];
      break;

    default:
      throw("No constructor of TimeSpan supports " + a.length + " arguments");
  }

  this.ms = ((d * 86400) + (h * 3600) + (m * 60) + s) * 1000 + ms;  // milliseconds

  let
    dp = Object.defineProperty,
    _d = me.round(me.ms / (24 * 3600 * 1000)),
    _h = me.round((me.ms % (24 * 3600 * 1000)) / (3600 * 1000)),
    _m = me.round((me.ms % (3600 * 1000)) / (60 * 1000)),
    _s = me.round((me.ms % 60000) / 1000),
    _ms = me.round(me.ms % 1000),
    _td = me.ms / (24 * 3600 * 1000),
    _th = me.ms / (3600 * 1000),
    _tm = me.ms / (60 * 1000),
    _ts = me.ms / 1000;

  dp(me, 'days', { get() { return _d; } });
  dp(me, 'hours', { get() { return _h; } });
  dp(me, 'minutes', { get() { return _m; } });
  dp(me, 'seconds', { get() { return _s; } });
  dp(me, 'milliseconds', { get() { return _ms; } });
  dp(me, 'totalDays', { get() { return _td; } });
  dp(me, 'totalHours', { get() { return _th; } });
  dp(me, 'totalMinutes', { get() { return _tm; } });
  dp(me, 'totalSeconds', { get() { return _ts; } });
  dp(me, 'totalMilliseconds', { get() { return this.ms; } });
}

TimeSpan.prototype = {

  toJSON () {
    return this.toTimeFormatISO();
  },

  duration () {
    return new TimeSpan(Math.abs(this.ms));
  },

  add (span) {
    return new TimeSpan(span.ms + this.ms);
  },

  compare (span) {
    if (this.ms > span.ms) { return 1; }
    if (this.ms < span.ms) { return -1; }
    return 0;
  },

  equals (span) {
    return this.ms === span.ms;
  },

  negate () {
    this.ms *= -1;
  },

  subtract (span) {
    return new TimeSpan(this.ms - span.ms);
  },

  round (n) {
    return this.ms < 0 ? Math.ceil(n) : Math.floor(n);
  },

  isEmpty () {
    // return this.ms === 0; To accept 0000
  },

  toTimeFormat (format = setup.timeFormat) {
    const date = toDate('1970-01-01T' + this.toString(), true);
    return date.format(format);
  },

  toTimeDisplayFormat () {
    return this.toTimeFormat(setup.timeDisplayFormat);
  },

  toTimeFormatISO () {
    return this.toTimeFormat(config.timeFormatISO);
  },

  toString () {
    return (this.ms < 0 ? '-' : '') + (Math.abs(this.days) ? pad(Math.abs(this.days))  + '.': '') + pad(Math.abs(this.hours)) + ':' + pad(Math.abs(this.minutes)) + ':' + pad(Math.abs(this.seconds)) + '.' + Math.abs(this.milliseconds);
  }

};

TimeSpan.parse = parseTime;

export function isDefined (obj) {
  return typeof obj !== 'undefined' && obj !== null;
}

export function isString (obj) {
  return typeof obj === 'string';
}

export function isNumber (obj) {
  return typeof obj === 'number';
}

export function isInteger (obj) {
  let i = obj / 1;
  if (isNaN(i)) { return false; }
  return Number.isInteger(i);
}

export function isDecimal (obj) {
  let i = obj / 1;
  return isNaN(i) ? false : true;
}

export function isBoolean (obj) {
  return typeof obj === 'boolean';
}

export function isFunction (obj) {
  return typeof obj === 'function';
}

export function isEvent (obj) {
  return obj instanceof Event;
}

export function isObject (obj) {
  return (typeof obj === 'object') && !Array.isArray(obj) && obj !== null;
}

export function isArray (obj) {
  return Array.isArray(obj);
}

export function isDate (dateText, isoFormat = false) {
  return DateTime.parse(dateText, isoFormat) ? true : false;
}

export function isTime (timeText) {
  return isDate('1970-01-01T' + timeText, true);
}

export function isPromiseSupported () {
  return typeof window !== 'undefined' && isDefined(window.Promise);
}

export function isPageId (pageId) {
  return config.pageIdPattern.test(pageId);
}

export function toDate (dateText, isoFormat = false) {
  return new DateTime(dateText, isoFormat);
}

export function toTime (timeText, isoFormat = false) {
  const info = TimeSpan.parse(timeText, isoFormat);
  if (info) {
    return new TimeSpan(info.hour, info.minute, 0);
  } else {
    return null;
  }
}

export function toBoolean (value) {
  return (value.toString() === 'true');
}

export function toDecimal (value) {
  return Math.round( value * 100 ) / 100;
}

export function getArrayItem (array, key, value) {
  const index = getArrayIndex(array, key, value);
  if (index === -1) { return undefined; }
  return array[index];
}

export function getArrayIndex (array, key, value) {
  if (!array || !key || !Array.isArray(array)) { return -1; }
  return array.findIndex(obj => obj[key] === value);
}

export function deleteArrayItem (array, item) {
  if (Array.isArray(array)) {
    const index = array.indexOf(item);
    if (index > -1) {
      array.splice(index, 1);
    }
  }
}

export function sumArray (array, propertyName) {
  if (!array || !Array.isArray(array)) {
    return 0;
  }
  if (propertyName) {
    // handling array of objects
    return array.reduce(function (a, b) {
      return a + b[propertyName];
    }, 0);
  } else {
    return array.reduce((a, b) => a + b, 0);
  }
}

export function shuffleArray (array, includeFirstElement = false) {
  if (!array || !Array.isArray(array)) {
    return null;
  }
  let
    tempValue,
    randomIndex,
    currentIndex = array.length;

  while (currentIndex !== 0) {
    randomIndex = Math.floor(Math.random() * currentIndex);
    currentIndex -= 1;

    if (includeFirstElement || (!includeFirstElement && randomIndex > 0)) {
      tempValue = array[currentIndex];
      array[currentIndex] = array[randomIndex];
      array[randomIndex] = tempValue;
    }
  }

  return array;
}

export function scrollBehavior (to, from, savedPosition) {
  return new Promise((resolve) => {
    setTimeout(() => {
      if (savedPosition) {
        resolve(savedPosition);
      } else {
        const position = {};
        if (to.hash) {
          position.selector = to.hash;
          if (document.querySelector(to.hash)) {
            resolve(position);
          }
          // retain current scroll position
          resolve(false);
        }
        position.x = 0;
        position.y = 0;
        resolve(position);
      }
    }, 1000);
  });
}

export function padLeft (value, length, padChar = ' ') {
  return (padChar.repeat(length) + value).substring(Math.min(('' + value).length, length));
}

export function navigate (route, replaceUrlFlag = false) {
  let target;
  if (typeof route === 'string') {
    target = route.startsWith('/') ? route : { name: route };
  } else {
    target = route;
  }
  if (replaceUrlFlag) {
    router.replace(target, () => {}, () => {});
  } else {
    router.push(target, () => {}, () => {});
  }
}

export function showHttpFault (fault, replaceUrlFlag = true) {
  const
    url = new URL(fault.url),
    target = {
      name: 'fault',
      params: { httpCode: fault.status },
      query: { statusText: fault.statusText, pathName: url.pathname.replace('/api/', '') + url.search }
    };

  if (replaceUrlFlag) {
    router.replace(target);
  } else {
    router.push(target);
  }
}

export function getFieldDef (id) {
  return id in dict ? dict[id] : null;
}

export function getMetaItem (meta, id) {
  return (meta && id && (id in meta)) ? meta[id] : null;
}

export function getRandomInteger (maxValue) {
  return getRandomBetween(1, maxValue);
}

export function getRandomBetween (minValue, maxValue) {
  return minValue + Math.floor(Math.random() * (maxValue - minValue + 1));
}

export function toIntegerFormat (value, isBlankWhenZero = true) {
  return value === 0 && isBlankWhenZero ? '' : value.toLocaleString(setup.locale);
}

export function toDecimalFormat (value, decimalDigits = setup.decimalDigits, isBlankWhenZero = false) {
  if (!isNumber(value)) { return ''; }
  return value === 0 && isBlankWhenZero ? '' : value.toLocaleString(setup.locale, { minimumFractionDigits: decimalDigits, maximumFractionDigits: decimalDigits });
}

export function toFinancialFormat (value, decimalDigits = setup.decimalDigits) {
  const amount = toDecimalFormat(value, decimalDigits);
  if (amount.startsWith('-')) {
    return '(' + amount.substr(1) + ')';
  }
  return amount;
}

export function isSmallDevice () {
  return (window.screen.width <= 576);    // pixels
}

export function hex4 () {
  return (((1+Math.random())*0x10000)|0).toString(16).substring(1);
}

export function hex8 () {
  return hex4() + hex4();
}

export function newDate (year, month, day) {
  return new DateTime(year, month, day);
}

export function emptyDateTime () {
  return new DateTime(1, 1, 1);
}

export function getTheme (themeId) {
  const t = {
    baseColor: themeId === 'dark' ? 'slategray' : themeId,
    formCaptionColor: 'caption-' + themeId,
    outlineColor: 'outline-' + themeId,
    buttonColor: themeId === 'dark' ? 'slategray' : themeId + '-light'
  };
  t.inputFocusColor = t.buttonColor;

  t.tableColor = 'white';
  t.tableCaptionColor = t.buttonColor;
  t.tableHeaderColor = 'ivory';

  if (themeId === 'info') {
    t.tableColor = 'aliceblue';
  }
  if (themeId === 'success') {
    t.tableColor = 'honeydew';
  }
  if (themeId === 'warning') {
    t.tableColor = 'lightyellow';
  }
  if (themeId === 'danger') {
    t.tableColor = 'oldlace';
  }
  if (themeId === 'primary') {
    t.tableColor = 'floralwhite';
  }
  if (themeId === 'dark') {
    t.tableHeaderColor = 'gainsboro';
    t.tableColor = 'gray';
  }

  return t;
}

export function escText (text) {
  return text.replace(/'/g, '%25');
}

export function toDateDisplayFormat (dateText, isoFormat = true) {
  return toDateFormat(dateText, isoFormat, setup.dateDisplayFormat);
}

export function toDateTimeDisplayFormat (dateText, isoFormat = true) {
  return toDateFormat(dateText, isoFormat, setup.dateDisplayFormat + ' ● h:mm tt');
}

export function toDateFormat (dateText, isoFormat = true, format = setup.dateFormat) {
  if (!dateText) { return ''; }
  const date = toDate(dateText, isoFormat);
  return date.format(format);
}

export function toTimeFormatISO (timeText) {
  if (!timeText) { return ''; }
  return toTimeFormat(timeText, config.timeFormatISO);
}

export function toTimeFormat (timeText, format = setup.timeFormat) {
  const date = toDate('1970-01-01T' + timeText, true);
  return date.format(format);
}

export function toTimeDisplayFormat (timeText, format = setup.timeDisplayFormat) {
  return toTimeFormat(timeText, format);
}

export function toPrettyFileSize (size) {
  const i = size === 0 ? 0 : Math.floor( Math.log(size) / Math.log(1024) );
  return ( size / Math.pow(1024, i) ).toFixed(2) * 1 + ' ' + ['B', 'KB', 'MB', 'GB', 'TB'][i];
}

// export function toWords (s) {
//   const
//     th = ['','thousand','million', 'billion','trillion'],
//     dg = ['zero','one','two','three','four','five','six','seven','eight','nine'],
//     tn = ['ten','eleven','twelve','thirteen', 'fourteen','fifteen','sixteen','seventeen','eighteen','nineteen'],
//     tw = ['twenty','thirty','forty','fifty','sixty','seventy','eighty','ninety'];

//   let s = s.toString();
//   s = s.replace(/[\, ]/g,'');
//   if (s !== parseFloat(s)) { return 'not a number' };

//   let x = s.indexOf('.');
//   if (x === -1) x = s.length;
//   if (x > 15) return 'too big';
//   var n = s.split('');
//   var str = '';
//   var sk = 0;
//   for (var i = 0; i < x; i++) {
//     if ((x-i) % 3 === 2) {
//       if (n[i] === '1') {
//         str += tn[Number(n[i+1])] + ' ';
//         i++;
//         sk = 1;
//       } else if (n[i] !== 0) {
//         str += tw[n[i]-2] + ' ';
//         sk = 1;
//       }
//     } else if (n[i] !== 0) {
//       str +=  dg[n[i]] +' ';
//       if ((x-i) % 3 === 0) str += 'hundred ';sk=1;
//     }

//     if ((x-i) % 3 === 1) {
//       if (sk)
//   str += th[(x-i-1)/3] + ' ';sk=0;}} if (x != s.length) {var y = s.length; str +=
//   'point '; for (var i=x+1; istr.replace(/\s+/g,' ');}

// }

export function setPageTitle (title) {
  if (title && (document.title !== title)) {
    document.title = title;
  }
}

export function convertDates (obj) {
  if (!obj) { return {}; }
  let
    value,
    newObj = Object.assign({}, obj);

  Object.keys(obj).forEach( key => {
    value = obj[key];
    if (typeof value === 'string' && value.length === 10) {
      if (isDate(value, true)) {
        newObj[key] = toDate(value, true);
      }
    }
  });

  return newObj;
}

// 16 May 2025 - EMT
export function convertTimes (obj) {
  if (!obj) { return {}; }
  let
    value,
    newObj = Object.assign({}, obj);

  Object.keys(obj).forEach( key => {
    value = obj[key];
    if (typeof value === 'string' && value.length === 8) {
      if (isTime(value, true)) {
        newObj[key] = toTime(value, true);
      }
    }
  });

  return newObj;
}

export function getFaultText (fault) {
  if (fault && 'message' in fault) {
    return fault.message.split(/\n/)[0];
  }
  return 'Problem encountered...Try refreshing the page.';
}

export function isValidEmail (email) {
  return regExEmail.test(email);
}

export function round (value, precision = 0) {
  if (!precision) { return Math.round(value); }
  let m = Math.pow(10, precision);
  return Math.round(value * m) / m;
}

export function getOrdinal (value, suffixOnly = false) {
  let sfx = 'th';
  if (value % 10 === 1 && value % 100 !== 11) {
    sfx = 'st';
  } else if (value % 10 === 2 && value % 100 !== 12) {
    sfx = 'nd';
  } else if (value % 10 === 3 && value % 100 !== 13) {
    sfx = 'rd';
  }
  return suffixOnly ? sfx : (value + sfx);
}

export function resetObject (obj) {
  if (!isObject(obj)) { return; }

  let value;
  Object.keys(obj).forEach( key => {
    value = obj[key];
    if (isString(value)) { obj[key] = ''; }
    if (isNumber(value)) { obj[key] = 0; }
    if (isBoolean(value)) { obj[key] = false; }
    if (value instanceof DateTime) { obj[key] = null; }
    if (value instanceof TimeSpan) { obj[key] = null; } // 16 May 2025 - EMT
  });
}

export function focusNext () {
  const
    active = document.activeElement,
    page = sessionMgr.currentModalElement || sessionMgr.currentPageElement;

  if (active && page) {
    const selectors = 'a:not(.disabled), input:not([disabled]), button:not([disabled]):not(.static):not(.input-button):not(.select-button), select:not([disabled]), textarea:not([disabled])';
    const targets = Array.prototype.filter.call(page.querySelectorAll(selectors),
      element => {
        // check for visibility; always include the current activeElement
        return element.offsetWidth > 0 || element.offsetHeight > 0 || element === active;
      }
    );
    if (targets) {
      const index = Array.prototype.indexOf.call(targets, active);
      if (index > -1) {
        const nextElement = targets[index + 1] || targets[0];
        nextElement.focus();
      }
    }
  }
}

export function getAjaxOptions (method, body) {
  const options = {
    method: method,
    headers: {
      Accept: 'application/json',
      'Content-Type': 'application/json'
    }
  };

  if (body) {
    options.body = body;
  }

  return options;
}

export function shortenText (text, maxLength=200) {
  let t = text;
  if (t.length > maxLength) {
    t = t.substr(0, maxLength - 3) + '...';
  }
  return t;
}

export function checksum ( text ) {
  let
    cs = 0x12345678,
    len = text.length;

  for (let i = 0; i < len; i++) {
    cs += (text.charCodeAt(i) * (i + 1));
  }
  return (cs & 0xffffffff);
}

export function getExtension (fileName) {
  return fileName.split('.').pop();
}

export function getTimestampQuery (stampId) {
  return (stampId ? stampId + '=' : 'ts=') + Date.now().toString();
}

export function isReachable (url) {
  return getLinkStatus(url).then(
    () => {
      return Promise.resolve(true);
    },
    () => {
      return Promise.resolve(false);
    }
  );
}

export function debounce (cb, delay = 500) {
  let timeout;

  return (...args) => {
    clearTimeout(timeout);
    timeout = setTimeout(() => {
      cb(...args);
    }, delay);
  }
}

export function guid () {
  return "10000000-1000-4000-8000-100000000000".replace(/[018]/g, c =>
    (+c ^ crypto.getRandomValues(new Uint8Array(1))[0] & 15 >> +c / 4).toString(16)
  );
}

export {
  config,
  sessionDb,
  localDb,
  sessionMgr,
  store,
  DateTime,
  TimeSpan,
  intKeys,
  decKeys,
  dateKeys,
  router
};
