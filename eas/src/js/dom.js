/* jslint esversion: 6  */

let
  scrollbarWidth = null,
  savedScreenSize = null;

export function isElement (el) {
  return el && el.nodeType === Node.ELEMENT_NODE;
}

export function cursorWait (elementId) {
  setCursor('wait', elementId);
}

export function cursorDefault (elementId) {
  setCursor('default', elementId);
}

export function setCursor (cursorName, elementId) {
  let el;
  if (elementId) {
    el = document.getElementById(elementId);
  } else {
    el = document.getElementsByTagName('html')[0];
  }
  if (el) { el.style.cursor = cursorName; }
}

export function addClass (el, name) {
  if (!isElement(el)) { return; }
  el.classList.add(name);
}

export function removeClass (el, name) {
  if (!isElement(el)) { return; }
  el.classList.remove(name);
}

export function hasClass (el, name) {
  if (!isElement(el)) { return false; }
  return (' ' + el.className + ' ').indexOf(' ' + name + ' ') > -1;
}

export function addAttribute (el, name, value="") {
  if (!isElement(el)) { return; }
  el.setAttribute(name, value);
}

export function removeAttribute (el, name) {
  if (!isElement(el)) { return; }
  el.removeAttribute(name);
}

export function hasAttribute (el, name) {
  if (!isElement(el)) { return false; }
  return el.hasAttribute(name);
}

export function removeElement (el) {
  if (isElement(el) && isElement(el.parentNode)) {
    el.parentNode.removeChild(el);
  }
}

export function enable(el) {
  removeAttribute(el, 'disabled');
}

export function disable(el) {
  addAttribute(el, 'disabled');
}

export function toggleAttribute(attributeName, addFlag, ...classNames) {
  let elements;

  classNames.forEach( className => {
    elements = document.body.getElementsByClassName(className);
    for (let i=0; i < elements.length; i++) {
      if (addFlag) {
        addAttribute(elements[i], attributeName);
      } else {
        removeAttribute(elements[i], attributeName);
      }
    }
  });
}

export function on (obj, event, handler) {
  obj.addEventListener(event, handler);
}

export function off (obj, event, handler) {
  obj.removeEventListener(event, handler);
}

export function getImageSource (imgBase64, format = 'jpg') {
  if (!imgBase64 || imgBase64.startsWith('data:image') ) { return imgBase64; }
  return 'data:image/' + format + ';base64,' + imgBase64;
}

export function extractImage (imgBase64) {
  if (imgBase64.startsWith('data:image')) {
    let index = imgBase64.indexOf(';base64,');
    return imgBase64.substr(index + 8);
  }
  return imgBase64;
}

export function scrollIntoView (el, nearestFlag = true) {
  if (el && isElement(el) && isElement(el.parentNode)) {
    if (nearestFlag) {
      el.scrollIntoView({ behavior: "smooth", block: "nearest", inline: "start" });
    } else {
      el.scrollIntoView();
    }
  }
}

export function scrollToTop (el) {
  if (isElement(el) && isElement(el.parentNode)) {
    el.scrollTop = 0;
  }
}

export function scrollToBottom (el) {
  if (isElement(el) && isElement(el.parentNode)) {
    el.scrollTop = el.scrollHeight;
  }
}

export function hasScrollbar () {
  return document.documentElement.scrollHeight > document.documentElement.clientHeight;
}

export function showSpinner () {
  const el = document.getElementById('spinner');
  if (el) {
    el.style.display = 'block';
  }
}

export function hideSpinner () {
  const el = document.getElementById('spinner');
  if (el) {
    el.style.display = 'none';
  }
}

export function showMenu () {
  const el = document.getElementById('navbar');
  if (el) {
    addClass(el, "show");
  }
}

export function hideMenu () {
  const el = document.getElementById('navbar');
  if (el) {
    removeClass(el, "show");
  }
}

export function showGoTop () {
  const el = document.getElementById('gotop');
  if (el) {
    removeClass(el, "d-none");
  }
}

export function hideGoTop () {
  const el = document.getElementById('gotop');
  if (el) {
    addClass(el, "d-none");
  }
}

export function flash (el, iterations = 3) {
  flashElement(el, 'flash', iterations);
}

export function flashFault (el, iterations = 3) {
  flashElement(el, 'flash-fault', iterations);
}

function flashElement (el, className, iterations = 3) {
  if (isElement(el) && isElement(el.parentNode)) {
    let
      cls = className,
      timeout = 4000;

    if (iterations && iterations >= 1 && iterations <= 5) {
      cls = cls + '-' + iterations.toString();
      timeout = (1000 * iterations) + 1000;
    } else {
      cls = cls + '-3';
    }
    addClass(el, cls);
    setTimeout(() => {
      removeClass(el, cls);
    }, timeout);
  }
}

export function isIE11 () {
  return !!window.MSInputMethodContext && !!document.documentMode;
}

export function isIE10 () {
  return window.navigator.appVersion.indexOf('MSIE 10') !== -1;
}

export function getViewportSize () {
  const
    el = document.documentElement,
    w = Math.max(el.clientWidth, window.innerWidth || 0),
    h = Math.max(el.clientHeight, window.innerHeight || 0);

  return { w, h };
}

export function getScrollbarWidth (recalculate = false) {
  const
    doc = document,
    body = doc.body,
    screenSize = getViewportSize();

  // return directly when already calculated & not force recalculate & screen size not changed
  if (scrollbarWidth !== null && !recalculate && screenSize.height === savedScreenSize.height && screenSize.width === savedScreenSize.width) {
    return scrollbarWidth;
  }
  if (doc.readyState === 'loading') {
    return null;
  }
  const
    div1 = doc.createElement('div'),
    div2 = doc.createElement('div');

  div1.style.width = div2.style.width = div1.style.height = div2.style.height = '100px';
  div1.style.overflow = 'scroll';
  div2.style.overflow = 'hidden';
  body.appendChild(div1);
  body.appendChild(div2);
  scrollbarWidth = Math.abs(div1.scrollHeight - div2.scrollHeight);
  removeElement(div1);
  removeElement(div2);

  // save new screen size
  savedScreenSize = screenSize;
  return scrollbarWidth;
}

export function hideScrollbar () {
  document.getElementsByTagName("body")[0].style.overflowY = 'hidden';
}

export function showScrollbar (always = true) {
  document.getElementsByTagName("body")[0].style.overflowY = always ? 'scroll' : 'auto';
}

// export function isOverflow (el) {
//   let overflow = el.style.overflow;

//   if ( !overflow || overflow === "visible" ) {
//     el.style.overflow = "hidden";
//   }

//   let isOverflowing = el.clientWidth < el.scrollWidth || el.clientHeight < el.scrollHeight;
//   el.style.overflow = overflow;

//   return isOverflowing;
// }